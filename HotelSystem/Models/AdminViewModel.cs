using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelSystem.Models
{
    public class AdminViewModel:IValidatableObject
    {
        [Required(ErrorMessage = "Enter location city")]
        [Display(Name = "User Location City")]
        public int user_location_city { get; set; }

        [Required(ErrorMessage = "UserID??")]
        [Display(Name = "User ID")]
        public int user_id{ get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> UserIds { get; set; }

        [Display(Name = "Is Mobile")]
        public int is_mobile { get; set; }

        [Display(Name = "Is it Package")]
        public int is_package { get; set; }

        [Required(ErrorMessage = "At least one adult required")]
        [Range(1, 500, ErrorMessage = "We can take between 1 to 500 adults only")]
        [Display(Name = "Adults(18+)")]
        public int srch_adults_cnt { get; set; }

        [Required]
        [Range(0, 500, ErrorMessage = "We can take between 0 to 500 children only")]
        [Display(Name = "Children(0-17)")]
        public int srch_children_cnt { get; set; }

        [Required(ErrorMessage = "Select at least one room")]
        [Range(1, 5000, ErrorMessage = "Enter valid number of rooms")]
        [Display(Name = "Rooms")]
        public int srch_rm_cnt { get; set; }

        [Required(ErrorMessage = "You must enter Destination Id")]
        [Range(1, 500000, ErrorMessage = "Enter valid Destincation id")]
        [Display(Name = "Destination Id")]
        public int srch_destination_id { get; set; }


        [Display(Name = "Is Booking")]
        public int is_booking { get; set; }

        [Display(Name = "Count")]
        public int cnt { get; set; }

        [Display(Name = "Hotel Cluster")]
        public int hotel_cluster { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime bookingDate { get; set; }

        [Required]
        [Display(Name = "Check in Date")]
        [DataType(DataType.Date)]
        public DateTime checkinDate { get; set; }

        [Required]
        [Display(Name = "Check out Date")]
        [DataType(DataType.Date)]
        public DateTime checkoutDate { get; set; }

        [Display(Name = "Year")]
        public int year { get; set; }

        [Display(Name = "Month")]
        public int month { get; set; }

        [Display(Name = "Day")]
        public int day { get; set; }

        [Display(Name = "Hour")]
        public int hour { get; set; }

        [Display(Name = "Day of Week")]
        public int DayOfWeek { get; set; }

        [Display(Name = "Weekday")]
        public int Weekday { get; set; }

        [Display(Name = "Booking Window")]
        public string Booking_Window { get; set; }

        [Display(Name = "Length of stay")]
        public string Stay_Length { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (checkinDate < DateTime.Now) {
                yield return new ValidationResult("Check your dates. Can not book on early dates ");
            }
            if (checkoutDate < checkinDate)
            {
                yield return new ValidationResult("Checkout date should be after CheckIn date right! ");
            }
        }
    }
}