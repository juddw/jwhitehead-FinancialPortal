using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jwhitehead_FinancialPortal.Models.CodeFirst
{
    public class Category
    {
        public Category()
        {
            //Users = new HashSet<ApplicationUser>();
            //BankAccounts = new HashSet<BankAccount>();
            Transactions = new HashSet<Transaction>();
            Budgets = new HashSet<Budget>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        //public virtual ICollection<ApplicationUser> Users { get; set; }
        //public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
    }
}