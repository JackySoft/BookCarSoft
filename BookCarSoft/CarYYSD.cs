using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BookCarSoft
{
    [DataContract]
    public class CarYYSD
    {
        [DataMember]
        public string Xnsd { get; set; }
        [DataMember]
        public string XnsdName { get; set; }
        public CarYYSD(string Xnsd, string XnsdName)
        {
            this.Xnsd = Xnsd;
            this.XnsdName = XnsdName;
        }
    }
}
