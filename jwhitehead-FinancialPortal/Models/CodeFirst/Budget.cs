using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jwhitehead_FinancialPortal.Models.CodeFirst
{
    public class Budget
    {
        public Budget()
        {
            Users = new HashSet<ApplicationUser>();
        }

        public decimal StartAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public int Frequency { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } // income or expense
        public string Description { get; set; }
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public int CategoryId { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}