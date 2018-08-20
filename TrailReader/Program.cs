using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TrailReader
{
    class Program
    {
        static void Main(string[] args)
        {

                using (StreamReader r = new StreamReader(@"C:\Users\Michael\Desktop\Programming\Projects\OlympicTrailData.Json"))
                {
                    var trailJson = r.ReadToEnd();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    dynamic trail = js.Deserialize<dynamic>(trailJson);
                dynamic trailStore = new ExpandoObject();


                trailStore.fields = trail["fields"];


                for (int i = 0; i < trailStore.fields.Length; i++)
                {
                    
                }

                Console.Read();

            }


        }
    }
}
