using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreelanceTimeTracker.Models
{
    public class Service
    {
        public int ServiceiD { get; set; }
        public string ServiceOwner { get; set; }
        public string ServiceName { get; set; }

        [Display(Name ="Price per Hour")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        public virtual ICollection<ProjectService> ProjectServices { get; set; }
    }
}