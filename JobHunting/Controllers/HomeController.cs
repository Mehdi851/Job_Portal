using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobHunting.Models;

namespace JobHunting.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            dynamic mymodel = new ExpandoObject();
            using (var db=new JobHuntingDBEntities())
            {
                IEnumerable<JobsModel> jobs = db.Jobs.OrderByDescending(x=>x.TimeofPost).Select(x => new JobsModel {
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

        public ActionResult JobSearch()
        {
            string title = Request["Title"];
            string city = Request["City"];
            if (title != "" && city != "" && title!=null&& city!=null)
            {
                dynamic mymodel = new ExpandoObject();
                using (var db = new JobHuntingDBEntities())
                {
                    IEnumerable<JobsModel> jobs = db.Jobs.Where(x=>x.Title.Equals(title)&&x.City.Equals(city)).Select(x => new JobsModel
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
            if (title!="" && city=="")
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
            string cate= null;
            dynamic mymodel = new ExpandoObject();
            using (var db = new JobHuntingDBEntities())
            {
                JobCategoryModel category = db.JobCategories.Where(x=>x.Id==id).Select(x => new JobCategoryModel
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

        public ActionResult JobDetail(int id)
        {
            using (var db = new JobHuntingDBEntities())
            {
                JobsModel job = db.Jobs.Where(x=>x.Id==id).Select(x => new JobsModel
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

        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
    }
}