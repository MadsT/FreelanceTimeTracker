using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FreelanceTimeTracker.Models
{
    public class ProjectService
    {
        [Key, Column(Order = 0)]
        public int ProjectId { get; set; }
        [Key, Column(Order = 1)]
        public int ServiceID { get; set; }

        [Display(Name = "Project Name")]
        public virtual Project Project { get; set; }

        [Display(Name ="Service")]
        public virtual Service Service { get; set; }

        [Display(Name = "Hours Worked")]
        public int HoursWorked { get; set; }
    }

    public class ProjectServiceViewModel
    {
        [Key, Column(Order = 0)]
        public int ProjectId { get; set; }
        [Key, Column(Order = 1)]
        public int ServiceID { get; set; }

        [Display(Name = "Project Name")]
        public virtual Project Project { get; set; }

        [Display(Name = "Service")]
        public virtual Service Service { get; set; }

        [Display(Name = "Hours Worked")]
        public int HoursWorked { get; set; }

        [Display(Name = "Total Price")]
        [DataType(DataType.Currency)]
        public double TotalPrice { get; set; }
    }
}