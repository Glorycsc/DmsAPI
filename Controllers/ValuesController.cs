using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using APIGATEWAY_SDK;
using DmsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace DmsAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static HashSet<string> callbackList = new HashSet<string>();
        private static bool flag = true;

        // GET api/display
        [HttpPost("startconsume")]
        public void StartConsume()
        {
            flag = true;
            while (flag)
            {
                Loop();
            }

            return;
        }


        // GET api/display
        [HttpPost("stopconsume")]
        public void StopConsume()
        {
            flag = false;
            return;
        }


        // GET api/subscribe
        [HttpPost("subscribe")]
        public ActionResult<string> Subscribe(DmsReqBodyModel drBody)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Console.WriteLine(printStr(callbackList));
            if (drBody.Callback != string.Empty && Regex.IsMatch(drBody.Callback,
                    "(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]"))
            {
                callbackList.Add(drBody.Callback);
                return "订阅成功";
            }

            return "订阅失败,输入的回调地址有误";
        }


        static string printStr(HashSet<string> set)
        {
            string ss = "";
            foreach (var s in set)
            {
                ss += " / " + s;
            }

            return ss;
        }


        private static void Loop()
        {
            string handlerMessageStr = ExecutRequst(
                "GET",
                "https://dms.cn-north-4.myhuaweicloud.com/v1.0/06b4630ebe00254e2fe9c0108e58951a/queues/9fc9ad5d-8337-4f7f-a1b5-3d4b7e676183/groups/g-37bf45ae-f42e-45c7-b29a-bb50ef9da4b5/messages?max_msgs=3",
                ""
            );
            //消费消息返回正确信息
            if (handlerMessageStr.EndsWith("]") && handlerMessageStr.StartsWith("["))
            {
                List<HandlerMessageModel> hdms =
                    JsonConvert.DeserializeObject<List<HandlerMessageModel>>(handlerMessageStr);

                HandlerResultModel hrm = new HandlerResultModel(new List<HandlerResultModel.Message>());

                if (hdms.Count > 0)
                {
                    foreach (var hdm in hdms)
                    {
                        hrm.message.Add(new HandlerResultModel.Message(hdm.handler, "success")); // success or fail


                        foreach (var callback in callbackList)
                        {
                            //执行回调 - 前端刷新
                            ExecutCallBack(callback, JsonConvert.SerializeObject(hdm));
                        }
                    }
                }

                //执行消息确认
                string hrmJsonStr = JsonConvert.SerializeObject(hrm);
                ExecutRequst(
                    "POST",
                    "https://dms.cn-north-4.myhuaweicloud.com/v1.0/06b4630ebe00254e2fe9c0108e58951a/queues/9fc9ad5d-8337-4f7f-a1b5-3d4b7e676183/groups/g-37bf45ae-f42e-45c7-b29a-bb50ef9da4b5/ack",
                    hrmJsonStr
                );
            }
        }


        private static string ExecutCallBack(string callback, string bodyStr)
        {
            string url = callback == string.Empty ? "http://192.168.1.206:3000/" : callback;
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.Timeout = 10 * 1000;
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", bodyStr, ParameterType.RequestBody);
            IRestResponse restResponse = client.Execute(request);
            var result = restResponse.Content;
            //     LocalEntity vnetGateway = JsonNullSafeConvert.DeserializeObject<LocalEntity>(result);
            //    return vnetGateway.Property1[0].name;
            return result;
        }


        //确认消费状态
        static string ExecutRequst(string methodStr, string urlStr, string bodyStr)
        {
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            Signer signer = new Signer();
            //Set the AK/SK to sign and authenticate the request.
            signer.Key = "K2QCGW91X0VVBSPFBW6X";
            signer.Secret = "hDK2RaaFyTHscHTm62kfeX5npjyvh6nzxmrMrqCS";

            //The following example shows how to set the request URL and parameters to query a VPC list.
            //Specify a request method, such as GET, PUT, POST, DELETE, HEAD, and PATCH.
            //Set request host.
            //Set request URI.
            //Set parameters for the request URL.
            HttpRequest r = new HttpRequest(methodStr, new Uri(urlStr));
            //Add a body if you have specified the PUT or POST method. Special characters, such as the double quotation mark ("), contained in the body must be escaped.
            r.body = bodyStr;

            //Add header parameters, for example, x-domain-id for invoking a global service and x-project-id for invoking a project-level service.
            r.headers.Add("Content-Type", "application/json");

            HttpWebRequest req = signer.Sign(r);
            req.Headers.ToString();
            //            req.
            Console.WriteLine(req.Headers.GetValues("x-sdk-date")[0]);
            Console.WriteLine(string.Join(", ", req.Headers.GetValues("authorization")));
            try
            {
                if (!"GET".Equals(methodStr.ToUpper()))
                {
                    var writer = new StreamWriter(req.GetRequestStream());
                    writer.Write(r.body);
                    writer.Flush();
                }

                HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                var reader = new StreamReader(resp.GetResponseStream());
                //      Console.WriteLine(reader.ReadToEnd());
                return reader.ReadToEnd();
            }
            catch (WebException e)
            {
                HttpWebResponse resp = (HttpWebResponse) e.Response;
                if (resp != null)
                {
                    Console.WriteLine((int) resp.StatusCode + " " + resp.StatusDescription);
                    var reader = new StreamReader(resp.GetResponseStream());
                    //     Console.WriteLine(reader.ReadToEnd());
                    return reader.ReadToEnd();
                }
                else
                {
                    Console.WriteLine(e.Message);
                    return e.Message;
                }
            }
        }


