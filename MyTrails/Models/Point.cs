using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyTrails.Models
{
    public class Point
    {

        /// <summary>
        /// Latitude at this location
        /// </summary>
        [Key, Column(Order=0)]
        public Decimal Latitude { get; set; }
        /// <summary>
        /// Longitude at this location
        /// </summary>
        [Key, Column(Order=1)]
        public Decimal Longitude { get; set; }
        /// <summary>
        /// Elevation at this location
        /// </summary>
        public int Elevation { get; set; }
        /// <summary>
        /// Trail on which this point is located
        /// </summary>
        public virtual Trail Trail { get; set; }

    }
}