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

        /// <summary> Goes through a set of geoJSON features, looks for them in the database by trailname and adds geometry to db if found</summary>
        ///
        /// <remarks>   Michael, 2/17/2019. </remarks>
        ///
        /// <param name="geoJsonSource">Receives location of geoJson string</param>
        public void ImportGeoJsonDataToDB(string geoJsonSource = @"~/App_Data/OlympicTrailData.Json")
        {
            //For Loggin
            string unfoundTrailNames = "Trail not found in db: ";

            JObject t = JObject.Parse(File.ReadAllText(geoJsonSource)) as JObject;
            //Allow the use of dynamic json targeting
            dynamic traildata = t;
            var wkid = traildata.spatialReference.wkid;
            foreach (dynamic trail in traildata.features)
            {
                //Check if a matching trail name in the database exists and if so add current trail section to trail
                string trailName = trail.attributes.TRLNAME;                          
                if (db.Trails.Where(x => x.TrailName.ToUpper() == trailName).Any())
                {
                    
                    var ts = new TrailSection(trail, wkid);
                    var currentTrail = db.Trails.Where(x => x.TrailName.ToUpper() == trailName).First();
                    if (currentTrail.TrailSections.Count < 1)
                    {
                        currentTrail.Status = ts.Status;
                    }
                    currentTrail.TrailSections.Add(ts);
                }
                else
                {
                    unfoundTrailNames += (trailName + ": ");
                }
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Receives GeoJSON trail object and returns a Well-Known Text geometry string
        /// </summary>
        /// <remarks>Only returns points and LineStrings</remarks>
        /// <param name="trail">Individual Trail Feature with attributes extracted from GeoJSON </param>
        /// <returns>Well-Known-Text Geometry String</returns>
        public static string CreateWKT(dynamic trail)
        {
            string lineType = trail.geometry.paths[0].Count > 1 ? "LineString" : "Point";
            //Create geometry string for creating GEOSpatial geography in SQL Server
            string wKTString = lineType + " (";
            foreach (dynamic point in trail.geometry.paths[0])
            {
                wKTString += (string)point[0] + " " + (string)point[1] + ",";
            }
            //Remove last comma and replace with a closing parentheses
            wKTString = wKTString.Remove(wKTString.Length - 1);
            wKTString += ")";
            return wKTString;
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
