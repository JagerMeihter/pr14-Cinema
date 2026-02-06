using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using pr14_Cinema.Data;

namespace pr14_Cinema.Views
{
    public partial class MainPage : Page
    {
        public MainWindow _mainWindow;

        public MainPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadMovies();
            LoadGenres();
        }

        public void LoadMovies()
        {
            try
            {
                using (var context = new CinemaDbContext())
                {
                    var movies = context.Movies
                        .OrderByDescending(m => m.Rating)
                        .ToList();

                    MoviesList.ItemsSource = movies;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки фильмов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadGenres()
        {
            try
            {
                using (var context = new CinemaDbContext())
                {
                    var allGenres = context.Movies
                        .Select(m => m.Genre)
                        .ToList()
                        .SelectMany(g => g.Split(','))
                        .Select(g => g.Trim())
                        .Distinct()
                        .OrderBy(g => g)
                        .ToList();

                    GenreComboBox.Items.Add("Все жанры");
                    foreach (var genre in allGenres)
                    {
                        GenreComboBox.Items.Add(genre);
                    }
                    GenreComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки жанров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        public void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        public void AgeRatingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        public void GenreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        public void ApplyFilters()
        {
            try
            {
                using (var context = new CinemaDbContext())
                {
                    var query = context.Movies.AsQueryable();

                    // Поиск по названию
                    if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
                    {
                        query = query.Where(m => m.Title.Contains(SearchTextBox.Text));
                    }

                    // Фильтр по возрастному рейтингу
                    if (AgeRatingComboBox.SelectedIndex > 0)
                    {
                        var selectedRating = (AgeRatingComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                        query = query.Where(m => m.AgeRating == selectedRating);
                    }

                    // Фильтр по жанру
                    if (GenreComboBox.SelectedIndex > 0)
                    {
                        var selectedGenre = GenreComboBox.SelectedItem.ToString();
                        query = query.Where(m => m.Genre.Contains(selectedGenre));
                    }

                    // Сортировка
                    switch (SortComboBox.SelectedIndex)
                    {
                        case 0: // По названию (А-Я)
                            query = query.OrderBy(m => m.Title);
                            break;
                        case 1: // По названию (Я-А)
                            query = query.OrderByDescending(m => m.Title);
                            break;
                        case 2: // По рейтингу (высокий)
                            query = query.OrderByDescending(m => m.Rating);
                            break;
                        case 3: // По рейтингу (низкий)
                            query = query.OrderBy(m => m.Rating);
                            break;
                        case 4: // По дате (новые)
                            query = query.OrderByDescending(m => m.ReleaseDate);
                            break;
                        case 5: // По дате (старые)
                            query = query.OrderBy(m => m.ReleaseDate);
                            break;
                        default:
                            query = query.OrderByDescending(m => m.Rating);
                            break;
                    }

                    MoviesList.ItemsSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ViewMovieDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int movieId = (int)button.Tag;

            _mainWindow.MainFrame.Navigate(new MoviePage(_mainWindow, movieId));
        }

        public void MovieItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var border = (Border)sender;
            border.BorderBrush = System.Windows.Media.Brushes.Blue;
            border.BorderThickness = new Thickness(2);
        }

        public void MovieItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var border = (Border)sender;
            border.BorderBrush = System.Windows.Media.Brushes.LightGray;
            border.BorderThickness = new Thickness(1);
        }
    }
}