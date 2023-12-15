using System.Net.Http.Json;
using blazor_n8.Models;
using Microsoft.AspNetCore.Components;

namespace blazor_n8
{
    public partial class Povezave : ComponentBase
    {
        private IzdelekDobaviteljDTO novaPovezava = new IzdelekDobaviteljDTO();
        private List<IzdelekDobaviteljDTO> povezave;
        private bool dodajanjePovezave = false;
        private bool sortiranjePadajoce = false;
        private string trenutniStolpecZaSortiranje = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await NaloziPovezave();
        }

        private async Task NaloziPovezave()
        {
            try
            {
                var url = $"/povezave?attrSortiranja={trenutniStolpecZaSortiranje}&padajoce={sortiranjePadajoce}";
                povezave = await Http.GetFromJsonAsync<List<IzdelekDobaviteljDTO>>(url);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private async Task ObSpremembiAtributa(ChangeEventArgs e)
        {
            trenutniStolpecZaSortiranje = e.Value.ToString();
            await NaloziPovezave();
        }

        private async Task SmerSortiranjaSpremenjena()
        {
            sortiranjePadajoce = !sortiranjePadajoce;
            await NaloziPovezave();
        }

        private async Task DodajPovezavo()
        {
            try
            {
                var odgovor = await Http.PostAsJsonAsync("/povezave/dodajPovezavo", novaPovezava);
                if (odgovor.IsSuccessStatusCode)
                {
                    dodajanjePovezave = false;
                    novaPovezava = new IzdelekDobaviteljDTO();
                    await NaloziPovezave();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private async Task OdstraniPovezavo(int id)
        {
            try
            {
                var odgovor = await Http.DeleteAsync($"/povezave/odstraniPovezavo/{id}");
                if (odgovor.IsSuccessStatusCode)
                {
                    await NaloziPovezave();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
