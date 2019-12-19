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
        
        //保存已订阅的回调URL
        public static HashSet<string> callbackList = new HashSet<string>();

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