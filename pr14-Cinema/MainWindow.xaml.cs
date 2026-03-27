using pr14_Cinema.Data;
using pr14_Cinema.Models;
using pr14_Cinema.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;

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
           try
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
            }
        }
        private void CreateDatabaseIfNotExists()
        {
            try
            {
                using (var context = new CinemaDbContext())
                {
                    // ЭТА СТРОКА СОЗДАСТ ВСЕ ТАБЛИЦЫ
                    bool dbExists = context.Database.Exists();

                    if (!dbExists)
                    {
                        MessageBox.Show("Создаю базу данных...", "Информация");

                        // Создаем базу и все таблицы
                        context.Database.Create();

                        // Добавляем тестовые данные
                        AddTestData(context);

                        MessageBox.Show("База данных создана успешно! Таблицы: Movies, Halls, Sessions, Users, Seats, Tickets",
                                      "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("База данных уже существует.", "Информация");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания БД: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTestData(CinemaDbContext context)
        {
            // Простые тестовые данные для проверки
            if (!context.Movies.Any())
            {
                var movie = new Movie
                {
                    Title = "Тестовый фильм",
                    Description = "Описание тестового фильма",
                    PosterUrl = "https://via.placeholder.com/300x450",
                    Rating = 7.5,
                    ReleaseDate = DateTime.Now.AddDays(-30),
                    AgeRating = "12+",
                    Duration = 120,
                    Genre = "Тест"
                };
                context.Movies.Add(movie);
            }

            if (!context.Users.Any())
            {
                var user = new User
                {
                    Username = "admin",
                    Password = "123",
                    Email = "admin@test.ru",
                    FullName = "Администратор"
                };
                context.Users.Add(user);
            }

            context.SaveChanges();
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
                    Title = "Зеленый слоник",
                    Description = "История дружбы ВЕЛИКОЙ ДРУЖБЫ",
                    PosterUrl = "https://via.placeholder.com/300x450/00FF00/000000?text=Зеленый+слоник",
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
        public void SeedTestData(CinemaDbContext context)
        {
            // ====================== 1. Фильмы (если ещё нет) ======================
            if (!context.Movies.Any())
            {
                var movies = new[]
                {
            new Movie
            {
                Title = "Интерстеллар",
                Description = "Фантастический эпос о путешествии через червоточину",
                PosterUrl = "Images/Posters/interstellar.jpg",   // позже заменишь на свои
                Rating = 8.6,
                ReleaseDate = new DateTime(2014, 11, 6),
                AgeRating = "12+",
                Duration = 169,
                Genre = "Фантастика, Драма, Приключения"
            },
            new Movie
            {
                Title = "Криминальное чтиво",
                Description = "Культовый фильм Квентина Тарантино",
                PosterUrl = "Images/Posters/pulp_fiction.jpg",
                Rating = 8.9,
                ReleaseDate = new DateTime(1994, 10, 14),
                AgeRating = "18+",
                Duration = 154,
                Genre = "Криминал, Драма"
            },
            new Movie
            {
                Title = "Дюна: Часть вторая",
                Description = "Продолжение эпической саги Дени Вильнёва",
                PosterUrl = "Images/Posters/dune2.jpg",
                Rating = 8.7,
                ReleaseDate = DateTime.Now.AddMonths(-2),
                AgeRating = "16+",
                Duration = 166,
                Genre = "Фантастика, Приключения, Драма"
            }
        };

                context.Movies.AddRange(movies);
                context.SaveChanges();
            }

            // ====================== 2. Залы ======================
            if (!context.Halls.Any())
            {
                var halls = new[]
                {
            new Hall
            {
                Name = "Зал 1 (Standard)",
                Capacity = 96,
                Type = "Standard",
                Description = "Малый зал для комфортного просмотра"
            },
            new Hall
            {
                Name = "Зал 2 (Standard)",
                Capacity = 150,
                Type = "Standard",
                Description = "Основной зал"
            },
            new Hall
            {
                Name = "Зал 3 (VIP)",
                Capacity = 168,
                Type = "VIP",
                Description = "Зал повышенного комфорта с креслами VIP"
            },
            new Hall
            {
                Name = "Зал 4 (IMAX)",
                Capacity = 252,
                Type = "IMAX",
                Description = "Большой зал с современным оборудованием"
            }
        };

                context.Halls.AddRange(halls);
                context.SaveChanges();

                // ====================== 3. Создание мест для каждого зала ======================
                foreach (var hall in halls)
                {
                    List<Seat> seats = new List<Seat>();
                    int rows = 0;
                    int seatsPerRow = 0;

                    switch (hall.Name)
                    {
                        case "Зал 1 (Standard)":
                            rows = 8;
                            seatsPerRow = 12;
                            break;
                        case "Зал 2 (Standard)":
                            rows = 10;
                            seatsPerRow = 15;
                            break;
                        case "Зал 3 (VIP)":
                            rows = 12;
                            seatsPerRow = 14;
                            break;
                        case "Зал 4 (IMAX)":
                            rows = 14;
                            seatsPerRow = 18;
                            break;
                    }

                    for (int row = 1; row <= rows; row++)
                    {
                        for (int num = 1; num <= seatsPerRow; num++)
                        {
                            string type = "Standard";

                            // VIP места в последних рядах VIP-зала
                            if (hall.Type == "VIP" && row >= 9)
                                type = "VIP";

                            // Места для инвалидов (например, в первом ряду)
                            if (row == 1 && (num == 1 || num == 2))
                                type = "Disabled";

                            seats.Add(new Seat
                            {
                                HallId = hall.Id,
                                Row = row,
                                Number = num,
                                Type = type
                            });
                        }
                    }

                    context.Seats.AddRange(seats);
                }

                context.SaveChanges();
                MessageBox.Show($"Создано 4 зала и {context.Seats.Count()} мест!", "Тестовые данные",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
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