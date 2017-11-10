using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jwhitehead_FinancialPortal.Models.CodeFirst
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Description { get; set; }
        [Display(Name = "Transaction Date")]
        public DateTimeOffset TransactionDate { get; set; }
        [Display(Name = "Reconciliation Date")]
        public DateTimeOffset? ReconciliationDate { get; set; } // ? allow date to be null, because when Transaction is created, it will not have a ReconiliationDate.
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }
        public bool Reconciled { get; set; }
        public bool Void { get; set; }
        public decimal? Amount { get; set; }
        [Display(Name = "Transaction Type")]
        public int TransactionTypeId { get; set; }
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        //public int BudgetId { get; set; }
        [Display(Name = "Bank Account Name")]
        public int? BankAccountId { get; set; }

        public virtual Category Category { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual TransactionType TransactionType { get; set; }
    }
   
}