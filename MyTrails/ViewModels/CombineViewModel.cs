using MyTrails.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrails.ViewModels
{

    /// <summary>
    /// View Model for passing names of trails in the DB and names of trial in Json data
    /// </summary>
    public class CombineViewModel
    {
        public List<string> Trails { get; set; }
        public List<string> TrailSections { get; set; }

        public CombineViewModel()
        {
        }
        public CombineViewModel(List<string> trails, List<string> trailSections)
        {
            Trails = trails;
            TrailSections = trailSections;
        }
    }
}