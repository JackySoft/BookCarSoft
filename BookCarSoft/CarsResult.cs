using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BookCarSoft
{
    [DataContract]
    class CarsResult
    {
        [DataMember(Order = 0)]
        public List<CarsList> Result { get; set; }
        [DataMember(Order = 1)]
        public int Total { get; set; }

        public CarsResult(List<CarsList> Result, int Total)
        {
            this.Result = Result;
            this.Total = Total;
        }

    }
}
