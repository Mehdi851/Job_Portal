using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobHunting.Models
{
    public class JobsModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string City { get; set; }
        public string Category { get; set; }
        public int Salary { get; set; }
        public string CompanyName { get; set; }
        public string Postion { get; set; }
        public string RequiredAge { get; set; }
        public string Gender { get; set; }
        public string Requirements { get; set; }
        public string Type { get; set; }
        public int PostNo { get; set; }
        public string status { get; set; }
        public System.DateTime TimeofPost { get; set; }
        public Nullable<int> EmployerId { get; set; }
    }
}