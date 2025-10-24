using Microsoft.EntityFrameworkCore;

namespace QLBH_REST.Models
{
    public class DBQLBH : DbContext
    {
        public DbSet<SANPHAM> tbSANPHAM { get; set; }
        public DBQLBH(DbContextOptions<DBQLBH> options) : base(options) { }

    }
}
