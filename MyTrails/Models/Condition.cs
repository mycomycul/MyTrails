using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyTrails.Models
{
    ///TODO: Create Base class for condition and child classes for rainier, olympic and user submitted?
    /// <summary>
    /// Data Scraped from NPS websites
    /// </summary>
    public class Condition
    {
        public string Id { get; set; }
        [DisplayName("% Snow")]
        public int PercentSnowCover { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public string TrailId { get; set; }
        public virtual Trail Trail { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }


        public Condition()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}