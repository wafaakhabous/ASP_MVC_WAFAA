using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ASP_MVC_WAFAA.Models;

namespace ASP_MVC_WAFAA.Data
{
    public class ASP_MVC_WAFAAContext : DbContext
    {
        public ASP_MVC_WAFAAContext (DbContextOptions<ASP_MVC_WAFAAContext> options)
            : base(options)
        {
        }

        public DbSet<ASP_MVC_WAFAA.Models.Pottery> Pottery { get; set; } = default!;
    }
}
