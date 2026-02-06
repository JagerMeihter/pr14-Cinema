using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using pr14_Cinema.Data;
using pr14_Cinema.Models;

namespace pr14_Cinema.Views
{
    public partial class ProfilePage : Page
    {
        private MainWindow _mainWindow;
        private User _currentUser;
        private List<TicketViewModel> _allTickets;

        public ProfilePage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            try
            {
                using (var context = new CinemaDbContext())
                {
                    if (!MainWindow.CurrentUserId.HasValue)
                    {
                        MessageBox.Show("Вы не авторизованы", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        _mainWindow.HomeBtn_Click(null, null);
                        return;
                    }

                    // Загружаем данные пользователя
                    _currentUser = context.Users
                        .FirstOrDefault(u => u.Id == MainWindow.CurrentUserId.Value);

                    if (_currentUser == null)
                    {
                        MessageBox.Show("Пользователь не найден", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        _mainWindow.LoginBtn_Click(null, null);
                        return;
                    }

                    // Отображаем информацию
                    DisplayUserInfo();

                    // Загружаем билеты
                    LoadUserTickets(context);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки профиля: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayUserInfo()
        {
            UserNameText.Text = _currentUser.FullName;
            UserEmailText.Text = _currentUser.Email;
            UserPhoneText.Text = string.IsNullOrWhiteSpace(_currentUser.Phone) ?
                "Не указан" : _currentUser.Phone;

            // В реальном проекте здесь была бы дата регистрации
            RegistrationDateText.Text = DateTime.Now.ToString("dd.MM.yyyy");
        }

        private void LoadUserTickets(CinemaDbContext context)
        {
            var tickets = context.Tickets
                .Include(t => t.Session.Movie)
                .Include(t => t.Session.Hall)
                .Include(t => t.Seat)
                .Where(t => t.UserId == _currentUser.Id)
                .ToList()
                .Select(t => new TicketViewModel
                {
                    Id = t.Id,
                    MovieTitle = t.Session.Movie.Title,
                    SessionDateTime = t.Session.DateTime,
                    HallName = t.Session.Hall.Name,
                    SeatInfo = $"Ряд {t.Seat.Row}, Место {t.Seat.Number} ({t.Seat.Type})",
                    Price = t.Price,
                    Status = GetStatusText(t.Status),
                    PurchaseDate = t.PurchaseDate,
                    CanCancel = t.Status == "Active" && t.Session.DateTime > DateTime.Now.AddHours(1)
                })
                .ToList();

            _allTickets = tickets;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allTickets == null) return;

            var filteredTickets = _allTickets.AsEnumerable();

            // Фильтр по статусу
            switch (StatusFilterComboBox.SelectedIndex)
            {
                case 1: // Активные
                    filteredTickets = filteredTickets.Where(t => t.Status == "Активный");
                    break;
                case 2: // Использованные
                    filteredTickets = filteredTickets.Where(t => t.Status == "Использованный");
                    break;
                case 3: // Отмененные
                    filteredTickets = filteredTickets.Where(t => t.Status == "Отмененный");
                    break;
            }

            // Сортировка
            switch (SortTicketsComboBox.SelectedIndex)
            {
                case 0: // По дате (новые)
                    filteredTickets = filteredTickets.OrderByDescending(t => t.SessionDateTime);
                    break;
                case 1: // По дате (старые)
                    filteredTickets = filteredTickets.OrderBy(t => t.SessionDateTime);
                    break;
                case 2: // По цене (дорогие)
                    filteredTickets = filteredTickets.OrderByDescending(t => t.Price);
                    break;
                case 3: // По цене (дешевые)
                    filteredTickets = filteredTickets.OrderBy(t => t.Price);
                    break;
                default:
                    filteredTickets = filteredTickets.OrderByDescending(t => t.SessionDateTime);
                    break;
            }

            var ticketsList = filteredTickets.ToList();
            TicketsList.ItemsSource = ticketsList;

            // Статистика
            TotalTicketsText.Text = _allTickets.Count.ToString();
            TotalSpentText.Text = _allTickets.Sum(t => t.Price).ToString("N0") + "₽";

            // Показываем сообщение если нет билетов
            NoTicketsText.Visibility = ticketsList.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        private string GetStatusText(string status)
        {
            switch (status)
            {
                case "Active":
                    return "Активный";
                case "Used":
                    return "Использованный";
                case "Cancelled":
                    return "Отмененный";
                default:
                    return status;
            }
        }

        public void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        public void SortTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        public void CancelTicket_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int ticketId = (int)button.Tag;

            var result = MessageBox.Show("Вы уверены, что хотите отменить этот билет?",
                "Подтверждение отмены", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new CinemaDbContext())
                    {
                        var ticket = context.Tickets.Find(ticketId);
                        if (ticket != null && ticket.Status == "Active")
                        {
                            ticket.Status = "Cancelled";
                            context.SaveChanges();

                            MessageBox.Show("Билет успешно отменен", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                            // Обновляем список
                            LoadUserProfile();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отмене билета: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CurrentUserId = null;
            MainWindow.CurrentUserName = null;

            _mainWindow.UpdateUserInfo();
            _mainWindow.HomeBtn_Click(null, null);

            MessageBox.Show("Вы успешно вышли из системы", "Выход",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.HomeBtn_Click(null, null);
        }

        // Вспомогательный класс для отображения билетов
        public class TicketViewModel
        {
            public int Id { get; set; }
            public string MovieTitle { get; set; }
            public DateTime SessionDateTime { get; set; }
            public string HallName { get; set; }
            public string SeatInfo { get; set; }
            public decimal Price { get; set; }
            public string Status { get; set; }
            public DateTime PurchaseDate { get; set; }
            public bool CanCancel { get; set; }
        }

        // Converter для Visibility
        public class BoolToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}