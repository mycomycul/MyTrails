using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrails.Models
{

    /// <summary>
    /// Longer Posts or trip reports submitted by users
    /// Not 
    /// </summary>
    public class Post
    {
        public Post()
        {
            throw new NotImplementedException();
            //this.Trails = new HashSet<Trail>();
        }
        public string Id { get; set; }
        ///// <summary>
        ///// Full Length Forum Post
        ///// </summary>
        //public string LongReport { get; set; }
        //public ApplicationUser Author { get; set; }
        //public DateTime AuthoredDate { get; set; }
        //public DateTime TripDate { get; set; }
        //public DateTime EndDate { get; set; }
        ///// <summary>
        ///// String of HTML element id that will be linked to point on the map for story map navigation
        ///// </summary>
        //public IDictionary<string, Point> MapLinks { get; set; }
        ///// <summary>
        ///// List of all trails that were travelled, even partially
        ///// </summary>
        public virtual ICollection<Trail> Trails { get; set; }//Many to Many


        //TODO: Create Constructor that adds all trails travelled based on Points included.
        //Should also account for manually including trail if a point was not included.

    }
}