using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Project.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.AppDBContext
{
    public class ProjectDBContext : IdentityDbContext
    {
        public ProjectDBContext(DbContextOptions<ProjectDBContext> options) : base(options) 
        {
            
        }

        //public class YourDbContextFactory : IDesignTimeDbContextFactory<ProjectDBContext>
        //{
        //    public ProjectDBContext CreateDbContext(string[] args)
        //    {
        //        var optionsBuilder = new DbContextOptionsBuilder<ProjectDBContext>();
        //        optionsBuilder.UseSqlServer("Server=.;Database=AuthorizationAuthenticationProject;Trusted_Connection=True;TrustServerCertificate=True;");
        //        return new ProjectDBContext(optionsBuilder.Options);
        //    }
        //}
        //public DbSet<Department> Departments { get; set; }
    }
}
