using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BookCarSoft
{
    [DataContract]
    class MessageInfo
    {
        [DataMember]
        public CarInfo data { get; set; }
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string message { get; set; }
        public MessageInfo(CarInfo data, string code, string message)
        {
            this.data = data;
            this.code = code;
            this.message = message;
        }
    }
}
