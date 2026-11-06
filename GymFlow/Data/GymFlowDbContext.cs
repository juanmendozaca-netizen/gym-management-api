using System;
using System.Collections.Generic;
using GymFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace GymFlow.Data;

public partial class GymFlowDbContext : DbContext
{
    public GymFlowDbContext(DbContextOptions<GymFlowDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Membresia> Membresias { get; set; }

    public virtual DbSet<Socio> Socios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Membresia>(entity =>
        {
            entity.Property(e => e.Activa).HasDefaultValue(true, "DF_Membresias_Activa");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysutcdatetime())", "DF_Membresias_FechaCreacion");
        });

        modelBuilder.Entity<Socio>(entity =>
        {
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(sysutcdatetime())", "DF_Socios_FechaRegistro");

            entity.HasOne(d => d.Membresia).WithMany(p => p.Socios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Socios_Membresias");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
