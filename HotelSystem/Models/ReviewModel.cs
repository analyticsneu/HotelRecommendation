using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelSystem.Models
{
    public class ReviewModel
    {
        [Display(Name = "Hotel ID")]
        public int hotelid { get; set; }

        public int reviewScore { get; set; }

        public string tempPic { get; set; }

        public string ratingCategory { get; set; }

        public rating reviewRating { get; set; }

        public enum rating {
            No,
            Excellent,
            VeryGood,
            Average,
            Poor,
            Terrible,
            NA
        }
    }

}