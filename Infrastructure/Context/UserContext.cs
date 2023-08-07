using Application.IServices;
using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.Reflection;

namespace Infrastructure.Context
{
    public class UserContext : IdentityDbContext<User,IdentityRole<int>,int>,IUserContext
    {
        public UserContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        public override DbSet<User> Users { get; set; }
        public DbSet<Adress> Adresses { get; set; }
        public DbSet<Company> Companys { get; set; }
        public DbSet<Geo> Geos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
           
            modelBuilder.Entity<User>().HasQueryFilter(b => !b.IsDeleted);
         
        }

        public async Task<int> SaveChangesAsync()
        {
            //HandleUserDelete();;
            return await base.SaveChangesAsync();
        }
        /*
        private void HandleUserDelete()
        {
            
            var entities = ChangeTracker.Entries()
                                .Where(e => e.State == EntityState.Deleted);
            foreach (var entity in entities)
            {
                if (entity.Entity is User)
                {
                    entity.State = EntityState.Modified;
                    var user = entity.Entity as User;
                    user.IsDeleted = true;
                }
            }
            
        }
        */
    }
}
