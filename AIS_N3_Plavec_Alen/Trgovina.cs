using AIS_N3_Plavec_Alen;
using AIS_N3_Plavec_Alen.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq.Dynamic.Core;

namespace WebAPI;

public class Trgovina
{
    private readonly TrgovinaContext _db;

    public Trgovina(TrgovinaContext db)
    {
        _db = db;
    }

    private readonly HashSet<string> dovoljenaPoljaIzdelki = new HashSet<string> { "Naziv", "Kategorija", "Cena" };
    public List<Izdelek> PridobiIzdelke(string attrSortiranja, bool padajoce)
    {
        var poizvedba = _db.Izdelki.AsQueryable();

        if (!string.IsNullOrEmpty(attrSortiranja) && dovoljenaPoljaIzdelki.Contains(attrSortiranja))
        {
            if (attrSortiranja == "Cena")
            {
                poizvedba = padajoce
                    ? poizvedba.OrderByDescending(i => (double)i.Cena)
                    : poizvedba.OrderBy(i => (double)i.Cena);
            }
        }
        else
        {
            var smer = padajoce ? "descending" : "ascending";
            poizvedba = poizvedba.OrderBy($"{attrSortiranja} {smer}");
        }

        return poizvedba.ToList();
    }


    private readonly HashSet<string> dovoljenaPoljaDobavitelji = new HashSet<string> { "Naziv", "Kontakt", "Lokacija" };
    public List<Dobavitelj> PridobiDobavitelje(string attrSortiranja, bool padajoce)
    {
        var poizvedba = _db.Dobavitelji.AsQueryable();

        if (!string.IsNullOrEmpty(attrSortiranja) && dovoljenaPoljaDobavitelji.Contains(attrSortiranja))
        {
            var smer = padajoce ? "descending" : "ascending";
            poizvedba = poizvedba.OrderBy($"{attrSortiranja} {smer}");
        }

        return poizvedba.ToList();
    }

    private readonly HashSet<string> dovoljenaPoljaPovezave = new HashSet<string> { "IzdelekId", "DobaviteljId" };
    public string PridobiSerializiranePovezave(string attrSortiranja, bool padajoce)
    {
        var poizvedba = _db.IzdelekDobavitelji.Include(x => x.Izdelek)
                                              .Include(x => x.Dobavitelj)
                                              .AsQueryable();

        if (!string.IsNullOrEmpty(attrSortiranja) && dovoljenaPoljaPovezave.Contains(attrSortiranja))
        {
            var smer = padajoce ? "descending" : "ascending";
            poizvedba = poizvedba.OrderBy($"{attrSortiranja} {smer}");
        }

        var oblikovaniPodatki = poizvedba.ToList().Select(x => new
        {
            x.Id,
            Izdelek = new
            {
                x.Izdelek.Id,
                x.Izdelek.Naziv,
                x.Izdelek.Opis,
                x.Izdelek.Cena,
                IzdelekDobavitelji = x.Izdelek.IzdelekDobavitelji.Select(id => new
                {
                    id.Id,
                    id.IzdelekId,
                    id.DobaviteljId,
                    id.KolicinaNaZalogi
                }).ToList()
            },
            Dobavitelj = new
            {
                x.Dobavitelj.Id,
                x.Dobavitelj.Naziv,
                x.Dobavitelj.Lokacija,
                x.Dobavitelj.Kontakt,
                IzdelekDobavitelji = x.Dobavitelj.IzdelekDobavitelji.Select(id => new
                {
                    id.Id,
                    id.IzdelekId,
                    id.DobaviteljId,
                    id.KolicinaNaZalogi
                }).ToList()
            },
            x.KolicinaNaZalogi
        }).ToList();

        var json = JsonSerializer.Serialize(oblikovaniPodatki, new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        });

        return json;
    }

    public List<Izdelek> PridobiIzdelkePoDobaviteljId(int id)
    {
        return _db.Izdelki.Where(i => _db.IzdelekDobavitelji.Any(d => d.DobaviteljId == id && d.IzdelekId == i.Id))
            .ToList();
    }

    public List<Dobavitelj> PridobiDobaviteljePoIzdelekId(int id)
    {
        return _db.Dobavitelji.Where(d => _db.IzdelekDobavitelji.Any(i => i.IzdelekId == id && i.DobaviteljId == d.Id))
            .ToList();
    }

    public Izdelek PridobiIzdelekPoId(int id)
    {
        return _db.Izdelki.FirstOrDefault(i => i.Id == id);
    }

    public Dobavitelj PridobiDobaviteljaZNajvecIzdelki()
    {
        return _db.IzdelekDobavitelji.GroupBy(id => id.DobaviteljId)
            .OrderByDescending(i => i.Count())
            .Select(i => _db.Dobavitelji.FirstOrDefault(d => d.Id == i.Key))
            .FirstOrDefault();
    }

    public Izdelek PridobiNajdrazjiIzdelek()
    {
        return _db.Izdelki.ToList().MaxBy(i => i.Cena);
    }

    public decimal PridobiPovprecnoCenoIzdelkov()
    {
        return _db.Izdelki.ToList().Count == 0 ? 0 : _db.Izdelki.ToList().Average(i => i.Cena);
    }
}