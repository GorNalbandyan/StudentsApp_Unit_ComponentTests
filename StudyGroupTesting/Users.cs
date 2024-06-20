using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyGroupFeature
{
    public class User
    {
        public User(int userId, string firstName, string lastName, string email)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public int UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            User user = (User)obj;
            return UserId == user.UserId;
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }
    }
}
