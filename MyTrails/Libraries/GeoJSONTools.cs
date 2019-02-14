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
    public static class MyExtensions
    {

        /// <summary>
        /// Calls method ConvertSQLGeographyTextStringToArray() on current geography object
        /// </summary>
        /// <param name="geography"></param>
        /// <returns></returns>
        public static decimal[,] AsArray(this DbGeography geography)
        {
            return GeoJSONTools.ConvertSQLGeographyTextStringToArray(geography.AsText());
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A geo JSON tools. </summary>
    ///
    /// <remarks>   Michael, 2/10/2019. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class GeoJSONTools
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public void AddElevation()
        {

        }

        /// <summary>
        /// Removes text from a Sql LineString Geometry string and returns and Array of ordered coordinates
        /// </summary>
        /// <param name="spatial"></param>
        /// <returns></returns>
        public static decimal[,] ConvertSQLGeographyTextStringToArray(string spatial)
        {
            spatial = spatial.Replace("LINESTRING (", "");
            string[] separator = { ", " };
            spatial = spatial.Replace(")", "");
            string[] spatialArray;
            spatialArray = spatial.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            decimal[,] fullSpatialArray = new decimal[spatialArray.Length, 2];
            for (int i = 0; i < spatialArray.Length; i++)
            {
                string[] coordinates = spatialArray[i].Split(' ');
                fullSpatialArray[i, 0] = Convert.ToDecimal(coordinates[0]);
                fullSpatialArray[i, 1] = Convert.ToDecimal(coordinates[1]);

            }
            return fullSpatialArray;
        }

        public void InputGeoJson1(string filename)
        {

            dynamic trail = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(@"/App_data/OlympicTrailData.Json"));

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
        }

        /// <summary>
        /// Updates Trails in database with geospatial data Imported 
        /// from a geoJson
        /// </summary>
        public void ImportGeoJson2()
        {

            IList<TrailSection> otherTrails = new List<TrailSection>();


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
            }
            db.SaveChanges();

        }


        /// <summary>
        /// Takes a dynamic feature parsed from JSON along with a trailname and wkid and creates a TrailSection
        /// </summary>
        /// <param name="trailname"></param>
        /// <param name="trail"></param>
        /// <param name="wkid"></param>
        /// <returns></returns>
        public static TrailSection CreateTrailSection(string trailname, dynamic trail, string wkid)
        {
            //Check if this should be a point or line
            string LineType = GeometryType(trail.geometry.paths[0].Count);

            //Create geometry string for creating GEOSpatial geography in SQL Server
            string geometryString = LineType + " (";
            foreach (dynamic point in trail.geometry.paths[0])
            {
                geometryString += (string)point[0] + " " + (string)point[1] + ",";
            }
            //Remove last comma and replace with a closing parenthese
            geometryString = geometryString.Remove(geometryString.Length - 1);
            geometryString += ")";
            //Build new trial section
            TrailSection ts = new TrailSection()
            {
                Id = trail.attributes.FEATUREID,
                ShortDescription = trail.attributes.TRLNAME,
                Geography = DbGeography.FromText(geometryString, Convert.ToInt32(wkid)),
                Status = trail.attributes.TRLSTATUS
            };

            return ts;
        }


        /// <summary>
        /// Returns "LineString" if pointcount > 1 else returns "Point"
        /// </summary>
        /// <param name="pointcount"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Used for organizing data for returning to a map view
        /// Must be nested in a collection to be parsed on View
        /// </summary>
        public class SingleTrailSection
        {
            public decimal[,] Geometry { get; set; }
            public string Note { get; set; }

            public SingleTrailSection() : this(new decimal[0, 2], "")
            {
            }
            public SingleTrailSection(decimal[,] geometry) : this(geometry, "")
            {
            }
            public SingleTrailSection(decimal[,] geometry, string note)
            {
                Geometry = geometry;
                Note = note;
            }
        }

    }

}
