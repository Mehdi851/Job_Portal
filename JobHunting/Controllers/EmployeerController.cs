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
    public class EmployeerController : Controller
    {
        // GET: Employeer
        #region Employeer Account
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(EmployerModel model)
        {
            using (var context = new JobHuntingDBEntities())
            {
                bool isValid = context.Employers.Any(x => x.Email == model.Email && x.Password == model.Password);
                if (isValid)
                {
                    var result = context.Employers.Where(x => x.Email == model.Email).Select(x => new EmployerModel()
                    {
                        Id = x.Id,
                        Username = x.Username,
                        Email = x.Email

                    }).FirstOrDefault();
                    Session["LogedinEmpUsername"] = result.Username;
                    Session["LogedinEmpID"] = result.Id;
                    Session["EmpLoginStatus"] = "true";


                    var employer = context.Employers.FirstOrDefault(x => x.Id == result.Id);
                    if (User != null)
                    {
                        employer.Status = "true";
                    }

                    context.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Massage = "Invalid Username and Password";
                    ModelState.AddModelError("", "Invalid Username and Password");
                    return View();
                }
            }
            
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(EmployerModel model)
        {
            Employer employer = new Employer();
            employer.Email = model.Email;
            employer.Username = model.Username;
            employer.Status = "False";
            employer.Password = model.Password;
            employer.CompanyName = model.CompanyName;
            employer.ActivationCode = Guid.NewGuid();
            using (var context = new JobHuntingDBEntities())
            {
                context.Employers.Add(employer);
                context.SaveChanges();
            }
            return RedirectToAction("Login");
        }

        public ActionResult Profile()
        {

            if (Session["EmpLoginStatus"] == "true")
            {
                int id = (int)Session["LogedinEmpID"];
                using (var db = new JobHuntingDBEntities())
                {
                    EmployerModel emp = db.Employers.Where(x => x.Id == id).Select(x => new EmployerModel { 
                    Id=x.Id,
                    Username=x.Username,
                    Email=x.Email,
                    CompanyName=x.CompanyName,
                    ProfilePicURL=x.ProfilePicURL
                    }).FirstOrDefault();
                    return View(emp);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        public ActionResult UpdateProfile(int id)
        {
            if (Session["EmpLoginStatus"] == "true")
            {
               
                using (var db = new JobHuntingDBEntities())
                {
                    EmployerModel emp = db.Employers.Where(x => x.Id == id).Select(x => new EmployerModel
                    {
                        Id = x.Id,
                        Username = x.Username,
                        Email = x.Email,
                        CompanyName = x.CompanyName,
                        ProfilePicURL = x.ProfilePicURL
                    }).FirstOrDefault();
                    return View(emp);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public ActionResult UpdateProfile(EmployerModel model)
        {
            if (Session["EmpLoginStatus"] == "true")
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
                

                using (var db = new JobHuntingDBEntities())
                {
                    var emp = db.Employers.Where(x => x.Id == model.Id).FirstOrDefault();
                    emp.Username = model.Username;
                    emp.Email = model.Email;
                    emp.CompanyName = model.CompanyName;
                    if (model.ProfilePicURL!=null)
                    {
                        emp.ProfilePicURL = model.ProfilePicURL;
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

        public ActionResult Logout()
        {

            if (Session["LogedinEmpID"] != null)
            {


                using (var context = new JobHuntingDBEntities())
                {

                    int id = (int)Session["LogedinEmpID"];
                    EmployerModel Emp = context.Employers.Where(x => x.Id == id).Select(x => new EmployerModel()
                    {
                        Id = x.Id,
                        Username = x.Username,
                        Email = x.Email

                    }).FirstOrDefault();
                    //Session["LogedinUsername"] = result.Username;
                    //Session["LogedinUserID"] = result.Id;

                    Employer EMp1 = context.Employers.FirstOrDefault(x => x.Id == Emp.Id);
                    if (EMp1 != null)
                    {
                        Emp.Status = "False";
                        Session["EmpLoginStatus"] = "False";
                    }

                    context.SaveChanges();

                }
            }

            return RedirectToAction("Index", "Home");



        }
        #endregion
        #region Jobs Management
        public ActionResult Index()
        {
            if (Session["EmpLoginStatus"] == "true")
            {
                int empId = (int)Session["LogedinEmpID"];
                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<JobsModel> jobs = db.Jobs.Where(x=>x.EmployerId==empId).Select(x => new JobsModel
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
                    return View(jobs);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

           
            
            
        }
        public ActionResult JobDetail(int id)
        {
            if (Session["EmpLoginStatus"] == "true")
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
        public ActionResult AddNewJob()
        {
            if (Session["EmpLoginStatus"] == "true")
            {
                dynamic mymodel = new ExpandoObject();
                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<JobCategoryModel> jobCategories = db.JobCategories.Select(x => new JobCategoryModel
                    {
                        Title = x.Title,
                        Id = x.Id
                    }).ToList();
                    if (jobCategories != null)
                    {
                        mymodel.Categories = jobCategories;
                    }
                }
                return View(mymodel);
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }
        [HttpPost]
        public ActionResult AddNewJob(JobsModel model)
        {
            if (Session["EmpLoginStatus"] == "true")
            {
                int empId = (int)Session["LogedinEmpID"];
                Job job = new Job();
                job.Title = model.Title;
                job.ShortDescription = model.ShortDescription;
                job.Salary = model.Salary;
                job.City = model.City;
                job.Category = model.Category;
                job.CompanyName = model.CompanyName;
                job.Postion = model.Postion;
                job.RequiredAge = model.RequiredAge;
                job.Gender = model.Gender;
                job.Requirements = model.Requirements;
                job.Type = model.Type;
                job.PostNo = model.PostNo;
                job.status = "Active";
                job.TimeofPost = DateTime.Now;
                job.EmployerId = empId;
                using (var context = new JobHuntingDBEntities())
                {
                    context.Jobs.Add(job);
                    context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login");
            }
           
        }
        public ActionResult UpdateJob(int id)
        {
            if (Session["EmpLoginStatus"] == "true")
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
                    IEnumerable<JobCategoryModel> jobCategories = db.JobCategories.Select(x => new JobCategoryModel
                    {
                        Title = x.Title,
                        Id = x.Id
                    }).ToList();
                    ViewBag.Categories = jobCategories;
                    return View(job);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
           
            
        }
        [HttpPost]
        public ActionResult UpdateJob(int id,JobsModel model)
        {
            if (Session["EmpLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    var job = db.Jobs.FirstOrDefault(x => x.Id == id);
                    if (job != null)
                    {
                        job.Title = model.Title;
                        job.ShortDescription = model.ShortDescription;
                        job.City = model.City;
                        job.Category = model.Category;
                        job.Salary = model.Salary;
                        job.CompanyName = model.CompanyName;
                        job.Postion = model.Postion;
                        job.RequiredAge = model.RequiredAge;
                        job.Gender = model.Gender;
                        job.Requirements = model.Requirements;
                        job.Type = model.Type;
                        job.PostNo = model.PostNo;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
           
          
        }
        public ActionResult Delete(int id)
        {
            if (Session["EmpLoginStatus"] == "true")
            {
                using (var db=new JobHuntingDBEntities())
                {
                    var job = db.Jobs.Where(x => x.Id == id).FirstOrDefault();
                    if (job!=null)
                    {
                        IEnumerable<Application> applications = db.Applications.Where(x => x.JobId == job.Id).ToList();
                        foreach (Application app in applications)
                        {
                            db.Applications.Remove(app);
                        }
                        db.Jobs.Remove(job);
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public ActionResult Applications(int id)
        {
            if (Session["EmpLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<ApplicationModel> apps = db.Applications.Where(x => x.JobId == id).Select(x => new ApplicationModel
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
                        LatestQualification = x.LatestQualification,
                        Massage = x.Massage,
                        CVurl = x.CVurl

                    }).ToList();
                    return View(apps);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }



        }
        #endregion
        #region Forgot Password
        [NonAction]
        public void sendVerificationLinkEmail(string Email, string activationCode)
        {
            //var schema = Request.Url.Scheme;
            //var host = Request.Url.Host;
            //var port = Request.Url.Port;

            //string url=schema+ "://"+
            var verifyUrl = "/Employeer/ResetPassword/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("Email", "Name");//At Email and Name you have add sender gmail and Name
            var toEmail = new MailAddress(Email);
            var fromEmailPasswor = "password";//You have to enter sender gmail password
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
                var account = db.Employers.Where(x => x.Email == Email).FirstOrDefault();
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
                var emp = db.Employers.Where(x => x.ResetPasswordCode == id).FirstOrDefault();
                if (emp != null)
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
                    var emp = db.Employers.Where(x => x.ResetPasswordCode == model.ResetCode).FirstOrDefault();
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