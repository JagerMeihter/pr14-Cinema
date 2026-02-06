using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using pr14_Cinema.Data;
using pr14_Cinema.Models;

namespace pr14_Cinema.Views
{
    public partial class RegisterPage : Page
    {
        private MainWindow _mainWindow;

        public RegisterPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        public void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем данные
            string fullName = FullNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Валидация
            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Введите ФИО", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                FullNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Введите Email", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return;
            }

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

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                PasswordBox.Focus();
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Focus();
                return;
            }

            try
            {
                using (var context = new CinemaDbContext())
                {
                    // Проверяем, не занят ли username
                    if (context.Users.Any(u => u.Username == username))
                    {
                        MessageBox.Show("Это имя пользователя уже занято", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        UsernameTextBox.Focus();
                        return;
                    }

                    // Проверяем, не занят ли email
                    if (context.Users.Any(u => u.Email == email))
                    {
                        MessageBox.Show("Этот Email уже зарегистрирован", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        EmailTextBox.Focus();
                        return;
                    }

                    // Создаем нового пользователя
                    var newUser = new User
                    {
                        FullName = fullName,
                        Email = email,
                        Phone = phone,
                        Username = username,
                        Password = password
                    };

                    context.Users.Add(newUser);
                    context.SaveChanges();

                    MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти.", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Переходим на страницу входа
                    _mainWindow.LoginBtn_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoginLink_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.LoginBtn_Click(null, null);
        }

        public void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.HomeBtn_Click(null, null);
        }
    }
}