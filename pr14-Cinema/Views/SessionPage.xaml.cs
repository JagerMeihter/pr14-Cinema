using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using pr14_Cinema.Data;
using pr14_Cinema.Models;

namespace pr14_Cinema.Views
{
    public partial class SessionPage : Page
    {
        private MainWindow _mainWindow;
        private int _sessionId;
        private Session _session;
        private ObservableCollection<SeatViewModel> _seats;
        private ObservableCollection<SeatViewModel> _selectedSeats;

        public SessionPage(MainWindow mainWindow, int sessionId)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _sessionId = sessionId;
            _selectedSeats = new ObservableCollection<SeatViewModel>();
            SelectedSeatsList.ItemsSource = _selectedSeats;

            LoadSessionData();
        }

        public void LoadSessionData()
        {
            try
            {
                using (var context = new CinemaDbContext())
                {
                    _session = context.Sessions
                        .Include(s => s.Movie)
                        .Include(s => s.Hall)
                        .Include(s => s.Tickets)
                        .Include(s => s.AvailableSeats)
                        .FirstOrDefault(s => s.Id == _sessionId);

                    if (_session == null)
                    {
                        MessageBox.Show("Сеанс не найден", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        _mainWindow.HomeBtn_Click(null, null);
                        return;
                    }

                    // Отображаем информацию о сеансе
                    SessionInfoText.Text = $"{_session.Movie.Title} - {_session.DateTime:dd.MM.yyyy HH:mm} - {_session.Hall.Name} ({_session.Hall.Type})";
                    PriceText.Text = $"Цена билета: {_session.TicketPrice}₽";

                    // Загружаем места в зале
                    LoadSeats(context);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных сеанса: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadSeats(CinemaDbContext context)
        {
            SeatViewModel seatVM = new SeatViewModel();

            
            _seats = new ObservableCollection<SeatViewModel>();

            // Получаем все места в зале
            var hallSeats = context.Seats
                .Where(s => s.HallId == _session.HallId)
                .OrderBy(s => s.Row)
                .ThenBy(s => s.Number)
                .ToList();

            // Получаем занятые места на этот сеанс
            var bookedSeatIds = context.Tickets
                .Where(t => t.SessionId == _sessionId && t.Status == "Active")
                .Select(t => t.SeatId)
                .ToList();

            foreach (var seat in hallSeats)
            {

                seatVM.IsSelected = true;

                _seats.Add(seatVM);
            }

            SeatsGrid.ItemsSource = _seats;
        }
        public void SeatBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var seat = border.DataContext as SeatViewModel;
            if (seat != null)
            {
                ToggleSeatSelection(seat);
            }
        }

        public void ToggleSeatSelection(SeatViewModel seat)
        {
            if (seat.IsBooked || seat.Type == "Disabled")
                return;

            seat.IsSelected = !seat.IsSelected;

            if (seat.IsSelected)
            {
                _selectedSeats.Add(seat);
            }
            else
            {
                _selectedSeats.Remove(seat);
            }

            UpdateTotalPrice();
            UpdateContinueButton();
        }

        public void UpdateTotalPrice()
        {
            decimal totalPrice = _selectedSeats.Sum(s =>
                s.Type == "VIP" ? _session.TicketPrice * 1.5m : _session.TicketPrice);

            TotalPriceText.Text = $"Итого: {totalPrice}₽";
        }

        public void UpdateContinueButton()
        {
            var continueButton = FindName("ContinueBookingButton") as Button;
            if (continueButton != null)
            {
                continueButton.IsEnabled = _selectedSeats.Any();
                continueButton.Content = _selectedSeats.Any() ?
                    $"Оформить {_selectedSeats.Count} билет(ов)" :
                    "Продолжить оформление";
            }
        }

        public void Seat_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null && border.DataContext is SeatViewModel seat &&
                !seat.IsBooked && seat.Type != "Disabled" && !seat.IsSelected)
            {
                border.Background = System.Windows.Media.Brushes.LightBlue;
            }
        }

        public void Seat_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null && border.DataContext is SeatViewModel seat &&
                !seat.IsBooked && seat.Type != "Disabled" && !seat.IsSelected)
            {
                border.Background = seat.Type == "VIP" ?
                    System.Windows.Media.Brushes.Gold :
                    System.Windows.Media.Brushes.WhiteSmoke;
            }
        }

        public void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            foreach (var seat in _selectedSeats.ToList())
            {
                seat.IsSelected = false;
            }
            _selectedSeats.Clear();
            UpdateTotalPrice();
            UpdateContinueButton();
        }

        public void ContinueBooking_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSeats.Any())
            {
                // Переходим на страницу оформления
                _mainWindow.MainFrame.Navigate(new BookingPage(
                    _mainWindow,
                    _sessionId,
                    _selectedSeats.ToList()));
            }
        }

        public void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.GoBack();
        }

        // Вспомогательный класс для отображения мест
        public class SeatViewModel
        {
            public int Id { get; set; }
            public int Row { get; set; }
            public int Number { get; set; }
            public string Type { get; set; }
            public bool IsBooked { get; set; }
            public bool IsSelected { get; set; }
            public ICommand SelectCommand { get; set; }
        }

        // Команда для RelayCommand
        public class RelayCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public RelayCommand(Action execute, Func<bool> canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
            public void Execute(object parameter) => _execute();
        }
    }
}