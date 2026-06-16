using System.Windows;
using System.Windows.Controls;
using CurrencyExchange.WpfClient.CurrencyServiceRef;

namespace CurrencyExchange.WpfClient
{
    public partial class MainWindow : Window
    {
        private CurrencyServiceClient _client;
        private string _loggedInUser = null;
        private bool _passwordVisible = false;

        public MainWindow()
        {
            InitializeComponent();
            _client = new CurrencyServiceClient();
            ShowPanel(LoginPanel);
        }

        // ── Password show/hide ─────────────────────────────────────────────
        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            _passwordVisible = !_passwordVisible;
            if (_passwordVisible)
            {
                LoginPasswordVisible.Text = LoginPassword.Password;
                LoginPasswordVisible.Visibility = Visibility.Visible;
                LoginPassword.Visibility = Visibility.Collapsed;
                TogglePassword.Content = "🙈";
            }
            else
            {
                LoginPassword.Password = LoginPasswordVisible.Text;
                LoginPassword.Visibility = Visibility.Visible;
                LoginPasswordVisible.Visibility = Visibility.Collapsed;
                TogglePassword.Content = "👁";
            }
        }

        private void Password_Changed(object sender, RoutedEventArgs e)
        {
            if (!_passwordVisible)
                LoginPasswordVisible.Text = LoginPassword.Password;
        }

