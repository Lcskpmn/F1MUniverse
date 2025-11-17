using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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

        private class ContractRow
        {
            public string Driver { get; set; } = string.Empty;
            public string Team { get; set; } = string.Empty;
            public string Series { get; set; } = string.Empty;
            public int StartSeason { get; set; }
            public int EndSeason { get; set; }
        }

        // Sample data for now: (Series, Season) -> list of rows
        private readonly Dictionary<(string Series, int Season), List<StandingRow>> _sampleStandings
            = new();
        private readonly List<ContractRow> _sampleContracts = new();

        public MainWindow()
        {
            InitializeComponent();

            InitializeSampleStandings();
            InitializeSampleContracts();
            ShowDashboard();
        }

        private void InitializeSampleStandings()
        {
            // F3 2025 sample
            _sampleStandings[("F3", 2025)] = new List<StandingRow>
            {
                new() { Position = 1, Driver = "T. Naël",           Team = "Prema",    Points = 210 },
                new() { Position = 2, Driver = "M. Boya",           Team = "Campos",   Points = 195 },
                new() { Position = 3, Driver = "N. León",           Team = "MP",       Points = 186 },
                new() { Position = 4, Driver = "U. Ugochukwu",      Team = "Prema",    Points = 175 },
                new() { Position = 5, Driver = "C. Stenshorne",     Team = "Hitech",   Points = 160 },
                new() { Position = 6, Driver = "J. Edgar",          Team = "ART",      Points = 148 },
                new() { Position = 7, Driver = "A. Lindblad",       Team = "Prema",    Points = 139 },
                new() { Position = 8, Driver = "L. Browning",       Team = "Hitech",   Points = 120 },
                new() { Position = 9, Driver = "S. Flörsch",        Team = "Van Amersfoort", Points = 112 },
                new() { Position = 10, Driver = "T. Barnard",       Team = "Jenzer",   Points = 104 },
            };

            // You can add more seasons later:
            // _sampleStandings[("F1", 2024)] = new List<StandingRow> { ... };
        }

        private void InitializeSampleContracts()
        {
            _sampleContracts.AddRange(new[]
            {
                new ContractRow
                {
                    Driver = "Max Verstappen",
                    Team = "Red Bull Racing",
                    Series = "F1",
                    StartSeason = 2023,
                    EndSeason = 2028
                },
                new ContractRow
                {
                    Driver = "Lando Norris",
                    Team = "McLaren",
                    Series = "F1",
                    StartSeason = 2024,
                    EndSeason = 2026
                },
                new ContractRow
                {
                    Driver = "Oliver Bearman",
                    Team = "Prema",
                    Series = "F2",
                    StartSeason = 2024,
                    EndSeason = 2025
                },
                new ContractRow
                {
                    Driver = "Ugo Ugochukwu",
                    Team = "Prema",
                    Series = "F3",
                    StartSeason = 2023,
                    EndSeason = 2024
                },
                new ContractRow
                {
                    Driver = "Gabriel Bortoleto",
                    Team = "Invicta Virtuosi",
                    Series = "F2",
                    StartSeason = 2023,
                    EndSeason = 2024
                },
                new ContractRow
                {
                    Driver = "Sophia Flörsch",
                    Team = "Van Amersfoort",
                    Series = "F3",
                    StartSeason = 2024,
                    EndSeason = 2025
                }
            });
        }

        // ---- Navigation helpers ----

        private void ShowDashboard()
        {
            TopTitleText.Text = "Dashboard";
            ContentTitleText.Text = "Welcome!";
            ContentBodyText.Text = "Choose a section on the left to get started (Standings or Contracts).";

            StandingsFilterPanel.Visibility = Visibility.Collapsed;
            StandingsDataGrid.Visibility = Visibility.Collapsed;
            StandingsEmptyStateText.Visibility = Visibility.Collapsed;
            ContractsFilterPanel.Visibility = Visibility.Collapsed;
            ContractsGrid.Visibility = Visibility.Collapsed;
            ContractsEmptyStateText.Visibility = Visibility.Collapsed;
        }

        private void StandingsButton_Click(object sender, RoutedEventArgs e)
        {
            TopTitleText.Text = "Standings";
            ContentTitleText.Text = "F3 2025 Standings";
            ContentBodyText.Text = "Using the design doc example data so you can preview how the standings layout will look.";

            StandingsFilterPanel.Visibility = Visibility.Visible;

            ContractsFilterPanel.Visibility = Visibility.Collapsed;
            ContractsGrid.Visibility = Visibility.Collapsed;
            ContractsEmptyStateText.Visibility = Visibility.Collapsed;

            // Default the filters to the dataset defined in the design doc
            SeriesComboBox.SelectedIndex = 2; // F3
            SeasonComboBox.SelectedIndex = 1; // 2025

            LoadStandings();
        }

        private void ContractsButton_Click(object sender, RoutedEventArgs e)
        {
            TopTitleText.Text = "Contracts";
            ContentTitleText.Text = "Contracts";
            ContentBodyText.Text = "Preview the layout that will be used to review and edit driver/staff contracts across F1, F2 and F3.";

            StandingsFilterPanel.Visibility = Visibility.Collapsed;
            StandingsDataGrid.Visibility = Visibility.Collapsed;
            StandingsEmptyStateText.Visibility = Visibility.Collapsed;

            ContractsFilterPanel.Visibility = Visibility.Visible;
            ContractsEmptyStateText.Visibility = Visibility.Collapsed;
            ContractsSeriesComboBox.SelectedIndex = 0; // F1 default
            ContractsSeasonComboBox.SelectedIndex = 2; // 2025

            LoadContracts();
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

        private void ContractsFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TopTitleText.Text == "Contracts")
            {
                LoadContracts();
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
                StandingsDataGrid.Visibility = Visibility.Collapsed;
                StandingsEmptyStateText.Text = $"No sample data yet for {series} {season}.";
                StandingsEmptyStateText.Visibility = Visibility.Visible;
                return;
            }

            StandingsDataGrid.ItemsSource = rows;
            StandingsDataGrid.Visibility = Visibility.Visible;
            StandingsEmptyStateText.Visibility = Visibility.Collapsed;
        }

        private void LoadContracts()
        {
            if (ContractsSeriesComboBox.SelectedItem is not ComboBoxItem seriesItem ||
                ContractsSeasonComboBox.SelectedItem is not ComboBoxItem seasonItem)
            {
                return;
            }

            string series = seriesItem.Content?.ToString() ?? "F1";
            if (!int.TryParse(seasonItem.Content?.ToString(), out int season))
            {
                season = 2025;
            }

            var filtered = _sampleContracts.FindAll(contract =>
                contract.Series == series &&
                contract.StartSeason <= season &&
                contract.EndSeason >= season);

            ContractsGrid.ItemsSource = filtered;
            if (filtered.Count == 0)
            {
                ContractsGrid.Visibility = Visibility.Collapsed;
                ContractsEmptyStateText.Text = $"No contracts yet for {series} {season}.";
                ContractsEmptyStateText.Visibility = Visibility.Visible;
                return;
            }

            ContractsEmptyStateText.Visibility = Visibility.Collapsed;
            ContractsGrid.Visibility = Visibility.Visible;
        }
    }
}
