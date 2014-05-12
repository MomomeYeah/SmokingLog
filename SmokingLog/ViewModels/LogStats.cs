using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmokingLog.ViewModels
{
    public class LogStats
    {
        public double TotalAverage { get; set; }
        public double TotalPacksPerWeek { get; set; }
        public double LastWeekAverage { get; set; }
        public double LastWeekPacks { get; set; }
    }
}