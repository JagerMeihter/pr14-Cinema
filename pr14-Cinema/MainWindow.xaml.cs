using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using pr14_Cinema.Data;
using pr14_Cinema.Views;

namespace pr14_Cinema
{
    public partial class MainWindow : Window
    {
        public static int? CurrentUserId { get; set; }
        public static string CurrentUserName { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeDatabase();

            // Показать главную страницу
            HomeBtn_Click(null, null);

            // Обновить информацию о пользователе
            UpdateUserInfo();
        }

        public void InitializeDatabase()
        {
           /* try
            {
                using (var context = new CinemaDbContext())
                {
                    context.Database.CreateIfNotExists();

                    // Добавляем тестовые данные если база пуста
                    if (!context.Movies.Any())
                    {
                        SeedTestData(context);
                        MessageBox.Show("Тестовые данные загружены!", "Информация",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка БД: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
        }

        public void SeedTestData(CinemaDbContext context)
        {
            // Тестовые фильмы
            var movies = new[]
            {
                new Models.Movie
                {
                    Title = "Интерстеллар",
                    Description = "Фантастический эпос о путешествии через червоточину",
                    PosterUrl = "https://via.placeholder.com/300x450/0000FF/FFFFFF?text=Интерстеллар",
                    Rating = 8.6,
                    ReleaseDate = new DateTime(2014, 11, 6),
                    AgeRating = "12+",
                    Duration = 169,
                    Genre = "Фантастика, Драма, Приключения"
                },
                new Models.Movie
                {
                    Title = "Криминальное чтиво",
                    Description = "Культовый фильм Квентина Тарантино",
                    PosterUrl = "https://via.placeholder.com/300x450/FF0000/FFFFFF?text=Криминальное+чтиво",
                    Rating = 8.9,
                    ReleaseDate = new DateTime(1994, 10, 14),
                    AgeRating = "18+",
                    Duration = 154,
                    Genre = "Криминал, Драма"
                },
                new Models.Movie
                {
                    Title = "Зеленая книга",
                    Description = "История дружбы пианиста и водителя",
                    PosterUrl = "https://via.placeholder.com/300x450/00FF00/000000?text=Зеленая+книга",
                    Rating = 8.2,
                    ReleaseDate = new DateTime(2018, 11, 16),
                    AgeRating = "12+",
                    Duration = 130,
                    Genre = "Комедия, Драма, Биография"
                }
            };

            context.Movies.AddRange(movies);
            context.SaveChanges();
        }

        public void UpdateUserInfo()
        {
            if (CurrentUserId.HasValue)
            {
                UserInfoText.Text = CurrentUserName;
                LoginBtn.Visibility = Visibility.Collapsed;
                RegisterBtn.Visibility = Visibility.Collapsed;
                ProfileBtn.Visibility = Visibility.Visible;
            }
            else
            {
                UserInfoText.Text = "Гость";
                LoginBtn.Visibility = Visibility.Visible;
                RegisterBtn.Visibility = Visibility.Visible;
                ProfileBtn.Visibility = Visibility.Collapsed;
            }
        }

        public void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MainPage(this));
        }

        public void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUserId.HasValue)
            {
                MainFrame.Navigate(new ProfilePage(this));
            }
            else
            {
                MessageBox.Show("Войдите в систему", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoginPage(this));
        }

        public void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RegisterPage(this));
        }
    }
}