using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobHunting.Models;

namespace JobHunting.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin\
        #region Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(AdminModel model)
        {
            using (var context = new JobHuntingDBEntities())
            {
                bool isValid = context.Admins.Any(x => x.Email == model.Email && x.Password == model.Password);
                if (isValid)
                {
                    var result = context.Admins.Where(x => x.Email == model.Email).Select(x => new AdminModel()
                    {
                        Id = x.Id,
                        Username = x.Username,
                        Email = x.Email

                    }).FirstOrDefault();
                    Session["LogedinUsername"] = result.Username;
                    Session["LogedinAdminID"] = result.Id;
                    Session["AdminLoginStatus"] = "true";


                    var User = context.Admins.FirstOrDefault(x => x.Id == result.Id);
                    if (result != null)
                    {
                        User.Status = "true";
                    }

                    context.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Username and Password");
                    return View();
                }
            }
        }
        public ActionResult Logout()
        {

            if (Session["LogedinAdminID"] != null)
            {


                using (var context = new JobHuntingDBEntities())
                {

                    int id = (int)Session["LogedinAdminID"];
                    AdminModel admin = context.Admins.Where(x => x.Id == id).Select(x => new AdminModel()
                    {
                        Id = x.Id,
                        Username = x.Username,
                        Email = x.Email

                    }).FirstOrDefault();
                    //Session["LogedinUsername"] = result.Username;
                    //Session["LogedinUserID"] = result.Id;

                    Admin Admin1 = context.Admins.FirstOrDefault(x => x.Id == admin.Id);
                    if (User != null)
                    {
                        Admin1.Status = "False";
                        Session["AdminLoginStatus"] = "False";
                    }

                    context.SaveChanges();

                }
            }

            return RedirectToAction("Login");


        }
        #endregion
        #region Jobs

        public ActionResult Index()//view jobs
        {
            if (Session["AdminLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<JobsModel> jobs = db.Jobs.Select(x => new JobsModel
                    {

                        Id = x.Id,
                        Title = x.Title,
                        ShortDescription = x.ShortDescription,
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
        public ActionResult JobDetail(int id)//view job detail
        {
            if (Session["AdminLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    JobsModel job = db.Jobs.Where(x => x.Id == id).Select(x => new JobsModel
                    {

                        Id = x.Id,
                        Title = x.Title,
                        City=x.City,
                        Category=x.Category,
                        ShortDescription = x.ShortDescription,
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

        public ActionResult JobDelete(int id)
        {
            if (Session["AdminLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    var job = db.Jobs.Where(x => x.Id == id).FirstOrDefault();
                    if (job != null)
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
        #endregion
        #region Categories
        public ActionResult Categories()
        {

            if (Session["AdminLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<JobCategoryModel> categories = db.JobCategories.Select(x => new JobCategoryModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        JobNumbers = x.JobNumbers

                    }).ToList();
                    return View(categories);
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
           
           
        }
        public ActionResult AddCategorie()
        {
            if (Session["AdminLoginStatus"] == "true")
            {

                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
           
        }
        [HttpPost]
        public ActionResult AddCategorie(JobCategoryModel model)
        {
            if (Session["AdminLoginStatus"] == "true")
            {
                JobCategory category = new JobCategory();
                category.Title = model.Title;
                category.JobNumbers = 0;
                using (var context = new JobHuntingDBEntities())
                {
                    context.JobCategories.Add(category);
                    context.SaveChanges();
                }
                return RedirectToAction("Categories");

            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }
        public ActionResult DeleteCategory(int id)
        {
            using (var db=new JobHuntingDBEntities())
            {
                var category = db.JobCategories.Where(x => x.Id == id).FirstOrDefault();
                db.JobCategories.Remove(category);
                db.SaveChanges();
                return RedirectToAction("Categories");
            }
           
        }
        #endregion
        #region JobSeekers
        public ActionResult JobSeekers()
        {
            if (Session["AdminLoginStatus"] == "true")
            {

                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<JobSeekerModel> jobSeekers = db.JobSeekers.Select(x => new JobSeekerModel
                    {

                        Id = x.Id,
                        Email = x.Email,
                        Username = x.Username,
                        Password = x.Password,
                        Status = x.Status,
                        ProfilePicURL = x.ProfilePicURL,
                        LatestQualification = x.LatestQualification

                    }).ToList();
                    return View(jobSeekers);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
            
            
        }
        public ActionResult JobSeekerDetail(int id)
        {
            if (Session["AdminLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    JobSeekerModel jobSeeker = db.JobSeekers.Where(x => x.Id == id).Select(x => new JobSeekerModel
                    {

                        Id = x.Id,
                        Email = x.Email,
                        Username = x.Username,
                        Password = x.Password,
                        Status = x.Status,
                        ProfilePicURL = x.ProfilePicURL,
                        LatestQualification = x.LatestQualification

                    }).FirstOrDefault();
                    return View(jobSeeker);
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
          
        }
        public ActionResult DeleteJobSeekers(int Id)
        {
            if (Session["AdminLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    var JS = db.JobSeekers.Where(x => x.Id == Id).FirstOrDefault();
                    db.JobSeekers.Remove(JS);
                    db.SaveChanges();
                    return RedirectToAction("JobSeekers");
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }
        #endregion
        #region Employer
        public ActionResult Employers()
        {
            if (Session["AdminLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<EmployerModel> employers = db.Employers.Select(x => new EmployerModel
                    {

                        Id = x.Id,
                        Email = x.Email,
                        Username = x.Username,
                        Password = x.Password,
                        Status = x.Status,
                        ProfilePicURL = x.ProfilePicURL,
                        CompanyName = x.CompanyName

                    }).ToList();
                    return View(employers);
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        
        }
        public ActionResult EmployerDetail(int id)
        {
            if (Session["AdminLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    EmployerModel employer = db.Employers.Where(x => x.Id == id).Select(x => new EmployerModel
                    {

                        Id = x.Id,
                        Email = x.Email,
                        Username = x.Username,
                        Password = x.Password,
                        Status = x.Status,
                        ProfilePicURL = x.ProfilePicURL,
                        CompanyName = x.CompanyName

                    }).FirstOrDefault();
                    return View(employer);
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
          
        }
        public ActionResult DeleteEmployer(int Id)
        {
            if (Session["AdminLoginStatus"] == "true")
            {
                using (var db = new JobHuntingDBEntities())
                {
                    var Emp = db.Employers.Where(x => x.Id == Id).FirstOrDefault();
                    db.Employers.Remove(Emp);
                    db.SaveChanges();
                    return RedirectToAction("Employers");
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
           
        }
        #endregion
        #region Applications
        public ActionResult Applications(int id)
        {
            if (Session["AdminLoginStatus"] == "true")
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
        public ActionResult ApplicationDetail(int id)
        {
            if (Session["AdminLoginStatus"] == "true")
            {

                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
           
        }
        #endregion
    }
}