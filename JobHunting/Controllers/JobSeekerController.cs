using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using JobHunting.Models;

namespace JobHunting.Controllers
{
    public class JobSeekerController : Controller
    {
        // GET: JobSeeker

        #region JobSeeker Account
        public ActionResult Login()
        {
            //  var priviousPageUrl = System.Web.HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
            //ViewBag.Privious = Request.UrlReferrer.ToString();
            return View();
        }
        [HttpPost]
        public ActionResult Login(JobSeekerModel model)
        {
            using (var context = new JobHuntingDBEntities())
            {
                string privious = Request["Privious"];
                bool isValid = context.JobSeekers.Any(x => x.Email == model.Email && x.Password == model.Password);
                if (isValid)
                {
                    var result = context.JobSeekers.Where(x => x.Email == model.Email).Select(x => new JobSeekerModel()
                    {
                        Id = x.Id,
                        Username = x.Username,
                        Email = x.Email

                    }).FirstOrDefault();
                    Session["LogedinJSUsername"] = result.Username;
                    Session["LogedinJSID"] = result.Id;
                    Session["JSLoginStatus"] = "true";


                    var employer = context.JobSeekers.FirstOrDefault(x => x.Id == result.Id);
                    if (User != null)
                    {
                        employer.Status = "true";
                    }

                    context.SaveChanges();


                    return RedirectToAction("Home");
                    //return Redirect(Request.UrlReferrer.ToString());
                }
                else
                {
                    ViewBag.Massage = "Invalid Username and Password";
                    ModelState.AddModelError("", "Invalid Username and Password");
                    return View();
                }
            }

        }

