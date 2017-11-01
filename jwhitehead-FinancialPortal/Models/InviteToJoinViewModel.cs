using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jwhitehead_FinancialPortal.Models
{
    public class InviteToJoinViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int Id { get; set; }
    }

}