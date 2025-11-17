using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace F1MUniverse
{
    public partial class MainWindow : Window
    {
        // Simple model for one row in the standings table
        private class StandingRow
        {
            public int Position { get; set; }
            public string Driver { get; set; } = "";
            public string Team { get; set; } = "";
            public int Points { get; set; }
        }

        // Sample data for now: (Series, Season) -> list of rows
        private readonly Dictionary<(string Series, int Season), List<StandingRow>> _sampleStandings
            = new();

        public MainWindow()
        {
            InitializeComponent();

            InitializeSampleData();
            ShowDashboard();
        }

        private void InitializeSampleData()
        {
            // F1 2025 sample
            _sampleStandings[("F1", 2025)] = new List<StandingRow>
            {
                new() { Position = 1, Driver = "Max Verstappen", Team = "Red Bull Racing", Points = 395 },
                new() { Position = 2, Driver = "Lando Norris",    Team = "McLaren",        Points = 325 },
                new() { Position = 3, Driver = "Charles Leclerc", Team = "Ferrari",        Points = 310 },
                new() { Position = 4, Driver = "Oscar Piastri",   Team = "McLaren",        Points = 280 },
                new() { Position = 5, Driver = "George Russell",  Team = "Mercedes",       Points = 245 },
            };

            // F2 2025 sample
            _sampleStandings[("F2", 2025)] = new List<StandingRow>
            {
                new() { Position = 1, Driver = "Andrea Kimi Antonelli", Team = "Prema",   Points = 230 },
                new() { Position = 2, Driver = "Gabriel Bortoleto",     Team = "Invicta", Points = 210 },
                new() { Position = 3, Driver = "Isack Hadjar",          Team = "Campos",  Points = 198 },
                new() { Position = 4, Driver = "Oliver Bearman",        Team = "Prema",   Points = 184 },
                new() { Position = 5, Driver = "Pepe Martí",            Team = "MP",      Points = 170 },
            };

            // F3 2025 sample
            _sampleStandings[("F3", 2025)] = new List<StandingRow>
            {
                new() { Position = 1, Driver = "T. Naël",           Team = "Prema",    Points = 210 },
                new() { Position = 2, Driver = "M. Boya",           Team = "Campos",   Points = 195 },
                new() { Position = 3, Driver = "N. León",           Team = "MP",       Points = 186 },
                new() { Position = 4, Driver = "U. Ugochukwu",      Team = "Prema",    Points = 175 },
                new() { Position = 5, Driver = "C. Stenshorne",     Team = "Hitech",   Points = 160 },
            };

            // You can add more seasons later:
            // _sampleStandings[("F1", 2024)] = new List<StandingRow> { ... };
        }

        // ---- Navigation helpers ----

        private void ShowDashboard()
        {
            TopTitleText.Text = "Dashboard";
            ContentTitleText.Text = "Welcome!";
            ContentBodyText.Text = "Choose a section on the left to get started (Standings or Contracts).";

            StandingsFilterPanel.Visibility = Visibility.Collapsed;
            ContentHost.Content = null;
        }

        private void StandingsButton_Click(object sender, RoutedEventArgs e)
        {
            TopTitleText.Text = "Standings";
            ContentTitleText.Text = "Standings";
            ContentBodyText.Text = "Example data – later this will come from your F1/F2/F3 save/database.";

            StandingsFilterPanel.Visibility = Visibility.Visible;

            LoadStandings();
        }

        private void ContractsButton_Click(object sender, RoutedEventArgs e)
        {
            TopTitleText.Text = "Contracts";
            ContentTitleText.Text = "Contracts";
            ContentBodyText.Text = "Later we will show all driver and staff contracts here.";

            StandingsFilterPanel.Visibility = Visibility.Collapsed;
            ContentHost.Content = null;
        }

        // ---- Filters changed ----

        private void SeriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Only reload if we are on the Standings screen
            if (TopTitleText.Text == "Standings")
            {
                LoadStandings();
            }
        }

        private void SeasonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TopTitleText.Text == "Standings")
            {
                LoadStandings();
            }
        }

        // ---- Standings table ----

        private void LoadStandings()
        {
            // Read selected series (F1/F2/F3)
            if (SeriesComboBox.SelectedItem is not ComboBoxItem seriesItem ||
                SeasonComboBox.SelectedItem is not ComboBoxItem seasonItem)
            {
                return;
            }

            string series = seriesItem.Content.ToString() ?? "F1";
            if (!int.TryParse(seasonItem.Content.ToString(), out int season))
            {
                season = 2025;
            }

            // Get data for this (series, season)
            if (!_sampleStandings.TryGetValue((series, season), out var rows))
            {
                ContentHost.Content = new TextBlock
                {
                    Text = $"No sample data yet for {series} {season}.",
                    Foreground = System.Windows.Media.Brushes.White,
                    Margin = new Thickness(0, 8, 0, 0)
                };
                return;
            }

            // Build the DataGrid
            var grid = new DataGrid
            {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                Margin = new Thickness(0, 8, 0, 0),
                GridLinesVisibility = DataGridGridLinesVisibility.None,
                RowBackground = System.Windows.Media.Brushes.Transparent,
                AlternatingRowBackground = System.Windows.Media.Brushes.Transparent,
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = System.Windows.Media.Brushes.White
            };

            grid.Columns.Add(new DataGridTextColumn
            {
                Header = "#",
                Binding = new Binding(nameof(StandingRow.Position)),
                Width = 40
            });

            grid.Columns.Add(new DataGridTextColumn
            {
                Header = "Driver",
                Binding = new Binding(nameof(StandingRow.Driver)),
                Width = new DataGridLength(2, DataGridLengthUnitType.Star)
            });

            grid.Columns.Add(new DataGridTextColumn
            {
                Header = "Team",
                Binding = new Binding(nameof(StandingRow.Team)),
                Width = new DataGridLength(2, DataGridLengthUnitType.Star)
            });

            grid.Columns.Add(new DataGridTextColumn
            {
                Header = "Points",
                Binding = new Binding(nameof(StandingRow.Points)),
                Width = 80
            });

            grid.ItemsSource = rows;

            ContentHost.Content = grid;
        }
    }
}