//        // GET api/consume
//        [HttpGet("consume")]
//        public ActionResult<string> Consume()
//        {
//            string handlerMessageStr = ExecutRequst(
//                "GET",
//                "https://dms.cn-north-4.myhuaweicloud.com/v1.0/06b4630ebe00254e2fe9c0108e58951a/queues/9fc9ad5d-8337-4f7f-a1b5-3d4b7e676183/groups/g-37bf45ae-f42e-45c7-b29a-bb50ef9da4b5/messages",
//                ""
//            );
//            return handlerMessageStr;
//        }
//
//        // GET api/confirm
//        [HttpGet("confirm")]
//        public ActionResult<string> Confirm()
//        {
//            return ExecutRequst(
//                "POST",
//                "https://dms.cn-north-4.myhuaweicloud.com/v1.0/06b4630ebe00254e2fe9c0108e58951a/queues/9fc9ad5d-8337-4f7f-a1b5-3d4b7e676183/groups/g-37bf45ae-f42e-45c7-b29a-bb50ef9da4b5/ack",
//                "{\n            \"message\": [\n            {\n                \"handler\":\"eyJ0b3BpYyI6InEtMDZiNDYzMGViZTAwMjU0ZTJmZTljMDEwOGU1ODk1MWEtOWZjOWFkNWQtODMzNy00ZjdmLWExYjUtM2Q0YjdlNjc2MTgzIiwiZ3JvdXAiOiJnLTM3YmY0NWFlLWY0MmUtNDVjNy1iMjlhLWJiNTBlZjlkYTRiNSIsInBhcnRpdGlvbiI6MSwiY29uc3VtZXJJZCI6IjcxMTE0NTliLTQ1ZWMtNDM1ZS05OTVjLWQ5MzgyODQ0ZWRlNiIsImNvbnN1bWVyVmVyc2lvbiI6OCwib2Zmc2V0IjoxOCwidGFnTGlzdCI6W10sInRhZ1R5cGUiOiJvciIsInJlZGVsaXZlclRpbWVzIjowfQ==\",\n                \"status\": \"success\"\n            },\n            {\n                \"handler\":\"eyJ0b3BpYyI6InEtMDZiNDYzMGViZTAwMjU0ZTJmZTljMDEwOGU1ODk1MWEtOWZjOWFkNWQtODMzNy00ZjdmLWExYjUtM2Q0YjdlNjc2MTgzIiwiZ3JvdXAiOiJnLTM3YmY0NWFlLWY0MmUtNDVjNy1iMjlhLWJiNTBlZjlkYTRiNSIsInBhcnRpdGlvbiI6MSwiY29uc3VtZXJJZCI6IjcxMTE0NTliLTQ1ZWMtNDM1ZS05OTVjLWQ5MzgyODQ0ZWRlNiIsImNvbnN1bWVyVmVyc2lvbiI6OCwib2Zmc2V0IjoxOSwidGFnTGlzdCI6W10sInRhZ1R5cGUiOiJvciIsInJlZGVsaXZlclRpbWVzIjowfQ==\",\n                \"status\": \"success\"\n            }\n            ]\n        }"
//            );
//        }


        //        // POST api/values
        //        [HttpPost]
        //        public void Post([FromBody] string value)
        //        {
        //        }
        //
        //        // PUT api/values/5
        //        [HttpPut("{id}")]
        //        public void Put(int id, [FromBody] string value)
        //        {
        //        }
        //
        //        // DELETE api/values/5
        //        [HttpDelete("{id}")]
        //        public void Delete(int id)
        //        {
        //        }
    }
}