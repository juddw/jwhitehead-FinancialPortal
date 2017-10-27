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
            //BankAccounts = new HashSet<BankAccount>();
            Transactions = new HashSet<Transaction>();
            //Budgets = new HashSet<Budget>();
        }

        public int Id { get; set; }
        public decimal Balance { get; set; }
        public string BankAccountName { get; set; }
        public string Type { get; set; }
        public int? HouseholdId { get; set; } // foreign key

        public virtual ICollection<ApplicationUser> Users { get; set; }
        //public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        //public virtual ICollection<Budget> Budgets { get; set; }

        public virtual Household Household { get; set; } // foreign key connection made to table
    }


}