using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FileSharingApp.Data;

namespace FileSharingApp.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the AppUser class
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public int? Semester { get; set; }

        public int? SpecialityId { get; set; }
        public Speciality Speciality { get; set; }
    }
}