        private void PasswordVisible_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_passwordVisible)
                LoginPassword.Password = LoginPasswordVisible.Text;
        }

        // ── Panel switching ────────────────────────────────────────────────
        private void ShowPanel(UIElement panel)
        {
            LoginPanel.Visibility = Visibility.Collapsed;
            RatesPanel.Visibility = Visibility.Collapsed;
            AccountPanel.Visibility = Visibility.Collapsed;
            BuyPanel.Visibility = Visibility.Collapsed;
            SellPanel.Visibility = Visibility.Collapsed;
            TransactionsPanel.Visibility = Visibility.Collapsed;
            HistoryPanel.Visibility = Visibility.Collapsed;
            panel.Visibility = Visibility.Visible;
        }

        // ── Auth ───────────────────────────────────────────────────────────
        private string GetPassword()
        {
            return _passwordVisible ? LoginPasswordVisible.Text : LoginPassword.Password;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginUsername.Text))
            {
                LoginMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                LoginMsg.Text = "❌ Please enter a username.";
                return;
            }
            if (string.IsNullOrWhiteSpace(GetPassword()))
            {
                LoginMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                LoginMsg.Text = "❌ Please enter a password.";
                return;
            }

            bool ok = _client.Login(LoginUsername.Text.Trim(), GetPassword());
            if (ok)
            {
                _loggedInUser = LoginUsername.Text.Trim();
                UserText.Text = _loggedInUser;
                StatusText.Text = "● Online";
                BtnSignOut.Visibility = Visibility.Visible;
                LoginMsg.Text = "";
                LoginUsername.Text = "";
                LoginPassword.Password = "";
                LoginPasswordVisible.Text = "";
                ShowPanel(RatesPanel);
                RefreshRates_Click(null, null);
            }
            else
            {
                LoginMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                LoginMsg.Text = "❌ Invalid username or password.";
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginUsername.Text))
            {
                LoginMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                LoginMsg.Text = "❌ Username cannot be empty.";
                return;
            }
            if (string.IsNullOrWhiteSpace(GetPassword()))
            {
                LoginMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                LoginMsg.Text = "❌ Password cannot be empty.";
                return;
            }
            if (LoginUsername.Text.Trim().Length < 3)
            {
                LoginMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                LoginMsg.Text = "❌ Username must be at least 3 characters.";
                return;
            }
            if (GetPassword().Length < 4)
            {
                LoginMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                LoginMsg.Text = "❌ Password must be at least 4 characters.";
                return;
            }

            string result = _client.RegisterUser(LoginUsername.Text.Trim(), GetPassword());
            if (result == "OK")
            {
                LoginMsg.Foreground = System.Windows.Media.Brushes.LightGreen;
                LoginMsg.Text = "✅ Account created! You can now sign in.";
            }
            else
            {
                LoginMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                LoginMsg.Text = "❌ " + result;
            }
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to sign out?",
                "Sign Out",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _loggedInUser = null;
                UserText.Text = "Not logged in";
                StatusText.Text = "● Online";
                BtnSignOut.Visibility = Visibility.Collapsed;
                ShowPanel(LoginPanel);
            }
        }

        // ── Nav ────────────────────────────────────────────────────────────
        private bool CheckLogin()
        {
            if (_loggedInUser == null)
            {
                MessageBox.Show("Please sign in first!", "Not Logged In",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void ShowRates_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel(RatesPanel);
            RefreshRates_Click(null, null);
        }

        private void ShowAccount_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckLogin()) return;
            ShowPanel(AccountPanel);
            AccountUser.Text = "Logged in as " + _loggedInUser;
            double bal = _client.GetUserBalance(_loggedInUser, "PLN");
            PlnBalance.Text = bal.ToString("F2") + " PLN";
        }

        private void ShowBuy_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckLogin()) return;
            ShowPanel(BuyPanel);
        }

        private void ShowSell_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckLogin()) return;
            ShowPanel(SellPanel);
        }

        private void ShowTransactions_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckLogin()) return;
            ShowPanel(TransactionsPanel);
            TxGrid.ItemsSource = _client.GetUserTransactions(_loggedInUser);
        }

        private void ShowHistory_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckLogin()) return;
            ShowPanel(HistoryPanel);
        }

        // ── Features ───────────────────────────────────────────────────────
        private void RefreshRates_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RatesGrid.ItemsSource = _client.GetAllRates();
                StatusText.Text = "● Online";
                StatusText.Foreground = System.Windows.Media.Brushes.LightGreen;
            }
            catch
            {
                StatusText.Text = "● Offline";
                StatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void TopUp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TopUpAmount.Text))
            {
                AccountMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                AccountMsg.Text = "❌ Please enter an amount.";
                return;
            }
            double amt;
            if (!double.TryParse(TopUpAmount.Text, out amt) || amt <= 0)
            {
                AccountMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                AccountMsg.Text = "❌ Please enter a valid positive amount.";
                return;
            }
            _client.TopUpAccount(_loggedInUser, amt);
            AccountMsg.Foreground = System.Windows.Media.Brushes.LightGreen;
            AccountMsg.Text = "✅ Added " + amt.ToString("F2") + " PLN successfully!";
            double bal = _client.GetUserBalance(_loggedInUser, "PLN");
            PlnBalance.Text = bal.ToString("F2") + " PLN";
            TopUpAmount.Text = "";
        }

        private void BuyCurrency_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BuyCurrencyCode.Text))
            {
                BuyMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                BuyMsg.Text = "❌ Please enter a currency code.";
                return;
            }
            double amt;
            if (!double.TryParse(BuyAmount.Text, out amt) || amt <= 0)
            {
                BuyMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                BuyMsg.Text = "❌ Please enter a valid positive amount.";
                return;
            }
            string result = _client.BuyCurrency(_loggedInUser, BuyCurrencyCode.Text.ToUpper(), amt);
            if (result.StartsWith("OK"))
            {
                BuyMsg.Foreground = System.Windows.Media.Brushes.LightGreen;
                BuyMsg.Text = "✅ " + result;
                BuyAmount.Text = "";
            }
            else
            {
                BuyMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                BuyMsg.Text = result;
            }
        }

        private void SellCurrency_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SellCurrencyCode.Text))
            {
                SellMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                SellMsg.Text = "❌ Please enter a currency code.";
                return;
            }
            double amt;
            if (!double.TryParse(SellAmount.Text, out amt) || amt <= 0)
            {
                SellMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                SellMsg.Text = "❌ Please enter a valid positive amount.";
                return;
            }
            string result = _client.SellCurrency(_loggedInUser, SellCurrencyCode.Text.ToUpper(), amt);
            if (result.StartsWith("OK"))
            {
                SellMsg.Foreground = System.Windows.Media.Brushes.LightGreen;
                SellMsg.Text = "✅ " + result;
                SellAmount.Text = "";
            }
            else
            {
                SellMsg.Foreground = System.Windows.Media.Brushes.OrangeRed;
                SellMsg.Text = result;
            }
        }

        private void GetHistory_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HistCurrency.Text))
            {
                MessageBox.Show("Please enter a currency code.", "Missing Input",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            HistGrid.ItemsSource = _client.GetHistoricalRates(
                HistCurrency.Text.ToUpper(), HistStart.Text, HistEnd.Text);
        }
    }
}