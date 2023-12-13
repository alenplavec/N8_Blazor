namespace AIS_N3_Plavec_Alen.Models;

public class Dobavitelj
{
    public int Id { get; set; }
    public string Naziv { get; set; }
    public string Lokacija { get; set; }
    public string Kontakt { get; set; }
    public List<IzdelekDobavitelj> IzdelekDobavitelji { get; set; }
}