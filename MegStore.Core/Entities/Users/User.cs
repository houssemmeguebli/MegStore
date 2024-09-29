using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Core.Entities.Users
{
    public enum UserStatus
    {
        Inactive, Active, Suspended
    }
    public enum Role
    {
        Admin, Customer, SuperAdmin
    }
    public enum Gender
    {
        Male, Female
    }
    public class User : IdentityUser<long>
    {
        public string fullName { get; set; }

        public UserStatus userStatus { get; set; }
        public Role role { get; set; }
        public DateTime dateOfbirth { get; set; }
        public string address { get; set; }

        public Gender gender;
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetCodeExpiration { get; set; }
        public DateTime dateOfCreation { get; set; } = DateTime.Now;


    }
}
