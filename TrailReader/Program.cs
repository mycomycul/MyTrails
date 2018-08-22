using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrailReader
{
    class Program
    {
        static void Main(string[] args)
        {

            //var t = JsonConvert.DeserializeObject(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"));
            var t = JObject.Parse(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"));
            IList<JToken> results = t["features"].Children().ToList();
            IList<Trail> trails = new List<Trail>();
            foreach (JToken item in results)
            {
                Trail trail = item["attributes"].ToObject<Trail>();
                trail.GEOMETRY = item["geometry"]["paths"][0];
                trails.Add(trail);
               // Console.Read();
            }
            //dynamic trailStore = new ExpandoObject();
            //Trail trail = (Trail)t.

            //var trailStore = new List<Trail>();
            //var trailFeatures = ((Array)(trail.fields));
            Console.Read();

            //for (int i = 0; i < trailFeatures.Length; i++)
            //{

            //    Console.Read();
            //}
            //foreach (var item in trail["features"])
            //{

            //    Trail newTrail = new Trail();
            //    //newTrail.Id = item["attributes"]["OBJECTID"];
            //    newTrail.TrailName = item["attributes"]["TRLNAME"];
            //    newTrail.Status = item["attributes"]["TRLSTATUS"];
            //    newTrail.Notes = item["attributes"]["NOTES"];

            //    trailStore.Add(newTrail);
            //}


            Console.Read();

        }


    }


    public class Trail
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


