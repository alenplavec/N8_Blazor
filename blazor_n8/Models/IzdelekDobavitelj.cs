namespace AIS_N3_Plavec_Alen.Models;

public class IzdelekDobavitelj
{
    public int Id { get; set; }
    public int IzdelekId { get; set; }
    public Izdelek Izdelek { get; set; }
    public int DobaviteljId { get; set; }
    public Dobavitelj Dobavitelj { get; set; }
    public int KolicinaNaZalogi { get; set; }
}