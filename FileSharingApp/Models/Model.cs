using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FileSharingApp.Models {
    public class ModelDbContext : DbContext {
        public ModelDbContext(DbContextOptions<ModelDbContext> options)
            : base(options) { }

        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<File> Files { get; set; }
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

        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
