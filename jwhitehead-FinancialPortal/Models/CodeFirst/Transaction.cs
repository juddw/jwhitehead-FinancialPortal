using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jwhitehead_FinancialPortal.Models.CodeFirst
{
    public class Transaction
    {
        public string UpdatedBy { get; set; }
        public bool Reconciled { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public DateTimeOffset? ReconciliationDate { get; set; } // ? allow date to be null, because when Transaction is created, it will not have a ReconiliationDate.
    }
}