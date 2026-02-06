using System.Linq;
using System.Windows;
using System.Windows.Controls;
using pr14_Cinema.Data;

namespace pr14_Cinema.Views
{
    public partial class LoginPage : Page
    {
        public MainWindow _mainWindow;

        public LoginPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        public void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // Валидация
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Введите имя пользователя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                UsernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Focus();
                return;
            }

            try
            {
                using (var context = new CinemaDbContext())
                {
                    // Поиск пользователя
                    var user = context.Users
                        .FirstOrDefault(u => u.Username == username && u.Password == password);

                    if (user != null)
                    {
                        // Сохраняем данные пользователя
                        MainWindow.CurrentUserId = user.Id;
                        MainWindow.CurrentUserName = user.FullName;

                        _mainWindow.UpdateUserInfo();
                        MessageBox.Show($"Добро пожаловать, {user.FullName}!", "Успешный вход",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Возврат на главную
                        _mainWindow.HomeBtn_Click(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Неверное имя пользователя или пароль", "Ошибка входа",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.RegisterBtn_Click(null, null);
        }

        public void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.HomeBtn_Click(null, null);
        }
    }
}