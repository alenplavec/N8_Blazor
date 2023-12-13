namespace blazor_n8.Models;

public class Izdelek
{
    public int Id { get; set; }
    public string Naziv { get; set; }
    public string Opis { get; set; }
    public decimal Cena { get; set; }
    public string? Kategorija { get; set; }
    public List<IzdelekDobavitelj> IzdelekDobavitelji { get; set; }
}