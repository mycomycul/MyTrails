using MyTrails.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrails.ViewModels
{
    public class CombineViewModel
    {
        public List<Trail> Trails { get; set; }
        public List<string> TrailSections { get; set; }
    }
}