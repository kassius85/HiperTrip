using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HiperTrip.Models
{
    public partial class DbHiperTripContext : DbContext
    {
        public IConfiguration Configuration { get; }

        public DbHiperTripContext()
        {
        }

        public DbHiperTripContext(DbContextOptions<DbHiperTripContext> options,
                                  IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        public virtual DbSet<Usuario> Usuario { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.CodUsuario);

                entity.Property(e => e.CodUsuario)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CodExterno)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ContrasHash)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasDefaultValueSql("(0x00)");

                entity.Property(e => e.CodActivHash)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasDefaultValueSql("(0x00)");

                entity.Property(e => e.ContrasSalt)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasDefaultValueSql("(0x00)");

                entity.Property(e => e.CorreoUsuar)
                    .IsRequired()
                    .HasMaxLength(320)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.NombreUsuar)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.NombreCompl)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.NumCelular)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.TipoUsuExt)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.UsuBorrado)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.UsuConectado)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.UsuarActivo)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('S')");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}