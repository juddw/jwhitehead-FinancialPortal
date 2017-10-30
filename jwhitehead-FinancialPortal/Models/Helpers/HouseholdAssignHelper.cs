using jwhitehead_FinancialPortal.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jwhitehead_FinancialPortal.Models.Helpers
{
    public class HouseholdAssignHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Is user in household
        public bool IsUserInHousehold(string userId, int householdId)
        {
            var household = db.Households.Find(householdId);
            var userBool = household.Users.Any(u => u.Id == userId);
            return userBool;
        }

        // Add user to household
        public void AddUserToHousehold(string userId, int householdId)
        {
            var user = db.Users.Find(userId);
            var household = db.Households.Find(householdId);
            household.Users.Add(user);
            db.SaveChanges();
        }

        // Remove user from household
        public void RemoveUserFromHousehold(string userId, int householdId)
        {
            var user = db.Users.Find(userId);
            var household = db.Households.Find(householdId);
            household.Users.Remove(user);
            db.SaveChanges();
        }
    }
}