using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelSystem.Controllers
{
    public static class MyData
    {
        public static DataTable users { get; set; }
        public static DataTable hotelReview { get; set; }
        public static DataTable sample { get; set; }

        public static DataTable SampleInput()
        {
            string pth = HttpContext.Current.Server.MapPath("~");
            DataTable dt = Helper.ReadExcelFile("UseCaseData", pth + "/Content/UseCaseData.csv");
            return dt;
        }

        public static DataTable getAllUsers()
        {
            string pth = HttpContext.Current.Server.MapPath("~");
            DataTable dt = Helper.ReadExcelFile("UserIdHotel", pth + "/Content/UserIdHotel.csv");
            dt = dt.AsEnumerable().Take(100).CopyToDataTable();
            return dt;
        }
        public static int getHotelReviewScore(int hotelid)
        {
            DataTable dt = getHotels();
            string expression;
            expression = "prop_id = " + hotelid;
            DataRow[] foundRows;
            // Use the Select method to find all rows matching the filter.
            foundRows = dt.Select(expression);
            if (foundRows.Length == 0)
                return 0;
            else
            {
                return (int)foundRows[0].ItemArray[1];
            }
        }
        public static IEnumerable<SelectListItem> getHotelListWithout0ForDemo()
        {
            List<string> retList = new List<string>();
            foreach (DataRow r in hotelReview.Rows)
            {
                int scr = (int)r.ItemArray[1];
                if (scr != 0)
                {
                    retList.Add(r.ItemArray[0].ToString());
                }
            }
            IEnumerable<SelectListItem> myCollection = retList
                                           .Select(i => new SelectListItem()
                                           {
                                               Text = "Hotel " + i.ToString(),
                                               Value = i
                                           }).OrderBy(a => a.Text);

            return myCollection;
        }
        public static IEnumerable<SelectListItem> getHotelList()
        {
            List<string> retList = new List<string>();
            foreach (DataRow r in hotelReview.Rows)
            {
                retList.Add(r.ItemArray[0].ToString());

            }
            IEnumerable<SelectListItem> myCollection = retList
                                           .Select(i => new SelectListItem()
                                           {
                                               Text = "Hotel " + i.ToString(),
                                               Value = i
                                           }).OrderBy(a => a.Text); ;
            return myCollection;
        }

        public static DataTable getHotels()
        {
            if (hotelReview == null)
            {
                string pth = HttpContext.Current.Server.MapPath("~");
                hotelReview = Helper.ReadExcelFile("HotelIDAndReviews", pth + "/Content/HotelIDAndReviews.csv");
            }
            return hotelReview;
        }
        public static IEnumerable<SelectListItem> GetUserIds()
        {
            if ((users == null))
                users = getAllUsers();
            List<string> retList = new List<string>();
            foreach (DataRow r in users.Rows)
            {
                retList.Add(r.ItemArray[0].ToString());

            }
            IEnumerable<SelectListItem> myCollection = retList
                                           .Select(i => new SelectListItem()
                                           {
                                               Text = i.ToString(),
                                               Value = i
                                           }).OrderBy(a => a.Text); ;
            return myCollection;
        }

    }
}