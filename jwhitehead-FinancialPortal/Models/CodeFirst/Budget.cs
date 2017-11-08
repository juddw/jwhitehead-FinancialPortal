using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentDateTime;

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
        public int FrequencyId { get; set; }
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
        public virtual Frequency Frequency { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        //public virtual ICollection<BankAccount> BankAccounts { get; set; }
        //public virtual ICollection<Transaction> Transactions { get; set; }
        //public virtual ICollection<Budget> Budgets { get; set; }


        public decimal? SpentAmount
        {
            get
            {
                if (Category != null)
                {
                    if (Frequency != null && Frequency.Name == "Weekly")
                    {
                        var previousSunday = DateTime.Now.Previous(DayOfWeek.Sunday);
                        var nextMonday = DateTime.Now.Next(DayOfWeek.Monday);
                        return Category.Transactions.Where(t => t.TransactionDate > previousSunday && t.TransactionDate < nextMonday && t.Void == false).Sum(t => t.Amount);

                    }
                    else if (Frequency != null && Frequency.Name == "Monthly")
                    {
                        return Category.Transactions.Where(t => t.TransactionDate.Month == DateTime.Now.Month && t.TransactionDate.Year == DateTime.Now.Year && t.Void == false).Sum(t => t.Amount);

                    }
                    else if (Frequency != null && Frequency.Name == "Yearly")
                    {
                        return Category.Transactions.Where(t => t.TransactionDate.Year == DateTime.Now.Year && t.Void == false).Sum(t => t.Amount);

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

            }
        }

        public decimal? LeftAmount
        {
            get
            {
                return StartAmount - SpentAmount;
            }
        }

    }
}