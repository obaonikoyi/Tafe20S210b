using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentsInfoPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmentsInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            CreateAppointmentTable();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateAppointmentTable();
            txtEventName.IsReadOnly = true;
            txtLocation.IsReadOnly = true;
            txtEventDate.IsEnabled = false;
            txtStartTime.IsEnabled = false;
            txtEndTime.IsEnabled = false;
        }

        public void CreateAppointmentTable()
        {
            // Creating table
            conn.CreateTable<AppointmentInfo>();
            var query = conn.Table<AppointmentInfo>();
            AppointmentList.ItemsSource = query.ToList();
        }


        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            AddBtn.Visibility = Visibility.Collapsed;
            EditBtn.IsEnabled = false;
            SaveBtn.Visibility = Visibility.Visible;
            CancelBtn.Visibility = Visibility.Visible;
            txtEventName.IsReadOnly = false;
            txtLocation.IsReadOnly = false;
            txtEventDate.IsEnabled = true;
            txtStartTime.IsEnabled = true;
            txtEndTime.IsEnabled = true;
        }

        private void EditAppointment_Click(object sender, RoutedEventArgs e)
        {
            AddBtn.IsEnabled = false;
            EditBtn.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Visible;
            CancelBtn.Visibility = Visibility.Visible;
            txtEventName.IsReadOnly = false;
            txtLocation.IsReadOnly = false;
            txtEventDate.IsEnabled = true;
            txtStartTime.IsEnabled = true;
            txtEndTime.IsEnabled = true;
        }

        private async void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppointmentInfo appointment = (AppointmentInfo)AppointmentList.SelectedItem;
                if (appointment != null)
                {
                    conn.Delete(appointment);
                    MessageDialog dialog = new MessageDialog("Appointment has been deleted");
                    await dialog.ShowAsync();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("Please select appointment to be deleted", "Oops..!");
                    await dialog.ShowAsync();
                }
                AppointmentList.ItemsSource = conn.Table<AppointmentInfo>().ToList();
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Appointment cannot be deleted", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Appointment cannot be deleted from the database", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private void AppointmentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppointmentInfo appointment = (AppointmentInfo)AppointmentList.SelectedItem;
            if (appointment != null)
            {
                txtEventName.Text = appointment.EventName;
                txtLocation.Text = appointment.Location;
                txtEventDate.Date = appointment.EventDate;
                txtStartTime.Time = appointment.StartTime;
                txtEndTime.Time = appointment.EndTime;
            }
            else
            {
                if (AppointmentList.Items.Count != 0)
                    AppointmentList.SelectedIndex = 0;
                else
                {
                    AppointmentList.SelectedIndex = -1;
                    txtEventName.Text = "";
                    txtLocation.Text = "";
                    txtEventDate.Date = DateTime.Now;
                    txtStartTime.Time = TimeSpan.Zero;
                    txtEndTime.Time = TimeSpan.Zero;
                }

            }
        }

        private void CancelAppointment_Click(object sender, RoutedEventArgs e)
        {
            CancelBtn.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Collapsed;
            AddBtn.Visibility = Visibility.Visible;
            EditBtn.Visibility = Visibility.Visible;
            AddBtn.IsEnabled = true;
            EditBtn.IsEnabled = true;
            txtEventName.IsReadOnly = true;
            txtLocation.IsReadOnly = true;
            txtEventDate.IsEnabled = false;
            txtStartTime.IsEnabled = false;
            txtEndTime.IsEnabled = false;
            AppointmentInfo appointment = (AppointmentInfo)AppointmentList.SelectedItem;
            if (appointment != null)
            {
                txtEventName.Text = appointment.EventName;
                txtLocation.Text = appointment.Location;
                txtEventDate.Date = appointment.EventDate;
                txtStartTime.Time = appointment.StartTime;
                txtEndTime.Time = appointment.EndTime;
            }
        }

        private void SaveAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (AddBtn.Visibility == Visibility.Collapsed)
            {
                AddAppointment();
            }
            else
            {
                EditAppointment();
            }
            txtEventName.IsReadOnly = true;
            txtLocation.IsReadOnly = true;
            txtEventDate.IsEnabled = false;
            txtStartTime.IsEnabled = false;
            txtEndTime.IsEnabled = false;
            AppointmentList.ItemsSource = conn.Table<AppointmentInfo>().ToList();
            CancelBtn.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Collapsed;
            AddBtn.Visibility = Visibility.Visible;
            EditBtn.Visibility = Visibility.Visible;
            AddBtn.IsEnabled = true;
            EditBtn.IsEnabled = true;
        }

        private async void AddAppointment()
        {
            try
            {
                if (txtEventName.Text == "" || txtLocation.Text == "" || txtEventDate.Date == DateTimeOffset.MinValue ||
                    txtStartTime.Time == TimeSpan.MinValue || txtEndTime.Time == TimeSpan.MinValue)
                {
                    MessageDialog dialog = new MessageDialog("Please fill all the fields", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (txtStartTime.Time.CompareTo(txtEndTime.Time) > 1)
                {
                    MessageDialog dialog = new MessageDialog("Please check time", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Insert(new AppointmentInfo()
                    {
                        EventName = txtEventName.Text,
                        Location = txtLocation.Text,
                        EventDate = txtEventDate.Date.DateTime,
                        StartTime = txtStartTime.Time,
                        EndTime = txtEndTime.Time
                });
                    MessageDialog dialog = new MessageDialog("Appointment has been added");
                    await dialog.ShowAsync();
                }

            }
            catch (Exception ex)
            {
                // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Appointment cannot be added", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Appointment cannot be added to the database", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private async void EditAppointment()
        {
            AppointmentInfo appointment = (AppointmentInfo)AppointmentList.SelectedItem;
            appointment.EventName = txtEventName.Text;
            appointment.Location = txtLocation.Text;
            appointment.EventDate = txtEventDate.Date.Date;
            appointment.StartTime = txtStartTime.Time;
            appointment.EndTime = txtEndTime.Time;

            try
            {
                conn.Update(appointment);

                MessageDialog dialog = new MessageDialog("Appointment has been updated");
                await dialog.ShowAsync();
                
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Appointment cannot be updated", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Appointment cannot be updated due to same values", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        
    }
}
