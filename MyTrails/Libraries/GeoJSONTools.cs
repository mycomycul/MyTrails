using MyTrails.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MyTrails.Libraries
{
    public class GeoJSONTools
    {
       public void AddElevation()
        {

        }

       public void InputGeoJson1(string filename)
        {

            dynamic trail = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"));
            //dynamic trailStore = new ExpandoObject();
            var trailStore = new List<Trail>();
            var trailFeatures = new List<string>();

            for (int i = 0; i < trail["fields"].Count; i++)
            {
                trailFeatures.Add((string)trail["fields"][i]["name"]);
            }
            foreach (var item in trail["features"])
            {
                using(ApplicationDbContext db = new ApplicationDbContext()) {
                    var currentTrail = item["attributes"];
                    Trail newTrail = new Trail();
                //Trail newTrail = db.Trails.Where(x => x.TrailName.ToUpper == currentTrail["TRLNAME"])
                newTrail.Id = item["attributes"]["OBJECTID"];
                newTrail.TrailName = item["attributes"]["TRLNAME"];
                newTrail.Status = item["attributes"]["TRLSTATUS"];
                newTrail.ShortDescription = item["attributes"]["NOTES"];
                trailStore.Add(newTrail);

                }
            }
            Console.Read();

        }
        public void InputGeoJson2(string filename)
        {
            //var t = JsonConvert.DeserializeObject(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"));
            var t = JObject.Parse(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"));
            IList<JToken> results = t["features"].Children().ToList();
            IList<TrailModel> trails = new List<TrailModel>();
            foreach (JToken item in results)
            {
                TrailModel trail = item["attributes"].ToObject<TrailModel>();
                trail.GEOMETRY = item["geometry"]["paths"][0];
                trails.Add(trail);
                // Console.Read();
            }
            Console.Read();
        }


        public class TrailModel
        {
            //public object displayFieldName { get; set; }
            //public object fieldAliases { get; set; }
            //public object features { get; set; }
            //public object fields { get; set; }
            //public object geometryType { get; set; }

            //public object spatialReference { get; set; }

            public string TRLNAME { get; set; }
            public string TRLSTATUS { get; set; }
            public string NOTES { get; set; }
            public string UNITCODE { get; set; }
            public object GEOMETRY { get; set; }

        }
    }
}