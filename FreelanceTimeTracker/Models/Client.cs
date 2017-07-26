using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FreelanceTimeTracker.Models
{
    public class Client
    {
        public int ClientID { get; set; }
        
        public string ClientOwner { get; set; }

        [Required]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

    }

  
}