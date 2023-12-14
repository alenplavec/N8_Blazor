namespace AIS_N3_Plavec_Alen.Models
{
    public class IzdelekDobaviteljDTO
    {
        public int Id { get; set; }
        public int IzdelekId { get; set; }
        public string IzdelekNaziv { get; set; }
        public int DobaviteljId { get; set; }
        public string DobaviteljNaziv { get; set; }
        public int KolicinaNaZalogi { get; set; }
    }

}
