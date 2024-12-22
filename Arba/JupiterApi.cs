using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArbitrageBot
{
    public class JupiterApi
    {
        private readonly HttpClient _httpClient;

        public JupiterApi(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<double> GetJupiterPriceAsync(string inputMint, string outputMint, long amount)
        {
            var url = $"https://quote-api.jup.ag/v6/quote?inputMint={inputMint}&outputMint={outputMint}&amount={amount}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JupiterQuoteResponse>(json);
                return result?.OutAmount / 1000000.0 ?? 0;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Ошибка получения цены с Jupiter: {ex.Message}");
            }
        }
    }

    public class JupiterQuoteResponse
    {
        public string InputMint { get; set; } = string.Empty;
        public long InAmount { get; set; }
        public string OutputMint { get; set; } = string.Empty;
        public long OutAmount { get; set; }
    }
}
