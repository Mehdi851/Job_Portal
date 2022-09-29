using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobHunting.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage ="New Password is Required",AllowEmptyStrings =false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword",ErrorMessage ="New Password and Confirm Password does not match")]
        public string ConfermPassword { get; set; }
        [Required]
        public string ResetCode { get; set; }
    }
}