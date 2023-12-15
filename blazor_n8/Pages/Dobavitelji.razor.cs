using System.Net.Http.Json;
using System.Text.Json;
using blazor_n8.Models;
using Microsoft.AspNetCore.Components;

namespace blazor_n8
{
    public partial class Dobavitelji : ComponentBase
    {
        private Dobavitelj dobaviteljNajvecIzdelkov;
        private Dobavitelj trenutniDobavitelj;
        private List<Dobavitelj> dobavitelji;
        private List<Izdelek> izdelkiDobavitelja;
        private string trenutniStolpecZaSortiranje = string.Empty;
        private bool sortiranjePadajoce = false;

        protected override async Task OnInitializedAsync()
        {
            await NaloziDobavitelje();
            await PrikaziDobaviteljaZNajvecIzdelki();
        }

        private async Task ObSpremembiAtributa(ChangeEventArgs e)
        {
            trenutniStolpecZaSortiranje = e.Value.ToString();
            await NaloziDobavitelje();
        }

        private async Task SmerSortiranjaSpremenjena()
        {
            sortiranjePadajoce = !sortiranjePadajoce;
            await NaloziDobavitelje();
        }

        private async Task NaloziDobavitelje()
        {
            try
            {
                var url = $"/dobavitelji?sortAttribute={trenutniStolpecZaSortiranje}&descending={(sortiranjePadajoce ? "true" : "false")}";
                var jsonString = await Http.GetStringAsync(url);
                dobavitelji = JsonSerializer.Deserialize<List<Dobavitelj>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                StateHasChanged();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void UrediDobavitelja(Dobavitelj dobavitelj)
        {
            trenutniDobavitelj = dobavitelj;
        }

        private async Task ShraniDobavitelja()
        {
            try
            {
                HttpResponseMessage odgovor;
                if (trenutniDobavitelj.Id == 0)
                {
                    odgovor = await Http.PostAsJsonAsync("/dobavitelji/dodajDobavitelja", trenutniDobavitelj);
                }
                else
                {
                    odgovor = await Http.PutAsJsonAsync($"/dobavitelji/posodobiDobavitelja", trenutniDobavitelj);
                }

                if (odgovor.IsSuccessStatusCode)
                {
                    trenutniDobavitelj = null;
                    await NaloziDobavitelje();
                    StateHasChanged();
                }
            }
            catch (Exception ex) { Console.WriteLine($"{ex.Message}"); }
        }

        private async Task OdstraniDobavitelja(Dobavitelj dobavitelj)
        {
            try
            {
                var odgovor = await Http.DeleteAsync($"/dobavitelji/odstraniDobavitelja/{dobavitelj.Id}");

                if (odgovor.IsSuccessStatusCode)
                {
                    dobavitelji.Remove(dobavitelj);
                    await NaloziDobavitelje();
                    StateHasChanged();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private async Task PrikaziDobaviteljaZNajvecIzdelki()
        {
            try
            {
                dobaviteljNajvecIzdelkov = await Http.GetFromJsonAsync<Dobavitelj>("/dobavitelji/najvecIzdelkov");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private async Task PrikaziIzdelkeDobavitelja(int dobaviteljId)
        {
            try
            {
                if (dobaviteljId != -1)
                    izdelkiDobavitelja = await Http.GetFromJsonAsync<List<Izdelek>>($"/dobavitelji/{dobaviteljId}/izdelki");
                else izdelkiDobavitelja = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
