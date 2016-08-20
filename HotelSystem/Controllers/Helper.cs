using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelSystem.Controllers
{
    public class Helper
    {
        public static Boolean writefile(string userid)
        {
            String pth = System.Web.HttpContext.Current.Server.MapPath("~");
            pth += "/Content/";
            StreamWriter sw = new StreamWriter(pth + "SingleUser" + userid + ".csv");
            int RowCount = 2;
            int ColumnCount = 1;
            string[][] vals = new string[RowCount][];
            vals[0] = new string[1] { "user_id" };//name of the values
            vals[1] = new string[1] { userid };
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    sw.Write(vals[i][j]);//write one row separated by columns
                }
                if (i < RowCount)
                {
                    sw.Write("\n");//use \n to separate between rows
                }
            }
            sw.Close();
            return true;
        }

        public static DataTable ReadExcelFile(string sheetName, string path)
        {

            using (OleDbConnection conn = new OleDbConnection())
            {
                DataTable dt = new DataTable();
                string Import_FileName = path;
                string fileExtension = System.IO.Path.GetExtension(Import_FileName);
                var connString = string.Format(
                     @"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};Extended Properties=""Text;HDR=YES;FMT=Delimited""",
                     Path.GetDirectoryName(Import_FileName)
                 );
                conn.ConnectionString = connString;
                using (OleDbCommand comm = new OleDbCommand())
                {
                    comm.CommandText = "Select * from [" + sheetName + ".csv" + "]";

                    comm.Connection = conn;

                    using (OleDbDataAdapter da = new OleDbDataAdapter())
                    {
                        da.SelectCommand = comm;
                        da.Fill(dt);
                        return dt;
                    }

                }
            }
        }

        public static List<string> readRecommendations(string userid) {
            var retList = new List<string>();
            try
            {
                String pth = System.Web.HttpContext.Current.Server.MapPath("~");
                pth += "/Content/" + "output1results" + userid + ".csv";
                string CSVFilePathName = pth;
                string[] Lines = File.ReadAllLines(CSVFilePathName);
                string[] Fields;
                Fields = Lines[0].Split(new char[] { ',' });
                int Cols = Fields.GetLength(0);
                Fields = Lines[1].Split(new char[] { ',' });

                for (int f = 1; f < Cols; f++)
                    retList.Add(Fields[f]);
            }
            catch (Exception e) { }
            // foreach (string inaddIP)
            //{
            //  string returnIP = s;
            // retList.Add(returnIP);
            // }
            return retList;
        }
        
        }
    }

