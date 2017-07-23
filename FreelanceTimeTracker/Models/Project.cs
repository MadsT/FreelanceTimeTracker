using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreelanceTimeTracker.Models
{
    public class Project
    {
        public int ProjectID { get; set; }
        public int ClientID { get; set; }

        public virtual Client Client { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Services for project")]
        public virtual ICollection<ProjectService> ProjectServices { get; set; }
    }
}