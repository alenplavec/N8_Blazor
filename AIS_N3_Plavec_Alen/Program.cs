using AIS_N3_Plavec_Alen;
using AIS_N3_Plavec_Alen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(opts => opts.EnableAnnotations());
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "WIP AIS API",
                Description =
                    "Testni API za predmet AIS, vsebuje podatke o izdelkih, njihovih dobaviteljih ter medsebojnimi kontakti"
            });
        });
        builder.Services.AddDbContext<TrgovinaContext>();

        var AllowAny = "_allowAny";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: AllowAny, policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "WIP AIS API V1"); });
        }

        app.UseCors(AllowAny);
        app.UseHttpsRedirection();

        PripraviBazo(app);
        TrgovinaPodatki(app);

        app.Run();
    }

    private static void PripraviBazo(WebApplication app)
    {
        var db = new TrgovinaContext();
        db.Database.EnsureCreated();
        db.NaloziZacetnePodatke();
    }

    private static void TrgovinaPodatki(WebApplication app)
    {
        var db = new TrgovinaContext();
        var trgovina = new Trgovina(db);

        app.MapGet("/izdelki", (string? sortAttribute, bool? descending) =>
            trgovina.PridobiIzdelke(sortAttribute ?? "", descending ?? false))
            .WithMetadata(new SwaggerOperationAttribute("Prika�i vse izdelke",
                "Metoda, ki prika�e vse izdelke v bazi z mo�nostjo sortiranja"));

        app.MapGet("/dobavitelji", (string? sortAttribute, bool? descending) =>
            trgovina.PridobiDobavitelje(sortAttribute ?? "", descending ?? false))
            .WithMetadata(new SwaggerOperationAttribute("Prika�i vse dobavitelje",
                "Metoda, ki prika�e vse dobavitelje v bazi z mo�nostjo sortiranja"));

        app.MapGet("/povezave", (string? attrSortiranja, bool? padajoce) =>
            trgovina.PridobiSerializiranePovezave(attrSortiranja ?? "", padajoce ?? false))
            .WithMetadata(new SwaggerOperationAttribute("Prika�i vse povezave",
                "Metoda, ki prika�e vse povezave med izdelki in dobavitelji z mo�nostjo sortiranja"));

        app.MapGet("/dobavitelji/{id}/izdelki", (int id) => trgovina.PridobiIzdelkePoDobaviteljId(id))
            .WithMetadata(new SwaggerOperationAttribute("Prika�i izdelke dobavitelja",
                "Metoda, ki prika�e vse izdelke dolo�enega dobavitelja"));

        app.MapGet("/izdelki/{id}/dobavitelji", (int id) => trgovina.PridobiDobaviteljePoIzdelekId(id))
            .WithMetadata(new SwaggerOperationAttribute("Prika�i dobavitelje izdelka",
                "Metoda, ki prika�e vse dobavitelje dolo�enega izdelka"));

        app.MapGet("/izdelki/{id}", (int id) => trgovina.PridobiIzdelekPoId(id))
            .WithMetadata(new SwaggerOperationAttribute("Prika�i izdelek",
                "Metoda, ki prika�e podatke o izdelku z dolo�enim ID-jem"));

        app.MapGet("/dobavitelji/najvecIzdelkov", () => trgovina.PridobiDobaviteljaZNajvecIzdelki())
            .WithMetadata(new SwaggerOperationAttribute("Dobavitelj z najve� izdelki",
                "Metoda, ki prika�e dobavitelja z najve� izdelki v bazi"));

        app.MapGet("/izdelki/najvisjaCena", () => trgovina.PridobiNajdrazjiIzdelek())
            .WithMetadata(new SwaggerOperationAttribute("Izdelek z najvi�jo ceno",
                "Metoda, ki prika�e izdelek z najvi�jo ceno"));

        app.MapGet("/izdelki/povprecnaCena", () => trgovina.PridobiPovprecnoCenoIzdelkov())
            .WithMetadata(new SwaggerOperationAttribute("Povpre�na cena izdelkov",
                "Metoda, ki prika�e povpre�no ceno vseh izdelkov"));

        app.MapPost("/izdelki/dodajIzdelek", ([FromBody] Izdelek izdelek) =>
            {
                if (izdelek == null) return Results.BadRequest("Neveljavni podatki o izdelku.");

                db.Izdelki.Add(izdelek);
                db.SaveChanges();
                return Results.Created($"/izdelki/{izdelek.Id}", izdelek);
            }).Produces<Izdelek>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithMetadata(new SwaggerOperationAttribute("Dodaj nov izdelek, Metoda, ki doda nov izdelek v bazo"));

        app.MapPost("/dobavitelji/dodajDobavitelja", ([FromBody] Dobavitelj dobavitelj) =>
            {
                if (dobavitelj == null) return Results.BadRequest("Neveljavni podatki o dobavitelju.");

                db.Dobavitelji.Add(dobavitelj);
                db.SaveChanges();
                return Results.Created($"/dobavitelji/{dobavitelj.Id}", dobavitelj);
            }).Produces(StatusCodes.Status400BadRequest)
            .Produces<Dobavitelj>(StatusCodes.Status201Created)
            .WithMetadata(new SwaggerOperationAttribute("Dodaj novega dobavitelja",
                "Metoda, ki doda novega dobavitelja v bazo"));

        app.MapPost("/povezave/dodajPovezavo", ([FromBody] IzdelekDobavitelj izdelekDobavitelj) =>
        {
            var izdelek = db.Izdelki.Find(izdelekDobavitelj.IzdelekId);
            var dobavitelj = db.Dobavitelji.Find(izdelekDobavitelj.DobaviteljId);

            if (izdelek != null && dobavitelj != null)
            {
                var ustvarjenaPovezava = new IzdelekDobavitelj
                {
                    IzdelekId = izdelekDobavitelj.IzdelekId,
                    DobaviteljId = izdelekDobavitelj.DobaviteljId,
                    Izdelek = izdelek,
                    Dobavitelj = dobavitelj,
                    KolicinaNaZalogi = izdelekDobavitelj.KolicinaNaZalogi
                };

                db.IzdelekDobavitelji.Add(ustvarjenaPovezava);
                db.SaveChanges();
                return Results.Ok("Povezava ustvarjena.");
            }

            return Results.NotFound("Izdelek ali dobavitelj ni bil najden.");
        }).Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithMetadata(new SwaggerOperationAttribute("Dodaj povezavo izdelek-dobavitelj",
                "Metoda, ki doda povezavo med izdelkom in dobaviteljem"));

        app.MapPost("/izdelki/dodajDvaIzdelka", () =>
        {
            using var transakcija = db.Database.BeginTransaction();

            try
            {
                var novIzdelek1 = new Izdelek
                {
                    Naziv = "Izdelek 1",
                    Opis = "Opis izdelka 1",
                    Cena = 100.00m,
                    Kategorija = "Kategorija 1"
                };

                var novIzdelek2 = new Izdelek
                {
                    Naziv = "Izdelek 2",
                    Opis = "Opis izdelka 2",
                    Cena = 200.00m,
                    Kategorija = "Kategorija 2"
                };

                var obstajaIzdelek1 = db.Izdelki.Any(i => i.Naziv == novIzdelek1.Naziv && i.Opis == novIzdelek1.Opis);
                var obstajaIzdelek2 = db.Izdelki.Any(i => i.Naziv == novIzdelek2.Naziv && i.Opis == novIzdelek2.Opis);
                var spremembaVbazi = false;

                if (!obstajaIzdelek1)
                {
                    db.Izdelki.Add(novIzdelek1);
                    spremembaVbazi = true;
                }

                if (!obstajaIzdelek2)
                {
                    db.Izdelki.Add(novIzdelek2);
                    spremembaVbazi = true;
                }

                if (!spremembaVbazi) return Results.Conflict("Eden ali oba izdelka �e obstajata v bazi.");

                db.SaveChanges();
                transakcija.Commit();

                return Results.Ok("Operacija uspe�no izvedena. Novi Izdelki so bili dodani.");
            }
            catch (Exception ex)
            {
                transakcija.Rollback();
                return Results.Problem("Med operacijo je pri�lo do napake: " + ex.Message);
            }
        }).WithMetadata(new SwaggerOperationAttribute("Dodaj dva vnaprej pripravljena testna izdelka",
            "Metoda, ki doda dva nova izdelka v bazo, �e �e ne obstajata"));

        app.MapPut("/izdelki/posodobiIzdelek", ([FromBody] Izdelek izdelek) =>
            {
                var obstojecIzdelek = db.Izdelki.FirstOrDefault(x => x.Id == izdelek.Id);
                if (obstojecIzdelek == null) return Results.NotFound("Izdelek ni bil najden.");

                obstojecIzdelek.Naziv = izdelek.Naziv;
                obstojecIzdelek.Opis = izdelek.Opis;
                obstojecIzdelek.Cena = izdelek.Cena;

                db.SaveChanges();
                return Results.Ok();
            }).Produces(StatusCodes.Status200OK)
            .Produces<Izdelek>(StatusCodes.Status404NotFound)
            .WithMetadata(new SwaggerOperationAttribute("Posodobi izdelek",
                "Metoda, ki posodobi podatke o izdelku"));

        app.MapPut("/dobavitelji/posodobiDobavitelja", ([FromBody] Dobavitelj dobavitelj) =>
            {
                var obstojecDobavitelj = db.Dobavitelji.FirstOrDefault(x => x.Id == dobavitelj.Id);
                if (obstojecDobavitelj == null) return Results.NotFound("Dobavitelj ni bil najden.");

                obstojecDobavitelj.Naziv = dobavitelj.Naziv;
                obstojecDobavitelj.Lokacija = dobavitelj.Lokacija;
                obstojecDobavitelj.Kontakt = dobavitelj.Kontakt;

                db.SaveChanges();
                return Results.NoContent();
            }).Produces(StatusCodes.Status204NoContent)
            .Produces<Dobavitelj>(StatusCodes.Status404NotFound)
            .WithMetadata(new SwaggerOperationAttribute("Posodobi dobavitelja",
                "Metoda, ki posodobi podatke o dobavitelju"));

        app.MapDelete("/izdelki/odstraniIzdelek/{id}", (int id) =>
            {
                var izdelekZaBrisanje = db.Izdelki.FirstOrDefault(x => x.Id == id);
                if (izdelekZaBrisanje == null) return Results.NotFound("Izdelek ni bil najden.");

                db.Izdelki.Remove(izdelekZaBrisanje);
                db.SaveChanges();
                return Results.NoContent();
            }).Produces(StatusCodes.Status204NoContent)
            .Produces<Izdelek>(StatusCodes.Status404NotFound)
            .WithMetadata(new SwaggerOperationAttribute("Izbri�i izdelek", "Metoda, ki izbri�e izdelek iz baze"));


        app.MapDelete("/dobavitelji/odstraniDobavitelja/{id}", (int id) =>
            {
                var najdenDobavitelj = db.Dobavitelji.FirstOrDefault(x => x.Id == id);
                if (najdenDobavitelj == null) return Results.NotFound("Dobavitelj ni bil najden.");

                db.Dobavitelji.Remove(najdenDobavitelj);
                db.SaveChanges();
                return Results.NoContent();
            }).Produces(StatusCodes.Status204NoContent)
             .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
             .WithMetadata(new SwaggerOperationAttribute("Odstrani dobavitelja", "Metoda, ki odstrani dobavitelja iz baze"));


        app.MapDelete("/povezave/odstraniPovezavo", ([FromBody] IzdelekDobavitelj izdelekDobavitelj) =>
        {
            if (izdelekDobavitelj == null) return Results.BadRequest("Neveljavni podatki o povezavi.");

            var najdenIzdelekDobavitelj = db.IzdelekDobavitelji.FirstOrDefault(x => x.Id == izdelekDobavitelj.Id);
            db.IzdelekDobavitelji.Remove(najdenIzdelekDobavitelj);
            db.SaveChanges();
            return Results.NoContent();
        }).WithMetadata(new SwaggerOperationAttribute("Odstrani povezavo",
                "Metoda, ki odstrani" +
                " podatke o dobavitelju"));
    }
}