using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobHunting.Models
{
    public class JobCategoryModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Nullable<int> JobNumbers { get; set; }
    }
}