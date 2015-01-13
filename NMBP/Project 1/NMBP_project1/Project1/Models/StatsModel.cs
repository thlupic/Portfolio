using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project1.Models
{
    public class StatsModel
    {
        public Core.analyseResults analyseResults { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool isByHour { get; set; }

        public StatsModel()
        {
            analyseResults = new Core.analyseResults();
        }

    }
}
