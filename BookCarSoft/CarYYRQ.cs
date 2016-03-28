using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BookCarSoft
{
    [DataContract]
    public class CarYYRQ
    {
        [DataMember]
        public string Yyrq { get; set; }
        [DataMember]
        public string DisplayWeek { get; set; }
        [DataMember]
        public string DisplayYyrq { get; set; }

        public CarYYRQ(string Yyrq, string DisplayWeek, string DisplayYyrq)
        {
            this.Yyrq = Yyrq;
            this.DisplayWeek = DisplayWeek;
            this.DisplayYyrq = DisplayYyrq;
        }
    }
}
