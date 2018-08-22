using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace TrailReader
{
    class Program
    {
        static void Main(string[] args)
        {

            using (StreamReader r = new StreamReader(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"))
            {
                var trailJson = r.ReadToEnd();
                /* read JSON with using System.Web.Script.Serialization;*/
                //   JavaScriptSerializer js = new JavaScriptSerializer();
                //    dynamic trail = js.Deserialize<dynamic>(trailJson);
                //dynamic trailStore = new ExpandoObject();


                //    trailStore.fields = new List<string>();
                //    for (int i = 0; i < trail["fields"].Length; i++)
                //    {
                //        trailStore.fields.Add(trail["fields"][i]["name"]);
                //    }

                //    foreach (var item in trail["features"])
                //    {
                //        var t = item["attributes"];
                //        foreach (var attribute in t)
                //        {
                //            var k = attribute.Key;
                //            var z = attribute.Value;
                //            Console.WriteLine(k + " = " + z);
                //        }
                //    }

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

                        Trail newTrail = new Trail();
                        newTrail.Id = item["attributes"]["OBJECTID"];
                        newTrail.TrailName = item["attributes"]["TRLNAME"];
                        newTrail.Status = item["attributes"]["TRLSTATUS"];
                        newTrail.Notes = item["attributes"]["NOTES"];

                        trailStore.Add(newTrail);
                    }
                






                Console.Read();

            }


        }


        public class Trail
        {
            public int Id { get; set; }
            public string TrailName { get; set; }
            public string Status { get; set; }
            public string Notes { get; set; }

        }
    }
}

