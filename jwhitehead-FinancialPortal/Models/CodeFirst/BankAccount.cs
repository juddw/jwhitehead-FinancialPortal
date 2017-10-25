using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jwhitehead_FinancialPortal.Models.CodeFirst
{
    public class BankAccount
    {
        public BankAccount()
        {
            Users = new HashSet<ApplicationUser>();
        }

        public int Id { get; set; }
        public decimal Balance { get; set; }
        public string BankAccountName { get; set; }
        public string Type { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }


}