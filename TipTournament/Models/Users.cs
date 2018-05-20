using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TipTournament.Models
{
    public class Users
    {
        private List<ApplicationUser> users;

        public Users(List<ApplicationUser> users)
        {
            this.users = users;
        }

        public List<ApplicationUser> GetUsers()
        {
            return users;
        }

        public string GetUserName(string userId)
        {
            var user = users.FirstOrDefault(item => item.Id.Equals(userId));
            if(user != null)
                return user.UserName;

            return null;
        }


    }
}