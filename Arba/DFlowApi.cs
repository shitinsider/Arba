using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArbitrageBot
{
    public class DFlowApi
    {
        private const string DFlowEndpoint = "https://quote-api.dflow.net/quote";

        public async Task<double> GetPriceAsync(double solAmount, double slippage)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string inputMint = "So11111111111111111111111111111111111111112"; // SOL
                    string outputMint = "EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v"; // USDC
                    long inAmount = (long)(solAmount * 1000000000); // Конвертация SOL в минимальные единицы
                    int slippageBps = (int)(slippage * 100); // Конвертация проскальзывания в bps

                    string query = $"{DFlowEndpoint}?inputMint={inputMint}&outputMint={outputMint}&amount={inAmount}&slippageBps={slippageBps}";

                    MainWindow.Instance.Log($"Отправка GET-запроса на DFlow: {query}");

                    HttpResponseMessage response = await client.GetAsync(query);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    MainWindow.Instance.Log($"Ответ DFlow: {responseBody}");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"DFlow API Error: {response.StatusCode}, {responseBody}");
                    }

                    var result = JsonConvert.DeserializeObject<DFlowQuoteResponse>(responseBody);
                    return result.OutAmount / 1000000.0; // Возвращаем цену в USDC
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения цены с DFlow: {ex.Message}");
            }
        }

        private class DFlowQuoteResponse
        {
            [JsonProperty("outAmount")]
            public long OutAmount { get; set; }
        }
    }
}
