using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelSystem.Models
{
      public class ResultViewModel
    {
        [Required]
        [Display(Name = "UserId")]
        [Range(6, 1198783, ErrorMessage = "Our userids belong to 6 to 1198783")]
        public int userid { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> UserIds { get; set; }

        public List<string> recommendations { get; set; }
    }
}
