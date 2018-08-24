﻿using MyTrails.Models;
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
            IList<TrailSection> Trails = new List<TrailSection>();
            //ICollection<TrailSection> Points = new List<TrailSection>();

            JObject t = JObject.Parse(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json")) as JObject;
            dynamic traildata = t;
            var wkid = traildata.spatialReference.wkid;

            foreach (dynamic trail in traildata.features)
            {
                string LineType = GeometryType(trail.geometry.paths[0].Count);
                //Check if this should be a point or line
                TrailSection ts = new TrailSection();
                string geometryString = LineType + " (";
                foreach (dynamic point in trail.geometry.paths[0])
                {
                    geometryString += (string)point[0] + " " + (string)point[1] + ",";
                }
                geometryString = geometryString.Remove(geometryString.Length - 1);
                geometryString += ")";

                ts.ShortDescription = trail.attributes.TRLNAME;
                ts.Geography = DbGeography.FromText(geometryString, (int)wkid);

                Trails.Add(ts);


               // Console.Read();
                
            }
            var sortedTrails = Trails.OrderByDescending(x => x.Geography.PointCount).ToList();
            var b = "";
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
