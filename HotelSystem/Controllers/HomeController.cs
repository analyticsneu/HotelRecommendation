using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelSystem.Models;
using System.IO;
using System.Data;
using Microsoft.AspNet.Identity;


namespace HotelSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public DataTable getAllHotels(string sheetName, string path)
        {
            string pth = Server.MapPath("~");
            DataTable dt = Helper.ReadExcelFile("UserIdHotel", pth + "/Content/UserIdHotel.csv");
            //dt = dt.AsEnumerable().Take(100).CopyToDataTable();
            return dt;
        }
        
        public FileStreamResult GetPDF()
        {
            String pth = Server.MapPath("~");
            FileStream fs = new FileStream(pth + "/Content/Report.pdf", FileMode.Open, FileAccess.Read);
            return File(fs, "application/pdf");
        }

        public ActionResult Report()
        {
            ViewBag.Message = "Team 3 Final Analysis and Report";
            return View();
        }
        public ActionResult Sample()
        {
            ViewBag.Message = "Explore Sample input for cluster prediction";
            DataTable dt = MyData.SampleInput();
            return View(dt);
        }
        public ActionResult Visualize()
        {
            ViewBag.Message = "Visualization of Hotel Data and business";
            return View();
        }

       
        [AllowAnonymous]
        public ActionResult Result()
        {
            ResultViewModel model = new ResultViewModel();
                model.userid = User.Identity.GetUserId<int>();
                RecommendationBatch.InvokeBatchExecutionService(model.userid.ToString());
                model.recommendations = Helper.readRecommendations(model.userid.ToString());
                return PartialView("Res",model);
            
            //int userId = User.Identity.GetUserId<int>();

        }
    
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Result(ResultViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //model.userid = User.Identity.GetUserId<int>();
            ViewBag.Message = "Your Recommendations";
            //int userId = User.Identity.GetUserId<int>();
            RecommendationBatch.InvokeBatchExecutionService(model.userid.ToString());
            model.recommendations = Helper.readRecommendations(model.userid.ToString());
            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Advances in Data Sciences Course - INFO 7390 - Final Project";

            return View();
        }

        public ActionResult Business()
        {
                       AdminViewModel mm = new AdminViewModel();
            //mm.UserIds = MyData.GetUserIds();
            ViewBag.Message = "Business page.";
            return View();
        }

     

        [HttpPost]
        public ActionResult Business(AdminViewModel model) {
            if (!ModelState.IsValid)
            {
                return View();
            }
            model.hotel_cluster=Convert.ToInt32(MyClusters.InvokeRequestResponseService(model));
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Review(int hotelid, string path)
        {
            ReviewModel model = new ReviewModel();
            model.hotelid = hotelid;
            model.tempPic = "~/Content/images/image_0"+path+".jpg";
            model.reviewScore = MyData.getHotelReviewScore(hotelid);
            ViewBag.Message = "Reviews";
            int score = model.reviewScore;
            if (score == 0) {
                model.reviewRating = ReviewModel.rating.No;
                model.ratingCategory = "No Review";
            }
            else if (score <= 202 && score > 100) {
                model.reviewRating = ReviewModel.rating.Excellent;
                model.ratingCategory = "Excellent";
            }
            else if (score <= 100 && score > 50) {
                model.reviewRating = ReviewModel.rating.VeryGood;
                model.ratingCategory = "Very Good";
            }
            else if (score <= 50 && score > 0) {
                model.reviewRating = ReviewModel.rating.Average;
                model.ratingCategory = "Average";
            }
            else if (score < 0 && score >= -10) {
                model.reviewRating = ReviewModel.rating.Terrible;
                model.ratingCategory = "Poor";
            }
            else if (score < -10 && score >= -55) {
                model.reviewRating = ReviewModel.rating.Terrible;
                model.ratingCategory = "Terrible";
            }
            else { 
                model.ratingCategory = "N.A.";
                model.reviewRating = ReviewModel.rating.NA;
            }

            return PartialView(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult Review2(int input)
        {
            ReviewModel model = new ReviewModel();
            model.hotelid = input;
            model.reviewScore = MyData.getHotelReviewScore(input);
            ViewBag.Message = "Reviews";
            int score = model.reviewScore;
            if (score == 0)
            {
                model.reviewRating = ReviewModel.rating.No;
                model.ratingCategory = "No Review";
            }
            else if (score <= 202 && score > 100)
            {
                model.reviewRating = ReviewModel.rating.Excellent;
                model.ratingCategory = "Excellent";
            }
            else if (score <= 100 && score > 50)
            {
                model.reviewRating = ReviewModel.rating.VeryGood;
                model.ratingCategory = "Very Good";
            }
            else if (score <= 50 && score > 0)
            {
                model.reviewRating = ReviewModel.rating.Average;
                model.ratingCategory = "Average";
            }
            else if (score < 0 && score >= -10)
            {
                model.reviewRating = ReviewModel.rating.Terrible;
                model.ratingCategory = "Poor";
            }
            else if (score < -10 && score >= -55)
            {
                model.reviewRating = ReviewModel.rating.Terrible;
                model.ratingCategory = "Terrible";
            }
            else
            {
                model.ratingCategory = "N.A.";
                model.reviewRating = ReviewModel.rating.NA;
            }

            return Json(model.reviewRating.ToString(), JsonRequestBehavior.AllowGet);
        }

    }
}