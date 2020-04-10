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

        public virtual DbSet<CambioRestringido> CambioRestringido { get; set; }
        public virtual DbSet<Destino> Destino { get; set; }
        public virtual DbSet<IntentoCambio> IntentoCambio { get; set; }
        public virtual DbSet<Pais> Pais { get; set; }
        public virtual DbSet<ParamGenUsu> ParamGenUsu { get; set; }
        public virtual DbSet<TipoCambioCuenta> TipoCambioCuenta { get; set; }
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
            modelBuilder.Entity<CambioRestringido>(entity =>
            {
                entity.HasKey(e => new { e.CodUsuario, e.FechaSolic });

                entity.Property(e => e.CodUsuario)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.FechaSolic)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CodActivHash)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasDefaultValueSql("(0x00)");

                entity.Property(e => e.CodTipCambCuenta)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.IpSolicita)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.HasOne(d => d.CodTipCambCuentaNavigation)
                    .WithMany(p => p.CambioRestringido)
                    .HasForeignKey(d => d.CodTipCambCuenta)
                    .HasConstraintName("FK_CambioRestringido_TipoCambioCuenta");

                entity.HasOne(d => d.CodUsuarioNavigation)
                    .WithMany(p => p.CambioRestringido)
                    .HasForeignKey(d => d.CodUsuario)
                    .HasConstraintName("FK_CambioRestringido_Usuario");
            });

            modelBuilder.Entity<Destino>(entity =>
            {
                entity.HasKey(e => e.CodigoDestino);

                entity.Property(e => e.CodigoDestino)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CodigoPais)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.NombreDestino)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.HasOne(d => d.CodigoPaisNavigation)
                    .WithMany(p => p.Destino)
                    .HasForeignKey(d => d.CodigoPais)
                    .HasConstraintName("FK_Destino_Pais");
            });

            modelBuilder.Entity<IntentoCambio>(entity =>
            {
                entity.HasKey(e => new { e.CodUsuario, e.FechaSolic, e.CodIntento });

                entity.Property(e => e.CodUsuario)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.FechaSolic)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CodIntento)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.FechaIntento)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IpIntento)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.IntenExitoso)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')");

                entity.HasOne(d => d.CambioRestringido)
                    .WithMany(p => p.IntentoCambio)
                    .HasForeignKey(d => new { d.CodUsuario, d.FechaSolic })
                    .HasConstraintName("FK_IntentoCambio_CambioRestringido");
            });

            modelBuilder.Entity<Pais>(entity =>
            {
                entity.HasKey(e => e.CodigoPais);

                entity.Property(e => e.CodigoPais)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CodigoTelef)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.NombrePais)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<ParamGenUsu>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.CantIntentAct).HasColumnType("numeric(2, 0)");

                entity.Property(e => e.CantIntentRecu).HasColumnType("numeric(2, 0)");

                entity.Property(e => e.CodActiCuenta)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CodRecupCuenta)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.HasOne(d => d.CodActiCuentaNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CodActiCuenta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ParamGenUsu_TipoCambioCuenta1");

                entity.HasOne(d => d.CodRecupCuentaNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.CodRecupCuenta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ParamGenUsu_TipoCambioCuenta2");
            });

            modelBuilder.Entity<TipoCambioCuenta>(entity =>
            {
                entity.HasKey(e => e.CodTipCambCuenta);

                entity.Property(e => e.CodTipCambCuenta)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.CodUsuario);

                entity.Property(e => e.CodUsuario)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ContrasHash)
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

                entity.Property(e => e.FechaRegist)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

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
                    .HasDefaultValueSql("('N')");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}