        public ActionResult Profile()
        {
            if (Session["JSLoginStatus"] == "true")
            {
                int jobseekerId = (int)Session["LogedinJSID"];
                dynamic mymodel = new ExpandoObject();
                using (var db = new JobHuntingDBEntities())
                {
                    JobSeekerModel JS = db.JobSeekers.Where(x => x.Id == jobseekerId).Select(x => new JobSeekerModel { 
                        Id=x.Id,
                        Email=x.Email,
                        LatestQualification=x.LatestQualification,
                        ProfilePicURL=x.ProfilePicURL,
                        Username=x.Username
                    
                    }).FirstOrDefault();
                    ViewBag.Profile = JS;
                    return View();
                }
                
            }
            else
            {
                return RedirectToAction("Login");
            }

           
        }
        public ActionResult UpdateProfile(int id)
        {
            if (Session["JSLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    JobSeekerModel JS = db.JobSeekers.Where(x => x.Id == id).Select(x => new JobSeekerModel
                    {
                        Id = x.Id,
                        Email = x.Email,
                        LatestQualification = x.LatestQualification,
                        ProfilePicURL = x.ProfilePicURL,
                        Username = x.Username

                    }).FirstOrDefault();
                    return View(JS);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

         
            
        }
        [HttpPost]
        public ActionResult UpdateProfile(JobSeekerModel model)
        {
            if (Session["JSLoginStatus"] == "true")
            {
                if (model.ImageFile!=null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    string extension = Path.GetExtension(model.ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    model.ProfilePicURL = "~/Images/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    model.ImageFile.SaveAs(fileName);
                }
               
              
                using (var db=new JobHuntingDBEntities())
                {
                    var js =db.JobSeekers.Where(x=>x.Id==model.Id).FirstOrDefault();
                    js.Username = model.Username;
                    js.Email = model.Email;
                    js.LatestQualification = model.LatestQualification;
                    if (model.ProfilePicURL!=null)
                    {
                        js.ProfilePicURL = model.ProfilePicURL;

                    }

                    db.SaveChanges();
                }
                return RedirectToAction("Profile");
            }
            else
            {
                return RedirectToAction("Login");
            }


        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(JobSeekerModel model)
        {
            JobSeeker jobSeeker = new JobSeeker();
            jobSeeker.Email = model.Email;
            jobSeeker.Username = model.Username;
            jobSeeker.Status = "False";
            jobSeeker.Password = model.Password;
            jobSeeker.LatestQualification = model.LatestQualification;
            jobSeeker.ActivationCode = Guid.NewGuid();
            using (var context = new JobHuntingDBEntities())
            {
                context.JobSeekers.Add(jobSeeker);
                context.SaveChanges();
            }
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {

            if (Session["LogedinJSID"] != null)
            {


                using (var context = new JobHuntingDBEntities())
                {

                    int id = (int)Session["LogedinJSID"];
                    JobSeekerModel JS = context.JobSeekers.Where(x => x.Id == id).Select(x => new JobSeekerModel()
                    {
                        Id = x.Id,
                        Username = x.Username,
                        Email = x.Email

                    }).FirstOrDefault();
                    //Session["LogedinUsername"] = result.Username;
                    //Session["LogedinUserID"] = result.Id;

                    JobSeeker JS1 = context.JobSeekers.FirstOrDefault(x => x.Id == JS.Id);
                    if (JS1 != null)
                    {
                        JS1.Status = "False";
                        Session["JSLoginStatus"] = "False";
                    }

                    context.SaveChanges();

                }
            }

            return RedirectToAction("Index","Home");


        }
        #endregion
        #region Home Page
        public ActionResult Home()
        {
            if (Session["JSLoginStatus"] == "true")
            {
                dynamic mymodel = new ExpandoObject();
                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<JobsModel> jobs = db.Jobs.OrderByDescending(x => x.TimeofPost).Select(x => new JobsModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        ShortDescription = x.ShortDescription,
                        City = x.City,
                        Category = x.Category,
                        Salary = x.Salary,
                        CompanyName = x.CompanyName,
                        Postion = x.Postion,
                        RequiredAge = x.RequiredAge,
                        Gender = x.Gender,
                        Requirements = x.Requirements,
                        Type = x.Type,
                        PostNo = x.PostNo,
                        status = x.status,
                        TimeofPost = x.TimeofPost
                    }).ToList();
                    mymodel.Jobs = jobs;
                    IEnumerable<JobsModel> newjobs = db.Jobs.OrderByDescending(x => x.TimeofPost).Select(x => new JobsModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        ShortDescription = x.ShortDescription,
                        City = x.City,
                        Category = x.Category,
                        Salary = x.Salary,
                        CompanyName = x.CompanyName,
                        Postion = x.Postion,
                        RequiredAge = x.RequiredAge,
                        Gender = x.Gender,
                        Requirements = x.Requirements,
                        Type = x.Type,
                        PostNo = x.PostNo,
                        status = x.status,
                        TimeofPost = x.TimeofPost
                    }).ToList();
                    mymodel.NewJobs = newjobs;
                    IEnumerable<JobCategoryModel> jobCategories = db.JobCategories.Select(x => new JobCategoryModel
                    {
                        Title = x.Title,
                        Id = x.Id
                    }).ToList();
                    if (jobCategories != null)
                    {
                        mymodel.Categories = jobCategories;
                    }

                    return View(mymodel);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

           

        }

        public ActionResult JobSearch()
        {
            string title = Request["Title"];
            string city = Request["City"];
            if (title != "" && city != "")
            {
                dynamic mymodel = new ExpandoObject();
                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<JobsModel> jobs = db.Jobs.Where(x => x.Title.Equals(title) && x.City.Equals(city)).Select(x => new JobsModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        ShortDescription = x.ShortDescription,
                        City = x.City,
                        Category = x.Category,
                        Salary = x.Salary,
                        CompanyName = x.CompanyName,
                        Postion = x.Postion,
                        RequiredAge = x.RequiredAge,
                        Gender = x.Gender,
                        Requirements = x.Requirements,
                        Type = x.Type,
                        PostNo = x.PostNo,
                        status = x.status,
                        TimeofPost = x.TimeofPost
                    }).ToList();
                    mymodel.Jobs = jobs;
                    IEnumerable<JobCategoryModel> jobCategories = db.JobCategories.Select(x => new JobCategoryModel
                    {
                        Title = x.Title,
                        Id = x.Id
                    }).ToList();
                    if (jobCategories != null)
                    {
                        mymodel.Categories = jobCategories;
                    }

                    return View(mymodel);
                }

            }
            if (title != "" && city == "")
            {
                dynamic mymodel = new ExpandoObject();
                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<JobsModel> jobs = db.Jobs.Where(x => x.Title.Equals(title)).Select(x => new JobsModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        ShortDescription = x.ShortDescription,
                        City = x.City,
                        Category = x.Category,
                        Salary = x.Salary,
                        CompanyName = x.CompanyName,
                        Postion = x.Postion,
                        RequiredAge = x.RequiredAge,
                        Gender = x.Gender,
                        Requirements = x.Requirements,
                        Type = x.Type,
                        PostNo = x.PostNo,
                        status = x.status,
                        TimeofPost = x.TimeofPost
                    }).ToList();
                    mymodel.Jobs = jobs;
                    IEnumerable<JobCategoryModel> jobCategories = db.JobCategories.Select(x => new JobCategoryModel
                    {
                        Title = x.Title,
                        Id = x.Id
                    }).ToList();
                    if (jobCategories != null)
                    {
                        mymodel.Categories = jobCategories;
                    }

                    return View(mymodel);
                }
            }
            else
            {
                dynamic mymodel = new ExpandoObject();
                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<JobsModel> jobs = db.Jobs.Where(x => x.City.Equals(city)).Select(x => new JobsModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        ShortDescription = x.ShortDescription,
                        City = x.City,
                        Category = x.Category,
                        Salary = x.Salary,
                        CompanyName = x.CompanyName,
                        Postion = x.Postion,
                        RequiredAge = x.RequiredAge,
                        Gender = x.Gender,
                        Requirements = x.Requirements,
                        Type = x.Type,
                        PostNo = x.PostNo,
                        status = x.status,
                        TimeofPost = x.TimeofPost
                    }).ToList();
                    mymodel.Jobs = jobs;
                    IEnumerable<JobCategoryModel> jobCategories = db.JobCategories.Select(x => new JobCategoryModel
                    {
                        Title = x.Title,
                        Id = x.Id
                    }).ToList();
                    if (jobCategories != null)
                    {
                        mymodel.Categories = jobCategories;
                    }

