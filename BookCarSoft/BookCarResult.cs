using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BookCarSoft
{
    [DataContract]
    class BookCarResult
    {
        [DataMember(Order = 0)]
        public string data { get; set; }
        [DataMember(Order = 1)]
        public int code { get; set; }
        [DataMember(Order = 2)]
        public string message { get; set; }


        public BookCarResult(string data, int code, string message)
        {
            this.data = data;
            this.code = code;
            this.message = message;
        }
    }
}
