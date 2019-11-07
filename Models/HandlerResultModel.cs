using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DmsAPI.Models
{
    public class HandlerResultModel
    {
        public HandlerResultModel(List<Message> message)
        {
            this.message = message;
        }

        public List<Message> message { get; set; }


        public class Message
        {
            public Message(string handler, string status)
            {
                this.handler = handler;
                this.status = status;
            }

            public string handler { get; set; }
            public string status { get; set; }
        }

    }
}