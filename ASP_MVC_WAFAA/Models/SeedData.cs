using ASP_MVC_WAFAA.Data;
using Microsoft.EntityFrameworkCore;

namespace ASP_MVC_WAFAA.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ASP_MVC_WAFAAContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ASP_MVC_WAFAAContext>>()))
            {
                // Look for any Potterys.
                if (context.Pottery.Any())
                {
                    return;   // DB has been seeded
                }
                context.Pottery.AddRange(
                    new Pottery
                    {
                        Title = "egg holder",
                        ReleaseDate = DateTime.Parse("2023-2-12"),
                        Price = 7.99M
                    },
                    new Pottery
                    {
                        Title = "vase ",
                        ReleaseDate = DateTime.Parse("2023-3-13"),
                        Price = 8.99M
                    },
                    new Pottery
                    {
                        Title = "cup",
                        ReleaseDate = DateTime.Parse("2023-2-23"),
                        Price = 9.99M
                    },
                    new Pottery
                    {
                        Title = "plate",
                        ReleaseDate = DateTime.Parse("2023-4-15"),
                        Price = 3.99M
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
