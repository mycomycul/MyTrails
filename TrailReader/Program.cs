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
using MyTrails.Models;

namespace TrailReader
{
    class Program
    {
        static void Main(string[] args)
        {
            //var t = JObject.Parse(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"));
            //IList<JToken> results = t["features"].Children().ToList();
            ////var t = JsonConvert.DeserializeObject(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\
            //IList<TrailModel> trails = new List<TrailModel>();
            //foreach (JToken item in results)
            //{
            //    TrailModel trail = item["attributes"].ToObject<TrailModel>();
            //    trail.GEOMETRY = item["geometry"]["paths"][0];
            //    trails.Add(trail);
            //   // Console.Read();
            //}

            //var t = JsonConvert.DeserializeObject(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"));
            JObject t = JObject.Parse(File.ReadAllText(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json")) as JObject;
            dynamic traildata = t;
            foreach (dynamic trail in traildata.features)
            {
                Console.WriteLine(trail.attributes.TRLNAME);
            }
            var bob = "";
            Console.Read();

        }


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


