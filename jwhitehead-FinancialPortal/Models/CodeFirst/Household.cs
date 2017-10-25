using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jwhitehead_FinancialPortal.Models.CodeFirst
{
    public class Household
    {

        public Household()
        {
            Users = new HashSet<ApplicationUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string AccountId { get; set; }
        public string BudgetId { get; set; }
        public string TransactionId { get; set; }
       
        public virtual ICollection<ApplicationUser> Users {get; set;}
    }
}