using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BookCarSoft
{
    [DataContract]
    public class CarItem
    {
        [DataMember(Order = 0)]
        public string Yyrq { get; set; }
        [DataMember(Order = 1)]
        public string Xnsd { get; set; }
        [DataMember(Order = 2)]
        public int SL { get; set; }
        [DataMember(Order = 3)]
        public bool IsBpked { get; set; }

        public CarItem(string Yyrq, string Xnsd, int SL, bool IsBpked)
        {
            this.Yyrq = Yyrq;
            this.Xnsd = Xnsd;
            this.SL = SL;
            this.IsBpked = IsBpked;
        }
    }
}
