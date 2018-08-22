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


