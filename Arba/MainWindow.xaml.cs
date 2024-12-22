using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArbitrageBot
{
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance { get; private set; }

        private ArbitrageLogic? _arbitrageLogic;
        private CancellationTokenSource? _cancellationTokenSource;
        private JupiterApi _jupiterApi;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            _jupiterApi = new JupiterApi(new HttpClient());
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            txtSolAmount.Text = "0.1";
            txtSlippage.Text = "0.5";
            txtSpreadThreshold.Text = "1.0";
            txtPollingInterval.Text = "10";
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtSolAmount.Text, out var solAmount) || solAmount <= 0)
                    throw new ArgumentException("Некорректное значение количества SOL.");

                if (!decimal.TryParse(txtSlippage.Text, out var slippage) || slippage <= 0 || slippage > 100)
                    throw new ArgumentException("Некорректное значение slippage.");

                if (!decimal.TryParse(txtSpreadThreshold.Text, out var spreadThreshold) || spreadThreshold <= 0)
                    throw new ArgumentException("Некорректное значение порога спреда.");

                if (!int.TryParse(txtPollingInterval.Text, out var pollingInterval) || pollingInterval <= 0)
                    throw new ArgumentException("Некорректное значение интервала опроса.");

                if (string.IsNullOrWhiteSpace(txtPrivateKey.Password))
                    throw new ArgumentException("Приватный ключ не указан.");

                _cancellationTokenSource = new CancellationTokenSource();

                _arbitrageLogic = new ArbitrageLogic(
                    pollingInterval * 1000
                );

                Log("Запуск арбитража...");
                Task.Run(() => _arbitrageLogic.RunArbitrageAsync(
                    (double)spreadThreshold,
                    (double)solAmount,
                    (double)slippage,
                    txtPrivateKey.Password,
                    _cancellationTokenSource.Token
                ));
            }
            catch (Exception ex)
            {
                Log($"Ошибка: {ex.Message}");
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                Log("Арбитраж успешно остановлен.");
            }
            catch (Exception ex)
            {
                Log($"Ошибка при остановке арбитража: {ex.Message}");
            }
        }

        public void Log(string message)
        {
            Dispatcher.Invoke(() =>
            {
                txtLogs.AppendText($"{DateTime.Now:yyyy.MM.dd HH:mm:ss}: {message}\n");
                txtLogs.ScrollToEnd();
            });
        }

        private void TxtPrivateKey_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var password = passwordBox?.Password ?? string.Empty;
            Log("Приватный ключ обновлён.");
        }
    }
}
