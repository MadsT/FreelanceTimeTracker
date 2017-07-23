using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreelanceTimeTracker.Models
{
    public class Service
    {
        public int ServiceiD { get; set; }
        public string ServiceOwner { get; set; }
        public string ServiceName { get; set; }
        public double Price { get; set; }

        public virtual ICollection<ProjectService> ProjectServices { get; set; }
    }
}