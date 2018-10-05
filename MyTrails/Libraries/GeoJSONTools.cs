using MyTrails.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Types;

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
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
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
            ////var t = JsonConvert.DeserializeObject(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"));
            //JObject t = JObject.Parse(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"));
            //dynamic trails = t;
            //foreach (dynamic trail in trails.features)
            //{

            //        // Console.Read();
            //}
            IList<TrailSection> otherTrails = new List<TrailSection>();
            //ICollection<TrailSection> Points = new List<TrailSection>();

            JObject t = JObject.Parse(File.ReadAllText(@"~/App_Data/OlympicTrailData.Json")) as JObject;
            dynamic traildata = t;
            var wkid = traildata.spatialReference.wkid;
            foreach (dynamic trail in traildata.features)
            {
                string trailName = trail.attributes.TRLNAME;
                var ts = CreateTrailSection(trailName, trail, wkid);

                //Check if a matching trail name in the database exists and if so add current trail section to trail
                if (db.Trails.Where(x => x.TrailName.ToUpper() == trailName).Any())
                {
                    var currentTrail = db.Trails.Where(x => x.TrailName.ToUpper() == trailName).First();
                    if (currentTrail.TrailSections.Count < 1)
                    {
                        currentTrail.Status = ts.Status;
                    }
                    currentTrail.TrailSections.Add(ts);
                }
                else
                {
                    otherTrails.Add(ts);
                }

                // Console.Read();
            }
            db.SaveChanges();

        }

        public TrailSection CreateTrailSection(string trailname, dynamic trail, string wkid)
        {
            string LineType = GeometryType(trail.geometry.paths[0].Count);
            //Check if this should be a point or line
            TrailSection ts = new TrailSection();
            //Create geometry string for creating GEOSpatial geography in SQL Server
            string geometryString = LineType + " (";
            foreach (dynamic point in trail.geometry.paths[0])
            {
                geometryString += (string)point[0] + " " + (string)point[1] + ",";
            }
            geometryString = geometryString.Remove(geometryString.Length - 1);
            geometryString += ")";

            //Build new trial section
            ts.Id = trail.attributes.FEATUREID;
            ts.ShortDescription = trail.attributes.TRLNAME;
            ts.Geography = DbGeography.FromText(geometryString, Convert.ToInt32(wkid));
            ts.Status = trail.attributes.TRLSTATUS;
            return ts;
        }

        public static string GeometryType(int pointcount)
        {
            if (pointcount > 1)
            {
                return "LineString";
            }
            else
            {
                return "Point";
            }

        }

    }

}
