using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BookCarSoft
{
    [DataContract]
    class MessageCode
    {
        [DataMember]
        public string data { get; set; }
        [DataMember]
        public int code { get; set; }
        [DataMember]
        public string message { get; set; }
        public MessageCode(string data, int code, string message)
        {
            this.data = data;
            this.code = code;
            this.message = message;
        }
    }
}
