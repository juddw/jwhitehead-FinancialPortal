using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentDateTime;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Start Amount")]
        public decimal StartAmount { get; set; }
        [Display(Name = "Frequency")]
        public int FrequencyId { get; set; }
        public string Type { get; set; } // income or expense
        public string Description { get; set; }
        //public int UserId { get; set; }
        //public int BankAccountId { get; set; } // foreign key
        //public int TransactionId { get; set; } // foreign key
        [Display(Name = "Category")]
        public int CategoryId { get; set; } // foreign key
        [Display(Name = "Household")]
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
                if (Household != null && Category != null && Frequency != null)
                {
                    decimal amount = 0;
                    if (Frequency.Name == "Weekly")
                    {
                        var previousSunday = DateTime.Now.Previous(DayOfWeek.Sunday);
                        var nextMonday = DateTime.Now.Next(DayOfWeek.Monday);
                        foreach (var trans in Category.Transactions.Where(t => t.BankAccount.HouseholdId == HouseholdId && t.TransactionDate > previousSunday && t.TransactionDate < nextMonday && t.Void == false).ToList())
                        {
                            amount -= trans.Amount;
                        }
                        return amount;
                    }

                    else if (Frequency.Name == "Monthly")
                    {
                        foreach (var trans in Category.Transactions.Where(t => t.BankAccount.HouseholdId == HouseholdId && t.TransactionDate.Month == DateTime.Now.Month && t.TransactionDate.Year == DateTime.Now.Year && t.Void == false).ToList())
                        {
                            amount -= trans.Amount;
                        }
                        return amount;
                    }
                    else if (Frequency.Name == "Yearly")
                    {
                        foreach (var trans in Category.Transactions.Where(t => t.BankAccount.HouseholdId == HouseholdId && t.TransactionDate.Year == DateTime.Now.Year && t.Void == false).ToList())
                        {
                            amount -= trans.Amount;
                        }
                        return amount;
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

        public int? BudgetPercentage
        {
            get
            {
                if (SpentAmount > 0)
                {
                    return Convert.ToInt32((SpentAmount / StartAmount) * 100);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}