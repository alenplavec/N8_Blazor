using Microsoft.AspNetCore.Components;
using blazor_n8.Models;
using System.Net.Http.Json;

namespace blazor_n8
{
    public partial class Izdelki : ComponentBase
    {
        private Izdelek izdelekNajvisjaCena;
        private Izdelek trenutniIzdelek;
        private List<Izdelek> izdelki;
        private List<Dobavitelj> dobavitelji;
        private decimal povprecnaCena = 0;
        private string trenutniStolpecZaSortiranje;
        private bool sortiranjePadajoce = false;

        protected override async Task OnInitializedAsync()
        {
            await NaloziIzdelke();
        }

        private async Task NaloziIzdelke()
        {
            try
            {
                await PrikaziNajvisjoCeno();
                await PrikaziPovprecnoCeno();
                var url = $"/izdelki?sortAttribute={trenutniStolpecZaSortiranje}&descending={(sortiranjePadajoce ? "true" : "false")}";
                izdelki = await Http.GetFromJsonAsync<List<Izdelek>>(url);
                StateHasChanged();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private async Task Sortiraj(string imeStolpca)
        {
            if (imeStolpca == trenutniStolpecZaSortiranje)
            {
                sortiranjePadajoce = !sortiranjePadajoce;
            }
            else
            {
                trenutniStolpecZaSortiranje = imeStolpca;
                sortiranjePadajoce = false;
            }

            await NaloziIzdelke();
        }

        private async Task ObSpremembiAtributa(ChangeEventArgs e)
        {
            trenutniStolpecZaSortiranje = e.Value.ToString();
            await NaloziIzdelke();
        }

        private async Task SmerSortiranjaSpremenjena()
        {
            sortiranjePadajoce = !sortiranjePadajoce;
            await NaloziIzdelke();
        }

        private async Task PrikaziDobavitelje(int izdelekId)
        {
            try
            {
                if (izdelekId != -1)
                    dobavitelji = await Http.GetFromJsonAsync<List<Dobavitelj>>($"/izdelki/{izdelekId}/dobavitelji");
                else dobavitelji = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private async Task OdstraniIzdelek(Izdelek izdelek)
        {
            try
            {
                var odgovor = await Http.DeleteAsync($"/izdelki/odstraniIzdelek/{izdelek.Id}");

                if (odgovor.IsSuccessStatusCode)
                {
                    izdelki.Remove(izdelek);
                    NaloziIzdelke();
                    StateHasChanged();
                }
            }
            catch (Exception ex){ Console.WriteLine(ex.Message); }
        }

        private async Task PrikaziNajvisjoCeno()
        {
            try
            {
                izdelekNajvisjaCena = await Http.GetFromJsonAsync<Izdelek>("/izdelki/najvisjaCena");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private async Task PrikaziPovprecnoCeno()
        {
            try
            {
                povprecnaCena = await Http.GetFromJsonAsync<decimal>("/izdelki/povprecnaCena");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void UrediIzdelek(Izdelek izdelek)
        {
            trenutniIzdelek = izdelek;
        }

        private async Task ShraniIzdelek()
        {
            HttpResponseMessage odgovor;
            if (trenutniIzdelek.Id == 0)
            {
                odgovor = await Http.PostAsJsonAsync("/izdelki/dodajIzdelek", trenutniIzdelek);
            }
            else
            {
                odgovor = await Http.PutAsJsonAsync($"/izdelki/posodobiIzdelek/", trenutniIzdelek);
            }

            if (odgovor.IsSuccessStatusCode)
            {
                trenutniIzdelek = null;
                await NaloziIzdelke();
            }
        }
    }
}