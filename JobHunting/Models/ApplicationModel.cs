using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobHunting.Models
{
    public class ApplicationModel
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int JobSeekerId { get; set; }
        public string ApplicantName { get; set; }
        public string FatherName { get; set; }
        public string CNIC { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Experience { get; set; }
        public int RequiredSalary { get; set; }
        public string Massage { get; set; }
        public string LatestQualification { get; set; }
        public string CVurl { get; set; }
        public HttpPostedFileBase PDFFile { get; set; }
    }
}