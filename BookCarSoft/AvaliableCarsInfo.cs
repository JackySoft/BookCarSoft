using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BookCarSoft
{
    [DataContract]
    class AvaliableCarsInfo
    {
       [DataMember]
        public CarsResult data { get; set; }
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string message { get; set; }
        public AvaliableCarsInfo(CarsResult data, string code, string message)
        {
            this.data = data;
            this.code = code;
            this.message = message;
        }
    }
}
