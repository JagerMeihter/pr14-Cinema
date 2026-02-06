using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using pr14_Cinema.Data;

namespace pr14_Cinema.Views
{
    public partial class MoviePage : Page
    {
        private MainWindow _mainWindow;
        private int _movieId;

        public MoviePage(MainWindow mainWindow, int movieId)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _movieId = movieId;
            LoadMovieData();
        }

        private void LoadMovieData()
        {
            try
            {
                using (var context = new CinemaDbContext())
                {
                    var movie = context.Movies
                        .Include(m => m.Sessions.Select(s => s.Hall))
                        .FirstOrDefault(m => m.Id == _movieId);

                    if (movie == null)
                    {
                        MessageBox.Show("Фильм не найден", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        _mainWindow.HomeBtn_Click(null, null);
                        return;
                    }

                    // Заполняем информацию о фильме
                    TitleText.Text = movie.Title;
                    RatingText.Text = movie.Rating.ToString("F1");
                    ReleaseDateText.Text = movie.ReleaseDate.ToString("dd.MM.yyyy");
                    AgeRatingText.Text = movie.AgeRating;
                    DurationText.Text = $"{movie.Duration} мин";
                    GenreText.Text = movie.Genre;
                    DescriptionText.Text = movie.Description;

                    // Загружаем постер
                    PosterImage.Source = new System.Windows.Media.Imaging.BitmapImage(
                        new Uri(movie.PosterUrl, UriKind.RelativeOrAbsolute));

                    // Загружаем сеансы
                    var upcomingSessions = movie.Sessions
                        .Where(s => s.DateTime > DateTime.Now)
                        .OrderBy(s => s.DateTime)
                        .ToList();

                    SessionsList.ItemsSource = upcomingSessions;

                    // Показываем сообщение если нет сеансов
                    NoSessionsText.Visibility =
                        upcomingSessions.Any() ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных фильма: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectSession_Click(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.CurrentUserId.HasValue)
            {
                MessageBox.Show("Для выбора мест необходимо войти в систему", "Требуется вход",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                _mainWindow.LoginBtn_Click(null, null);
                return;
            }

            var button = (Button)sender;
            int sessionId = (int)button.Tag;

            _mainWindow.MainFrame.Navigate(new SessionPage(_mainWindow, sessionId));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.HomeBtn_Click(null, null);
        }
    }
}