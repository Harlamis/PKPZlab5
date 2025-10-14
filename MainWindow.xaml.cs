// MainWindow.xaml.cs
using Microsoft.Win32;
using PKPZlab5;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace PKPZlab5
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Automobile> carList = new ObservableCollection<Automobile>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeApp();
        }

        private void InitializeApp()
        {
            AutomobileListBox.ItemsSource = carList;
            ColorComboBox.ItemsSource = Enum.GetValues(typeof(Automobile.Colour));
            ColorComboBox.SelectedIndex = 0;
            carList.CollectionChanged += CarList_CollectionChanged;
        }

        private void AutomobileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AutomobileListBox.SelectedItems.Count == 1 && AutomobileListBox.SelectedItem is Automobile selectedCar)
            {
                LengthTextBox.Text = selectedCar.Length.ToString();
                WidthTextBox.Text = selectedCar.Width.ToString();
                HeightTextBox.Text = selectedCar.Height.ToString();
                HorsepowerTextBox.Text = selectedCar.Horsepower.ToString(CultureInfo.InvariantCulture);
                SeatsTextBox.Text = selectedCar.Seats.ToString();
                ZeroToHundredTextBox.Text = selectedCar.Zero_to_hundred.ToString(CultureInfo.InvariantCulture);
                ColorComboBox.SelectedItem = selectedCar.Color;

                UpdateCarButton.IsEnabled = true;
                AddCarButton.IsEnabled = false;
            }
            else
            {
                ClearInputFields();
                UpdateCarButton.IsEnabled = false;
                AddCarButton.IsEnabled = true;
            }
        }

        private void UpdateCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (AutomobileListBox.SelectedItem is Automobile selectedCar)
            {
                try
                {
                    selectedCar.Length = uint.Parse(LengthTextBox.Text);
                    selectedCar.Width = uint.Parse(WidthTextBox.Text);
                    selectedCar.Height = uint.Parse(HeightTextBox.Text);
                    selectedCar.Horsepower = double.Parse(HorsepowerTextBox.Text, CultureInfo.InvariantCulture);
                    selectedCar.Seats = uint.Parse(SeatsTextBox.Text);
                    selectedCar.Zero_to_hundred = float.Parse(ZeroToHundredTextBox.Text, CultureInfo.InvariantCulture);
                    selectedCar.Color = (Automobile.Colour)ColorComboBox.SelectedItem;

                    AutomobileListBox.Items.Refresh();
                    MessageBox.Show("Car details updated successfully!", "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating car: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            AutomobileListBox.UnselectAll();
        }

        private void AddCarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newCar = new Automobile
                {
                    Length = uint.Parse(LengthTextBox.Text),
                    Width = uint.Parse(WidthTextBox.Text),
                    Height = uint.Parse(HeightTextBox.Text),
                    Horsepower = double.Parse(HorsepowerTextBox.Text, CultureInfo.InvariantCulture),
                    Seats = uint.Parse(SeatsTextBox.Text),
                    Zero_to_hundred = float.Parse(ZeroToHundredTextBox.Text, CultureInfo.InvariantCulture),
                    Color = (Automobile.Colour)ColorComboBox.SelectedItem
                };

                carList.Add(newCar);
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding automobile: {ex.Message}\n\nPlease ensure all fields are filled with valid numbers.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Save Automobile List"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var lines = carList.Select(car => string.Join(",",
                        car.Length,
                        car.Width,
                        car.Height,
                        car.Horsepower.ToString(CultureInfo.InvariantCulture),
                        car.Seats,
                        car.Zero_to_hundred.ToString(CultureInfo.InvariantCulture),
                        car.Color
                    )).ToList();

                    lines.Insert(0, "Length,Width,Height,Horsepower,Seats,Zero_to_hundred,Color");
                    File.WriteAllLines(saveFileDialog.FileName, lines);
                    MessageBox.Show("Automobile list saved successfully as a text file!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save file: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text file (*.txt)|*.txt|CSV file (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Load Automobile List"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var lines = File.ReadAllLines(openFileDialog.FileName).Skip(1);
                    var loadedCars = new List<Automobile>();

                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var parts = line.Split(',');
                        if (parts.Length == 7)
                        {
                            var car = new Automobile
                            {
                                Length = uint.Parse(parts[0]),
                                Width = uint.Parse(parts[1]),
                                Height = uint.Parse(parts[2]),
                                Horsepower = double.Parse(parts[3], CultureInfo.InvariantCulture),
                                Seats = uint.Parse(parts[4]),
                                Zero_to_hundred = float.Parse(parts[5], CultureInfo.InvariantCulture),
                                Color = (Automobile.Colour)Enum.Parse(typeof(Automobile.Colour), parts[6])
                            };
                            loadedCars.Add(car);
                        }
                    }

                    carList.Clear();
                    foreach (var car in loadedCars)
                    {
                        carList.Add(car);
                    }
                    MessageBox.Show("Automobile list loaded successfully from the text file!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load or parse the file: {ex.Message}\n\nMake sure the file format is correct.", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CarList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RenumberItems();
        }

        private void RenumberItems()
        {
            for (int i = 0; i < carList.Count; i++)
            {
                carList[i].Number = i + 1;
            }
        }

        private void CalculateVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (AutomobileListBox.SelectedItem is Automobile selectedCar)
            {
                ulong volume = selectedCar.CalculateVolume();
                MessageBox.Show($"Approximate volume of car #{selectedCar.Number}: {volume} cubic mm.", "Calculation Result");
            }
            else
            {
                MessageBox.Show("Please select one automobile from the list.", "Warning");
            }
        }

        private void CheckFitButton_Click(object sender, RoutedEventArgs e)
        {
            if (AutomobileListBox.SelectedItem is Automobile selectedCar)
            {
                if (uint.TryParse(PassengersTextBox.Text, out uint passengers))
                {
                    bool canFit = selectedCar.CanFitInCar((int)passengers);
                    string message = canFit
                        ? $"Yes, {passengers} passengers can fit in car #{selectedCar.Number}."
                        : $"No, {passengers} passengers will not fit in car #{selectedCar.Number}.";
                    MessageBox.Show(message, "Capacity Check");
                }
                else
                {
                    MessageBox.Show("Please enter a valid number of passengers.", "Input Error");
                }
            }
            else
            {
                MessageBox.Show("Please select one automobile from the list.", "Warning");
            }
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            if (AutomobileListBox.SelectedItems.Count == 2)
            {
                var car1 = (Automobile)AutomobileListBox.SelectedItems[0];
                var car2 = (Automobile)AutomobileListBox.SelectedItems[1];

                bool car1IsBetter = car1.IsBetterPerformance(car2);
                bool car2IsBetter = car2.IsBetterPerformance(car1);

                string result;
                if (car1IsBetter)
                {
                    result = $"Car #{car1.Number} ('{car1}') has better performance than car #{car2.Number} ('{car2}').";
                }
                else if (car2IsBetter)
                {
                    result = $"Car #{car2.Number} ('{car2}') has better performance than car #{car1.Number} ('{car1}').";
                }
                else
                {
                    result = "Neither car has a clear performance advantage.";
                }
                MessageBox.Show(result, "Comparison Result");
            }
            else
            {
                MessageBox.Show("Please select exactly two automobiles to compare (hold Ctrl to select multiple).", "Warning");
            }
        }

        private void ClearInputFields()
        {
            LengthTextBox.Clear();
            WidthTextBox.Clear();
            HeightTextBox.Clear();
            HorsepowerTextBox.Clear();
            SeatsTextBox.Clear();
            ZeroToHundredTextBox.Clear();
            ColorComboBox.SelectedIndex = 0;
        }
    }
}