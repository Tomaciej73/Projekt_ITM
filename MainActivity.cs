using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using ZXing.Mobile;

namespace MyApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void SkanujButton_Clicked(object sender, EventArgs e)
        {
            var scanner = new MobileBarcodeScanner();
            var result = await scanner.Scan();

            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var awaria = JsonConvert.DeserializeObject<Awaria>(json);

                    var awariaText = $"Awaria: {awaria.Numer}{Environment.NewLine}Stanowisko: {awaria.Stanowisko}{Environment.NewLine}Opis: {awaria.Opis}{Environment.NewLine}Priorytet: {awaria.Priorytet}{Environment.NewLine}Czy możliwa jest kontynuacja pracy?: {awaria.MozliwoscKontynuacjiPracy}";

                    var answer = await DisplayAlert("Potwierdzenie", awariaText, "Tak", "Nie");

                    if (answer)
                    {
                        var postData = new Dictionary<string, string>
                        {
                            { "numer", awaria.Numer },
                            { "stanowisko", awaria.Stanowisko },
                            { "opis", awaria.Opis },
                            { "priorytet", awaria.Priorytet },
                            { "mozliwoscKontynuacjiPracy", awaria.MozliwoscKontynuacjiPracy }
                        };

                        var content = new FormUrlEncodedContent(postData);
                        var postResponse = await httpClient.PostAsync("", content);

                        if (postResponse.IsSuccessStatusCode)
                        {
                            await DisplayAlert("Sukces", "Awaria została podjęta", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Błąd", "Wystąpił błąd podczas podjęcia awarii", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Anulowanie", "Awaria nie została podjęta", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Błąd", "Wystąpił błąd podczas pobierania danych o awarii", "OK");
                }
            }
        }
    }

    public class Awaria
    {
        [JsonProperty("numer")]
        public string Numer { get; set; }

        [JsonProperty("stanowisko")]
        public string Stanowisko { get; set; }

        [JsonProperty("opis")]
        public string Opis { get; set; }

        [JsonProperty("priorytet")]
        public string Priorytet { get; set; }

        [JsonProperty("mozliwoscKontynuacjiPracy")]
        public string MozliwoscKontynuacjiPracy { get; set; }
    }
}
