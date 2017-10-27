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
            //BankAccounts = new HashSet<BankAccount>();
            //Transactions = new HashSet<Transaction>();
            //Budgets = new HashSet<Budget>();
        }

        public int Id { get; set; }
        public decimal StartAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public int Frequency { get; set; }
        public string Type { get; set; } // income or expense
        public string Description { get; set; }
        //public int UserId { get; set; }
        //public int BankAccountId { get; set; } // foreign key
        //public int TransactionId { get; set; } // foreign key
        public int CategoryId { get; set; } // foreign key
        public int? HouseholdId { get; set; } // foreign key

        // for foreign key connection
        //public virtual BankAccount BankAccount { get; set; }
        public virtual Category Category { get; set; }
        public virtual Household Household { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        //public virtual ICollection<BankAccount> BankAccounts { get; set; }
        //public virtual ICollection<Transaction> Transactions { get; set; }
        //public virtual ICollection<Budget> Budgets { get; set; }

    }
}