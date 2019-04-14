using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileSharingApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FileSharingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public virtual DbSet<Speciality> Specialities { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<File> Files { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }

    public class Speciality {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Subject> Subjects { get; set; }
    }

    public class Subject {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Semester { get; set; }

        public int? SpecialityId { get; set; }
        public Speciality Speciality { get; set; }
    }

    public class File {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool Public { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
