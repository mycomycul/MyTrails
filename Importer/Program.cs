using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace Importer
{
    class Program
    {
        private void button1_Click(object sender, EventArgs e)
        {
            ApplicationdbCOntext 
            string connetionString = null;
            SqlConnection connection;
            SqlCommand command;
            SqlDataAdapter adpter = new SqlDataAdapter();
            DataSet ds = new DataSet();
            XmlReader xmlFile;
            string sql = null;

            int product_ID = 0;
            string Product_Name = null;
            double product_Price = 0;

            connetionString = "Data Source=servername;Initial Catalog=databsename;User ID=username;Password=password";

            connection = new SqlConnection(connetionString);

            xmlFile = XmlReader.Create("Product.xml", new XmlReaderSettings());
            ds.ReadXml(xmlFile);
            int i = 0;
            connection.Open();
            for (i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
            {
                product_ID = Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[0]);
                Product_Name = ds.Tables[0].Rows[i].ItemArray[1].ToString();
                product_Price = Convert.ToDouble(ds.Tables[0].Rows[i].ItemArray[2]);
                sql = "insert into Product values(" + product_ID + ",'" + Product_Name + "'," + product_Price + ")";
                command = new SqlCommand(sql, connection);
                adpter.InsertCommand = command;
                adpter.InsertCommand.ExecuteNonQuery();
            }
            connection.Close();
            MessageBox.Show("Done .. ");
        }




        /*public void Importer()
        {
            string file = @"C:\Users\Michael\Desktop\TestSheet.xls";
            Console.WriteLine(file);

            Excel.Application excel = null;
            Excel.Workbook wkb = null;

            try
            {
                excel = new Excel.Application();

                wkb = excelTools.OfficeUtil.OpenBook(excel, file);

                Excel.Worksheet sheet = wkb.Sheets["Data"] as Excel.Worksheet;

                Excel.Range range = null;

                if (sheet != null)
                    range = sheet.get_Range("A1", Missing.Value);

                string A1 = String.Empty;

                if (range != null)
                    A1 = range.Text.ToString();

                Console.WriteLine("A1 value: {0}", A1);

            }
            catch (Exception ex)
            {
                //if you need to handle stuff
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (wkb != null)
                    ExcelTools.OfficeUtil.ReleaseRCM(wkb);

                if (excel != null)
                    ExcelTools.OfficeUtil.ReleaseRCM(excel);
            }
        }*/
    }
}
    

