using AIS_N3_Plavec_Alen.Models;
using Microsoft.EntityFrameworkCore;

namespace AIS_N3_Plavec_Alen;

public class TrgovinaContext : DbContext
{
    public DbSet<Dobavitelj> Dobavitelji { get; set; }
    public DbSet<Izdelek> Izdelki { get; set; }
    public DbSet<IzdelekDobavitelj> IzdelekDobavitelji { get; set; }

    public string DbPath { get; }

    public TrgovinaContext()
    {
        var pot = Directory.GetCurrentDirectory();
        DbPath = Path.Join(pot, "trgovina.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Izdelek>()
            .Property(b => b.Cena)
            .HasDefaultValue(0.0m);
        modelBuilder.Entity<Izdelek>()
            .HasIndex(p => p.Naziv);

        modelBuilder.Entity<Dobavitelj>()
            .HasIndex(p => p.Naziv);
    }

    public void NaloziZacetnePodatke()
    {
        if (!Izdelki.Any())
            Izdelki.AddRange(
                new Izdelek { Id = 1, Naziv = "Svincnik", Opis = "HB2 svincnik.", Cena = 2.99M },
                new Izdelek { Id = 2, Naziv = "Prenosnik", Opis = "Gigabyte G5", Cena = 999.99M },
                new Izdelek { Id = 3, Naziv = "Bakugan", Opis = "Igrača iz risanke Bakugan", Cena = 7.99M },
                new Izdelek { Id = 4, Naziv = "Moped", Opis = "Nova Tomos APN 6tka", Cena = 1899.99M });

        if (!Dobavitelji.Any())
            Dobavitelji.AddRange(
                new Dobavitelj { Id = 1, Naziv = "Dobavitelj A", Lokacija = "Ljubljana", Kontakt = "orgA@orga.com" },
                new Dobavitelj { Id = 2, Naziv = "Dobavitelj B", Lokacija = "Maribor", Kontakt = "orgB@orgb.com" },
                new Dobavitelj { Id = 3, Naziv = "Dobavitelj C", Lokacija = "Murska Sobota", Kontakt = "orgC@orgc.com" });

        if (!IzdelekDobavitelji.Any())
            IzdelekDobavitelji.AddRange(
                new IzdelekDobavitelj { Id = 1, IzdelekId = 1, DobaviteljId = 1, KolicinaNaZalogi = 5 },
                new IzdelekDobavitelj { Id = 2, IzdelekId = 2, DobaviteljId = 2, KolicinaNaZalogi = 10 },
                new IzdelekDobavitelj { Id = 3, IzdelekId = 3, DobaviteljId = 1, KolicinaNaZalogi = 15 },
                new IzdelekDobavitelj { Id = 4, IzdelekId = 4, DobaviteljId = 3, KolicinaNaZalogi = 20 });

        if (!Izdelki.Any() && !Dobavitelji.Any() && !IzdelekDobavitelji.Any())
            SaveChanges();
    }
}