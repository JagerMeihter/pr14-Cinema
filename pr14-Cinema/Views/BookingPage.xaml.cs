using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using pr14_Cinema.Data;
using pr14_Cinema.Models;

namespace pr14_Cinema.Views
{
    public partial class BookingPage : Page
    {
        private MainWindow _mainWindow;
        private int _sessionId;
        private List<SeatViewModel> _selectedSeats;
        private Session _session;
        private User _currentUser;

        public class BookingSeat
        {
            public int Row { get; set; }
            public int Number { get; set; }
            public string Type { get; set; }
            public decimal Price { get; set; }
        }

        public BookingPage(MainWindow mainWindow, int sessionId, List<SessionPage.SeatViewModel> selectedSeats)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _sessionId = sessionId;

            // Конвертируем в BookingSeat
            _selectedSeats = selectedSeats.Select(s => new SeatViewModel
            {
                Row = s.Row,
                Number = s.Number,
                Type = s.Type
            }).ToList();

            LoadBookingData();
        }

        private void LoadBookingData()
        {
            try
            {
                using (var context = new CinemaDbContext())
                {
                    // Загружаем данные сеанса
                    _session = context.Sessions
                        .Include(s => s.Movie)
                        .Include(s => s.Hall)
                        .FirstOrDefault(s => s.Id == _sessionId);

                    if (_session == null)
                    {
                        MessageBox.Show("Сеанс не найден", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        _mainWindow.HomeBtn_Click(null, null);
                        return;
                    }

                    // Загружаем данные пользователя
                    if (MainWindow.CurrentUserId.HasValue)
                    {
                        _currentUser = context.Users
                            .FirstOrDefault(u => u.Id == MainWindow.CurrentUserId.Value);
                    }

                    // Отображаем информацию
                    DisplayBookingInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayBookingInfo()
        {
            // Информация о сеансе
            MovieText.Text = _session.Movie.Title;
            DateTimeText.Text = _session.DateTime.ToString("dd.MM.yyyy HH:mm");
            HallText.Text = $"{_session.Hall.Name} ({_session.Hall.Type})";
            TicketPriceText.Text = $"{_session.TicketPrice}₽";

            // Выбранные места с ценами
            var bookingSeats = new List<BookingSeat>();
            decimal totalPrice = 0;

            foreach (var seat in _selectedSeats)
            {
                decimal price = seat.Type == "VIP" ?
                    _session.TicketPrice * 1.5m : _session.TicketPrice;

                bookingSeats.Add(new BookingSeat
                {
                    Row = seat.Row,
                    Number = seat.Number,
                    Type = seat.Type,
                    Price = price
                });

                totalPrice += price;
            }

            SelectedSeatsList.ItemsSource = bookingSeats;
            TotalPriceText.Text = $"{totalPrice}₽";

            // Информация о пользователе
            if (_currentUser != null)
            {
                UserNameText.Text = _currentUser.FullName;
                UserEmailText.Text = _currentUser.Email;
                UserPhoneText.Text = _currentUser.Phone ?? "Не указан";
            }
            else
            {
                UserNameText.Text = "Гость";
                UserEmailText.Text = "Не указан";
                UserPhoneText.Text = "Не указан";
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.CurrentUserId.HasValue)
            {
                MessageBox.Show("Для оформления билета необходимо войти в систему",
                    "Требуется вход", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new CinemaDbContext())
                {
                    // Получаем реальные ID мест из базы
                    var realSeats = new List<Seat>();
                    foreach (var seatVM in _selectedSeats)
                    {
                        var seat = context.Seats
                            .FirstOrDefault(s => s.HallId == _session.HallId &&
                                                s.Row == seatVM.Row &&
                                                s.Number == seatVM.Number);
                        if (seat != null)
                        {
                            realSeats.Add(seat);
                        }
                    }

                    // Создаем билеты
                    foreach (var seat in realSeats)
                    {
                        decimal price = seat.Type == "VIP" ?
                            _session.TicketPrice * 1.5m : _session.TicketPrice;

                        var ticket = new Ticket
                        {
                            UserId = MainWindow.CurrentUserId.Value,
                            SessionId = _sessionId,
                            SeatId = seat.Id,
                            PurchaseDate = DateTime.Now,
                            Price = price,
                            Status = "Active"
                        };

                        context.Tickets.Add(ticket);
                    }

                    context.SaveChanges();

                    // Показываем подтверждение
                    MessageBox.Show($"Билеты успешно оформлены!\n" +
                                   $"Количество билетов: {_selectedSeats.Count}\n" +
                                   $"Общая сумма: {TotalPriceText.Text}\n" +
                                   $"Номера мест: {string.Join(", ", _selectedSeats.Select(s => $"{s.Row}-{s.Number}"))}",
                                   "Успешное оформление", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Возвращаемся на главную
                    _mainWindow.HomeBtn_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении билетов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.GoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.GoBack();
        }

        // Вспомогательный класс для отображения мест
        public class SeatViewModel
        {
            public int Row { get; set; }
            public int Number { get; set; }
            public string Type { get; set; }
        }
    }
}