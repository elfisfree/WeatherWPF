using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace TemperatureApp
{
    public class TemperatureEntry
    {
        public int Day { get; set; }
        public double Temperature { get; set; }
        public TemperatureEntry(int day, double temperature)
        {
            Day = day;
            Temperature = temperature;
        }
        public override string ToString()
        {
            return $"day {Day}: {Temperature}";
        }
    }

    public partial class MainWindow : Window
    {
        private List<TemperatureEntry> temperatureEntries = new List<TemperatureEntry>();
        private int dayCounter = 1;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void AddTemperature_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(TemperatureInput.Text, out double temp))
            {
                var entry = new TemperatureEntry(dayCounter, temp);
                temperatureEntries.Add(entry);
                TemperatureList.Items.Add(entry);
                dayCounter++;
                TemperatureInput.Clear();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите допустимую температуру.");
            }
        }
        private void ShowStatistics_Click(object sender, RoutedEventArgs e)
        {
            if (temperatureEntries.Any())
            {
                double average = temperatureEntries.Average(te => te.Temperature);
                double min = temperatureEntries.Min(te => te.Temperature);
                double max = temperatureEntries.Max(te => te.Temperature);

                var temperatureCounts = temperatureEntries
                    .GroupBy(te => te.Temperature)
                    .Select(g => new { Temperature = g.Key, Count = g.Count() })
                    .OrderBy(g => g.Temperature)
                    .ToList();

                var repeatedTemperatures = temperatureCounts.Where(tc => tc.Count > 1).ToList();

                string repetitionStats = repeatedTemperatures.Any()
                    ? string.Join("\n", repeatedTemperatures.Select(tc => $"Температура в {tc.Temperature} гр: {tc.Count} дня"))
                    : "Нет повторяющихся температур.";

                var abnormalDrops = new List<string>();
                for (int i = 1; i < temperatureEntries.Count; i++)
                {
                    if (temperatureEntries[i - 1].Temperature - temperatureEntries[i].Temperature >= 10)
                    {
                        abnormalDrops.Add($"Аномальный спад: день {temperatureEntries[i - 1].Day} -> день {temperatureEntries[i].Day}");
                    }
                }
                var abnormalUps = new List<string>();
                for (int i = 1; i < temperatureEntries.Count; i++)
                {
                    if (temperatureEntries[i - 1].Temperature + temperatureEntries[i].Temperature >= 10)
                    {
                        abnormalUps.Add($"Аномальный подъем: день {temperatureEntries[i - 1].Day} -> день {temperatureEntries[i].Day}");
                    }
                }
                string abnormalDropsStats = abnormalDrops.Any()
                    ? string.Join("\n", abnormalDrops)
                    : "Аномальных спадов нет.";
                string abnormalUpsStats = abnormalUps.Any()
                    ? string.Join("\n", abnormalUps)
                    : "Аномальных подъемов нет.";

                Statistics.Text = $"Средняя температура: {average:F2}\n" +
                                  $"Минимальная температура: {min:F2}\n" +
                                  $"Максимальная температура: {max:F2}\n\n" +
                                  $"Повторения температур:\n{repetitionStats}\n\n" +
                                  $"Аномальные спады и подъемы:\n{abnormalDropsStats}\n{abnormalUpsStats}\n";
            }
            else
            {
                MessageBox.Show("Список температур пуст.");
            }
        }
        private void CloseStatistics_Click(object sender, RoutedEventArgs e)
        {
            Statistics.Text = string.Empty;
        }

        private void SortAscending_Click(object sender, RoutedEventArgs e)
        {
            SortTemperatures(true);
        }
        private void SortDescending_Click(object sender, RoutedEventArgs e)
        {
            SortTemperatures(false);
        }
        private void SortByDay_Click(object sender, RoutedEventArgs e)
        {
            temperatureEntries = temperatureEntries.OrderBy(te => te.Day).ToList();
            TemperatureList.Items.Clear();
            foreach (var entry in temperatureEntries)
            {
                TemperatureList.Items.Add(entry);
            }
        }
        private void FilterTemperature_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(FilterMinInput.Text, out double min) && double.TryParse(FilterMaxInput.Text, out double max))
            {
                FilterTemperatures(min, max);
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите допустимые значения для фильтра.");
            }
        }
        private void SortTemperatures(bool ascending = true)
        {
            var sortedEntries = ascending ?
                temperatureEntries.OrderBy(te => te.Temperature).ToList() :
                temperatureEntries.OrderByDescending(te => te.Temperature).ToList();

            TemperatureList.Items.Clear();
            foreach (var entry in sortedEntries)
            {
                TemperatureList.Items.Add(entry);
            }
        }
        private void FilterTemperatures(double min, double max)
        {
            var filteredEntries = temperatureEntries.Where(te => te.Temperature >= min && te.Temperature <= max).ToList();
            TemperatureList.Items.Clear();
            foreach (var entry in filteredEntries)
            {
                TemperatureList.Items.Add(entry);
            }
        }
        private void ClearList_Click(object sender, RoutedEventArgs e)
        {
            temperatureEntries.Clear();
            TemperatureList.Items.Clear();
            dayCounter = 1;
            Statistics.Text = string.Empty;
        }
        private void ExportToTxt_Click(object sender, RoutedEventArgs e)
        {
            if (temperatureEntries.Any())
            {
                string filePath = "C:\\Users\\verah\\source\\repos\\TemperatureData.txt";

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var entry in temperatureEntries)
                    {
                        writer.WriteLine(entry.ToString());
                    }
                    double average = temperatureEntries.Average(te => te.Temperature);
                    double min = temperatureEntries.Min(te => te.Temperature);
                    double max = temperatureEntries.Max(te => te.Temperature);

                    var temperatureCounts = temperatureEntries
                        .GroupBy(te => te.Temperature)
                        .Select(g => new { Temperature = g.Key, Count = g.Count() })
                        .OrderBy(g => g.Temperature)
                        .ToList();

                    var repeatedTemperatures = temperatureCounts.Where(tc => tc.Count > 1).ToList();

                    string repetitionStats = repeatedTemperatures.Any()
                        ? string.Join("\n", repeatedTemperatures.Select(tc => $"Температура в {tc.Temperature} гр: {tc.Count} дня"))
                        : "Нет повторяющихся температур.";

                    var abnormalDrops = new List<string>();
                    for (int i = 1; i < temperatureEntries.Count; i++)
                    {
                        if (temperatureEntries[i - 1].Temperature - temperatureEntries[i].Temperature >= 10)
                        {
                            abnormalDrops.Add($"Аномальный спад: день {temperatureEntries[i - 1].Day} -> день {temperatureEntries[i].Day}");
                        }
                    }

                    var abnormalUps = new List<string>();
                    for (int i = 1; i < temperatureEntries.Count; i++)
                    {
                        if (temperatureEntries[i].Temperature - temperatureEntries[i - 1].Temperature >= 10)
                        {
                            abnormalUps.Add($"Аномальный подъем: день {temperatureEntries[i - 1].Day} -> день {temperatureEntries[i].Day}");
                        }
                    }

                    string abnormalDropsStats = abnormalDrops.Any()
                        ? string.Join("\n", abnormalDrops)
                        : "Аномальных спадов нет.";

                    string abnormalUpsStats = abnormalUps.Any()
                        ? string.Join("\n", abnormalUps)
                        : "Аномальных подъемов нет.";

                    writer.WriteLine();
                    writer.WriteLine($"Средняя температура: {average:F2}");
                    writer.WriteLine($"Минимальная температура: {min:F2}");
                    writer.WriteLine($"Максимальная температура: {max:F2}");
                    writer.WriteLine();
                    writer.WriteLine("Повторения температур:");
                    writer.WriteLine(repetitionStats);
                    writer.WriteLine();
                    writer.WriteLine("Аномальные спады и подъемы:");
                    writer.WriteLine(abnormalDropsStats);
                    writer.WriteLine(abnormalUpsStats);
                }

                MessageBox.Show("Данные успешно экспортированы в TemperatureData.txt");
            }
            else
            {
                MessageBox.Show("Список температур пуст.");
            }
        }
    }
}