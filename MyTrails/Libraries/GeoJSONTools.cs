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
        ApplicationDbContext db = new ApplicationDbContext();
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
               // newTrail.ShortDescription = item["attributes"]["NOTES"];
                trailStore.Add(newTrail);

                }
            }
            Console.Read();

        }
        public void InputGeoJson2()
        {
            //var t = JsonConvert.DeserializeObject(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"));
            JObject t = JObject.Parse(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"));
            dynamic trails = t;
            foreach (dynamic trail in trails.features)
            {
   
                    // Console.Read();
            }


        }

        }

    }
