using System;
using System.Threading;
using System.Threading.Tasks;

namespace ArbitrageBot
{
    public class ArbitrageLogic
    {
        private readonly DFlowApi _dFlowApi;
        private readonly JupiterApi _jupiterApi;
        private readonly int _pollingInterval;

        public ArbitrageLogic(int pollingInterval)
        {
            _dFlowApi = new DFlowApi();
            _jupiterApi = new JupiterApi();
            _pollingInterval = pollingInterval;
        }

        public async Task RunArbitrageAsync(double spreadThreshold, double solAmount, double slippage, string privateKey, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    double dflowPrice = await _dFlowApi.GetPriceAsync(solAmount, slippage);
                    double jupiterPrice = await _jupiterApi.GetPriceAsync(solAmount);

                    double spread = jupiterPrice - dflowPrice;
                    MainWindow.Instance.Log($"Текущий спред: {spread} USDC");

                    if (spread >= spreadThreshold)
                    {
                        MainWindow.Instance.Log($"Спред подходит. Выполняется арбитраж...");
                        await ExecuteArbitrageAsync(dflowPrice, jupiterPrice, solAmount, privateKey, token);
                    }
                    else
                    {
                        MainWindow.Instance.Log($"Спред не подходит. Текущее значение: {spread} USDC.");
                    }

                    await Task.Delay(_pollingInterval, token);
                }
                catch (Exception ex)
                {
                    MainWindow.Instance.Log($"Ошибка: {ex.Message}");
                }
            }
        }

        private async Task ExecuteArbitrageAsync(double dflowPrice, double jupiterPrice, double solAmount, string privateKey, CancellationToken token)
        {
            try
            {
                MainWindow.Instance.Log($"Исполнение сделки...");
                // Логика для совершения сделок через DFlow и Jupiter
                await Task.Delay(1000, token); // Заглушка
                MainWindow.Instance.Log($"Сделка успешно завершена.");
            }
            catch (Exception ex)
            {
                MainWindow.Instance.Log($"Ошибка при исполнении сделки: {ex.Message}");
            }
        }
    }
}
