using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using jwhitehead_FinancialPortal.Models.CodeFirst;
using System.Collections.Generic;

namespace jwhitehead_FinancialPortal.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        // jw added
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePic { get; set; }  // this is optional
        public string TimeZone { get; set; }  // when you add, update database! jw 10/5/17
        public bool Leave { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public ApplicationUser()
        {
            BankAccounts = new HashSet<BankAccount>();
            Budgets = new HashSet<Budget>();
            Households = new HashSet<Household>();
            Transactions = new HashSet<Transaction>();
        }

        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<Household> Households { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }



        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        // this will create the tables!
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Household> Households { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Transaction> Category { get; set; }
    }
}