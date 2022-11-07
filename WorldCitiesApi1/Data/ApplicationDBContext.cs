using Microsoft.EntityFrameworkCore;
using WorldCitiesApi1.Data.Models;

namespace WorldCitiesApi1.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext() : base()
        {

        }
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<City> Cities => Set<City>();
        public DbSet<Country> Countries => Set<Country>();
    }
}
