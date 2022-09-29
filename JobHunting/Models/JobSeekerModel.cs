using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobHunting.Models
{
    public class JobSeekerModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string ProfilePicURL { get; set; }
        public string LatestQualification { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public Nullable<System.Guid> ActivationCode { get; set; }
        public string ResetPasswordCode { get; set; }
    }
}