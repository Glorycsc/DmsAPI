using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DmsAPI.Models
{
    public class HandlerMessageModel
    {
        public string handler { get; set; }
        public Message message { get; set; }


        public class Message
        {
            public string body { get; set; }
            public object attributes { get; set; }
        }
    }
}