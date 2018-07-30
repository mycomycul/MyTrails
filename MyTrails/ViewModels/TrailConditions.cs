using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrails.ViewModels
{
    public class TrailConditions
    {

        public string TrailName { get; set; }
        public string Zone { get; set; }
        public string Description { get; set; }
        public string Elevation { get; set; }
        public float? Miles { get; set; }
        public string Conditions { get; set; }
        /// <summary>
        /// Link to information page on NPS.gov
        /// </summary>
        public string InfoLink { get; set; }
        public DateTime Updated { get; set; }

    }
}