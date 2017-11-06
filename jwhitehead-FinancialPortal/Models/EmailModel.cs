using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jwhitehead_FinancialPortal.Models
{
    public class EmailModel
    {
        //[Required]
        //public string Subject { get; set; }
        //[Required]
        //public string Body { get; set; }
        //[Required]
        //public string EmailTo { get; set; }

        [Required, Display(Name = "Email"), EmailAddress]  // login view model
        public string ToEmail { get; set; }
        [Required]
        public string Body { get; set; }
        public int Id { get; set; }
        //public int HouseholdId { get; set; } // may need to add.
    }
}