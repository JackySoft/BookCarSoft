using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BookCarSoft
{
    [DataContract]
    class CarsList
    {
        [DataMember(Order = 0)]
        public string JLCBH { get; set; }
        [DataMember(Order = 1)]
        public string CNBH { get; set; }
        [DataMember(Order = 2)]
        public int YT { get; set; }

        public CarsList(string JLCBH, string CNBH, int YT)
        {
            this.JLCBH = JLCBH;
            this.CNBH = CNBH;
            this.YT = YT;
        }
    }
}
