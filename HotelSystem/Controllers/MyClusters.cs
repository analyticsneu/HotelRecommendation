// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Microsoft.AspNet.WebApi.Client

using HotelSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HotelSystem.Controllers
{

    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    public class MyClusters
    {
        public static int isWeekday(DateTime dtToValidate)
        {
            if (dtToValidate.DayOfWeek == DayOfWeek.Sunday || dtToValidate.DayOfWeek == DayOfWeek.Saturday)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public static string InvokeRequestResponseService(AdminViewModel model)
        {
            model.is_booking = 0;
            model.is_mobile = 0;
            model.cnt = 1;
            model.bookingDate = DateTime.Now;
            model.year = model.bookingDate.Year;
            model.month = model.bookingDate.Month;
            model.day = model.bookingDate.Day;
            model.hour = model.bookingDate.Hour;
            model.Weekday = isWeekday(model.bookingDate);
            model.DayOfWeek = (int)model.bookingDate.DayOfWeek;
            model.Booking_Window = (int)(model.checkinDate - model.bookingDate).TotalDays + ".00:00:00";
            model.Stay_Length = (model.checkoutDate - model.checkinDate).TotalDays + ".00:00:00";

            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"user_location_city", "user_id", "is_mobile", "is_package", "srch_adults_cnt", "srch_children_cnt", "srch_rm_cnt", "srch_destination_id", "is_booking", "cnt", "year", "month", "day", "hour", "DayOfWeek", "Weekday", "Booking_Window", "Stay_Length"},
                                Values = new string[,] {  { model.user_location_city.ToString(), model.user_id.ToString(), model.is_mobile.ToString(),model.is_package.ToString(), model.srch_adults_cnt.ToString(),model.srch_children_cnt.ToString(),model.srch_rm_cnt.ToString(),model.srch_destination_id.ToString(),model.is_booking.ToString(), model.cnt.ToString(), model.year.ToString(), model.month.ToString(), model.day.ToString(), model.hour.ToString(), model.DayOfWeek.ToString(), model.Weekday.ToString(),model.Booking_Window.ToString(), model.Stay_Length.ToString()}  }
                            }
                        },
                    },

                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "d/jnA+4Z0W0TRIBRhbUaUIN6BeaRPZ09urfMx3/rWADjBajgBQfEa4mLTKJzAri0pvfr2ELMO4sSYgVHiuogvg=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/b78cb51f8e1941598ccc6b3f1aa4ffb5/services/b18195d3308544ac8988ed2b91e3a326/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = client.PostAsJsonAsync("", scoreRequest).Result;

                if (response.IsSuccessStatusCode)
                {
                    string jsonDoc = response.Content.ReadAsStringAsync().Result;
                    var ResponseBody = JsonConvert.DeserializeObject<RRSResponse>(jsonDoc);
                    return ResponseBody.Results.output1.value.Values[0][118] ;
                }
                else
                {
                    return "0";
                }
            }
        }
        #region Helper
        private class RRSResponse
        {
            public Results Results { get; set; }
        }

        private class Results
        {
            public Output1 output1 { get; set; }
        }

        private class Output1
        {
            public string type { get; set; }
            public Value value { get; set; }
        }

        private class Value
        {
            public string[] ColumnNames { get; set; }
            public string[] ColumnTypes { get; set; }
            public string[][] Values { get; set; }
        }
        #endregion
    }

}
