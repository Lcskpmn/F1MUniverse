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
            // F3 2025 sample
            _sampleStandings[("F3", 2025)] = new List<StandingRow>
            {
                new() { Position = 1,  Driver = "T. Naël",       Team = "Prema",          Points = 210 },
                new() { Position = 2,  Driver = "M. Boya",       Team = "Campos",         Points = 195 },
                new() { Position = 3,  Driver = "N. León",       Team = "MP",             Points = 186 },
                new() { Position = 4,  Driver = "U. Ugochukwu",  Team = "Prema",          Points = 175 },
                new() { Position = 5,  Driver = "C. Stenshorne", Team = "Hitech",         Points = 160 },
                new() { Position = 6,  Driver = "J. Edgar",      Team = "ART",            Points = 148 },
                new() { Position = 7,  Driver = "A. Lindblad",   Team = "Prema",          Points = 139 },
                new() { Position = 8,  Driver = "L. Browning",   Team = "Hitech",         Points = 120 },
                new() { Position = 9,  Driver = "S. Flörsch",    Team = "Van Amersfoort", Points = 112 },
                new() { Position = 10, Driver = "T. Barnard",    Team = "Jenzer",         Points = 104 },
            };

            // Later kun je hier extra combo's toevoegen, bijv:
            // _sampleStandings[("F1", 2024)] = new List<StandingRow> { ... };
        }

        // ---- Navigation helpers ----

        private void ShowDashboard()
        {
            TopTitleText.Text = "Dashboard";
            ContentTitleText.Text = "Welcome!";
            ContentBodyText.Text = "Choose a section on the left to get started (Standings or Contracts).";

            // Alles dat specifiek voor Standings is verbergen
            StandingsFilterPanel.Visibility = Visibility.Collapsed;
            StandingsDataGrid.Visibility = Visibility.Collapsed;
            StandingsEmptyStateText.Visibility = Visibility.Collapsed;
        }

        private void StandingsButton_Click(object sender, RoutedEventArgs e)
        {
            TopTitleText.Text = "Standings";
            ContentTitleText.Text = "F3 2025 Standings";
            ContentBodyText.Text = "Using the design doc example data so you can preview how the standings layout will look.";

            StandingsFilterPanel.Visibility = Visibility.Visible;

            // Default de filters naar de dataset uit het design doc
            SeriesComboBox.SelectedIndex = 2; // F3
            SeasonComboBox.SelectedIndex = 1; // 2025

            LoadStandings();
        }

        private void ContractsButton_Click(object sender, RoutedEventArgs e)
        {
            // Geen apart window meer – we wisselen gewoon de content in dit hoofdvenster
            TopTitleText.Text = "Contracts";
            ContentTitleText.Text = "Contracts";
            ContentBodyText.Text =
                "Placeholder for the contracts screen. Here you will be able to view and edit driver and staff contracts for F1 / F2 / F3.";

            // Standings-specifieke UI verbergen
            StandingsFilterPanel.Visibility = Visibility.Collapsed;
            StandingsDataGrid.Visibility = Visibility.Collapsed;
            StandingsEmptyStateText.Visibility = Visibility.Collapsed;
        }

        // ---- Filters changed ----

        private void SeriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Alleen herladen als we op de Standings-pagina zitten
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
            // Read selected series (F1/F2/F3) en season
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
    }
}