                    return View(mymodel);
                }
            }


        }

        public ActionResult Categories(int id)
        {

            if (Session["JSLoginStatus"] == "true")
            {

                string cate = null;
                dynamic mymodel = new ExpandoObject();
                using (var db = new JobHuntingDBEntities())
                {
                    JobCategoryModel category = db.JobCategories.Where(x => x.Id == id).Select(x => new JobCategoryModel
                    {
                        Title = x.Title,
                        Id = x.Id
                    }).FirstOrDefault();
                    cate = category.Title;
                    IEnumerable<JobsModel> jobs = db.Jobs.Where(x => x.Category.Equals(cate)).Select(x => new JobsModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        ShortDescription = x.ShortDescription,
                        City = x.City,
                        Category = x.Category,
                        Salary = x.Salary,
                        CompanyName = x.CompanyName,
                        Postion = x.Postion,
                        RequiredAge = x.RequiredAge,
                        Gender = x.Gender,
                        Requirements = x.Requirements,
                        Type = x.Type,
                        PostNo = x.PostNo,
                        status = x.status,
                        TimeofPost = x.TimeofPost
                    }).ToList();
                    if (jobs != null)
                    {
                        mymodel.Jobs = jobs;
                    }

                    IEnumerable<JobCategoryModel> jobCategories = db.JobCategories.Select(x => new JobCategoryModel
                    {
                        Title = x.Title,
                        Id = x.Id
                    }).ToList();
                    if (jobCategories != null)
                    {
                        mymodel.Categories = jobCategories;
                    }

                    return View(mymodel);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult JobDetail(int id)
        {
            if (Session["JSLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    JobsModel job = db.Jobs.Where(x => x.Id == id).Select(x => new JobsModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        ShortDescription = x.ShortDescription,
                        City = x.City,
                        Category = x.Category,
                        Salary = x.Salary,
                        CompanyName = x.CompanyName,
                        Postion = x.Postion,
                        RequiredAge = x.RequiredAge,
                        Gender = x.Gender,
                        Requirements = x.Requirements,
                        Type = x.Type,
                        PostNo = x.PostNo,
                        status = x.status,
                        TimeofPost = x.TimeofPost
                    }).FirstOrDefault();
                    return View(job);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
          

        }
        #endregion
        #region Jobs
        public ActionResult Index()
        {
            if (Session["JSLoginStatus"] == "true")
            {
                int jobseekerId = (int)Session["LogedinJSID"];
                dynamic mymodel = new ExpandoObject();
                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<ApplicationModel> apps = db.Applications.Where(x => x.JobSeekerId == jobseekerId).Select(x => new ApplicationModel
                    {
                        Id = x.Id,
                        JobId = x.JobId,
                        JobSeekerId = x.JobSeekerId,
                        ApplicantName = x.ApplicantName,
                        FatherName = x.FatherName,
                        CNIC = x.CNIC,
                        City = x.City,
                        Country = x.Country,
                        Address = x.Address,
                        Experience = x.Experience,
                        RequiredSalary = x.RequiredSalary,
                        Massage = x.Massage,
                        CVurl = x.CVurl

                    }).ToList();
                    IEnumerable<JobsModel> jobs = db.Jobs.Select(x => new JobsModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        ShortDescription = x.ShortDescription,
                        City = x.City,
                        Category = x.Category,
                        Salary = x.Salary,
                        CompanyName = x.CompanyName,
                        Postion = x.Postion,
                        RequiredAge = x.RequiredAge,
                        Gender = x.Gender,
                        Requirements = x.Requirements,
                        Type = x.Type,
                        PostNo = x.PostNo,
                        status = x.status,
                        TimeofPost = x.TimeofPost
                    }).ToList();
                    mymodel.Applications = apps;
                    mymodel.Jobs = jobs;
                }
                return View(mymodel);
            }
            else
            {
                return RedirectToAction("Login");
            }



        }
        public ActionResult JobApply(int id)
        {
            if (Session["JSLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    JobsModel job = db.Jobs.Where(x => x.Id == id).Select(x => new JobsModel
                    {
                        Id = x.Id,
                        Title = x.Title
                        //ShortDescription = x.ShortDescription,
                        //City = x.City,
                        //Category = x.Category,
                        //Salary = x.Salary,
                        //CompanyName = x.CompanyName,
                        //Postion = x.Postion,
                        //RequiredAge = x.RequiredAge,
                        //Gender = x.Gender,
                        //Requirements = x.Requirements,
                        //Type = x.Type,
                        //PostNo = x.PostNo,
                        //status = x.status,
                        //TimeofPost = x.TimeofPost
                    }).FirstOrDefault();
                    ViewBag.Job = job;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult JobApply(ApplicationModel model)
        {
            if (Session["JSLoginStatus"] == "true")
            {
                string JobName = Request["JobName"].ToString();
                int jobseekerId = (int)Session["LogedinJSID"];
                string fileName = Path.GetFileNameWithoutExtension(model.PDFFile.FileName);
                string extension = Path.GetExtension(model.PDFFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                model.CVurl = "~/CVs/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/CVs/"), fileName);
                model.PDFFile.SaveAs(fileName);
                Application app = new Application();
                app.JobSeekerId = jobseekerId;
                app.ApplicantName = model.ApplicantName;
                app.FatherName = model.FatherName;
                app.CNIC = model.CNIC;
                app.City = model.City;
                app.Country = model.Country;
                app.Address = model.Address;
                app.Experience = model.Experience;
                app.RequiredSalary = model.RequiredSalary;
                app.Massage = model.Massage;
                app.LatestQualification = model.LatestQualification;
                app.CVurl = model.CVurl;
                using (var context = new JobHuntingDBEntities())
                {
                    JobsModel job = context.Jobs.Where(x => x.Title == JobName).Select(x => new JobsModel
                    {
                        Id = x.Id,


                    }).FirstOrDefault();

                    app.JobId = job.Id;

                    context.Applications.Add(app);
                    context.SaveChanges();
                }
                return RedirectToAction("Index");

            }
            else
            {
                return RedirectToAction("Login");
            }


        }

       
        #endregion
        #region Forgot Password region
        [NonAction]
        public void sendVerificationLinkEmail(string Email, string activationCode)
        {
            var verifyUrl = "/JobSeeker/ResetPassword/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("www.muhammadjawadmehdi22@gmail.com", "Jawad Mehdi");
            var toEmail = new MailAddress(Email);
            var fromEmailPasswor = "BSSE(0851)j";
            string subject = "Reset Password";
            string body = "Hi, <br/><br/> We got request for your acount password. Please Click on below link to reset your password" +
                "<br/><br/><a href=" + link + ">Reset password</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPasswor)
            };

            using (var massage = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(massage);

        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            //verify email
            //Generate Reset Password Link
            //Send Email
            string massage = "";
            bool status = false;
            using (var db = new JobHuntingDBEntities())
            {
                var account = db.JobSeekers.Where(x => x.Email == Email).FirstOrDefault();
                if (account != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    sendVerificationLinkEmail(account.Email, resetCode);
                    account.ResetPasswordCode = resetCode;

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();

                    massage = "Reset Password Link has been send to your email address";
                }
                else
                {
                    massage = "Acount Not Found";
                }
            }
            ViewBag.Message = massage;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            //vrify the reset pasword link
            //find account associated with this link
            //redirct to reset password page
            using (var db = new JobHuntingDBEntities())
            {
                var js = db.JobSeekers.Where(x => x.ResetPasswordCode == id).FirstOrDefault();
                if (js != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);

                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var massage = "";
            if (ModelState.IsValid)
            {
                using (var db = new JobHuntingDBEntities())
                {
                    var emp = db.JobSeekers.Where(x => x.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (emp != null)
                    {
                        emp.Password = model.NewPassword;
                        emp.ResetPasswordCode = "";
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        massage = "New Password is updated Successfully";
                    }
                }
            }
            else
            {
                massage = "Something Invalid";
            }
            ViewBag.Message = massage;
            return View(model);

        }

        #endregion

    }
}