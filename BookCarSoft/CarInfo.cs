using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BookCarSoft
{
    [DataContract]
    public class CarInfo
    {
        [DataMember(Order = 0)]
        public List<CarYYSD> XnsdList { get; set; }
        [DataMember(Order = 1)]
        public List<CarYYRQ> YyrqList { get; set; }
        [DataMember(Order = 2)]
        public List<CarItem> UIDatas { get; set; }

        public CarInfo(List<CarYYSD> XnsdList, List<CarYYRQ> YyrqList, List<CarItem> UIDatas)
        {
            this.XnsdList = XnsdList;
            this.YyrqList = YyrqList;
            this.UIDatas = UIDatas;
        }
    }
}
