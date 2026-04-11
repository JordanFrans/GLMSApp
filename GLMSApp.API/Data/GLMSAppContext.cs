using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GLMSApp.Data
{
    public class GLMSAppContext : DbContext
    {
        public GLMSAppContext(DbContextOptions<GLMSAppContext> options)
            : base(options)
        {
        }

        public DbSet<Contract> Contract { get; set; } = default!;
    }
}
