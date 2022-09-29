using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobHunting.Models
{
    public class AdminModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string ProfilePicURL { get; set; }
    }
}