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
    public sealed partial class ContactDetailsPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");


        public ContactDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            CreateContactDetailsTable();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateContactDetailsTable();
            txtfName.IsReadOnly = true;
            txtlName.IsReadOnly = true;
            txtcompName.IsReadOnly = true;
            txtPhone.IsReadOnly = true;

        }

        public void CreateContactDetailsTable()
        {
            // Creating table
            conn.CreateTable<ContactDetails>();
            var query = conn.Table<ContactDetails>();
            ContactsList.ItemsSource = query.ToList();
        }

        private void AddContactDetails_Click(object sender, RoutedEventArgs e)
        {
            AddBtn.Visibility = Visibility.Collapsed;
            
            SaveBtn.Visibility = Visibility.Visible;
            CancelBtn.Visibility = Visibility.Visible;
            txtfName.IsReadOnly = false;
            txtlName.IsReadOnly = false;
            txtcompName.IsReadOnly = false;
            txtPhone.IsReadOnly = false;
            txtfName.Text = "";
            txtlName.Text = "";
            txtcompName.Text = "";
            txtPhone.Text = "";
        }

        private void EditContactDetails_Click(object sender, RoutedEventArgs e)
        {
            EditBtn.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Visible;
            CancelBtn.Visibility = Visibility.Visible;
            txtfName.IsReadOnly = false;
            txtlName.IsReadOnly = false;
            txtcompName.IsReadOnly = false;
            txtPhone.IsReadOnly = false;

        }

        private async void DeleteContactDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContactDetails contact = (ContactDetails)ContactsList.SelectedItem;
                if (contact != null)
                {
                    conn.Delete(contact);
                    MessageDialog dialog = new MessageDialog("Contact has been deleted");
                    await dialog.ShowAsync();
                }
                ContactsList.ItemsSource = conn.Table<ContactDetails>().ToList();
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Contact cannot be deleted", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Contact cannot be deleted from the database", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
            
        }

        private void ContactsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ContactDetails contact = (ContactDetails)ContactsList.SelectedItem;
            if (contact != null)
            {
                txtfName.Text = contact.FirstName;
                txtlName.Text = contact.LastName;
                txtcompName.Text = contact.CompanyName;
                txtPhone.Text = contact.PhoneNumber;
            }
            else
            {
                if (ContactsList.Items.Count != 0)
                    ContactsList.SelectedIndex = 0;
                else
                {
                    ContactsList.SelectedIndex = -1;
                    txtfName.Text = "";
                    txtlName.Text = "";
                    txtcompName.Text = "";
                    txtPhone.Text = "";
                }

            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelBtn.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Collapsed;
            AddBtn.Visibility = Visibility.Visible;
            EditBtn.Visibility = Visibility.Visible;
            txtfName.IsReadOnly = true;
            txtlName.IsReadOnly = true;
            txtcompName.IsReadOnly = true;
            txtPhone.IsReadOnly = true;
            ContactDetails contact = (ContactDetails)ContactsList.SelectedItem;
            if (contact != null)
            {
                txtfName.Text = contact.FirstName;
                txtlName.Text = contact.LastName;
                txtcompName.Text = contact.CompanyName;
                txtPhone.Text = contact.PhoneNumber;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddBtn.Visibility == Visibility.Collapsed)
            {
                AddContactDetails();
            }
            else
            {
                EditContactDetails();
            }
            txtfName.IsReadOnly = true;
            txtlName.IsReadOnly = true;
            txtcompName.IsReadOnly = true;
            txtPhone.IsReadOnly = true;
            ContactsList.ItemsSource = conn.Table<ContactDetails>().ToList();
            CancelBtn.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Collapsed;
            AddBtn.Visibility = Visibility.Visible;
            EditBtn.Visibility = Visibility.Visible;
        }

        private async void AddContactDetails()
        {
            try
            {
                // checks if account name is null
                if (txtfName.Text == "" || txtlName.Text == "" || txtcompName.Text == "")
                {
                    MessageDialog dialog = new MessageDialog(" All fields are compulsory", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (txtfName.Text == "First Name" || txtlName.Text == "Last Name" || txtcompName.Text == "Company Name")
                {
                    MessageDialog variableerror = new MessageDialog("You cannot use default values", "Oops..!");
                    await variableerror.ShowAsync();
                }
                else
                {   // Inserts the data
                    conn.Insert(new ContactDetails()
                    {
                        FirstName = txtfName.Text,
                        LastName = txtlName.Text,
                        CompanyName = txtcompName.Text,
                        PhoneNumber = txtPhone.Text,

                    });
                    MessageDialog dialog = new MessageDialog("Contact has been added");
                    await dialog.ShowAsync();
                    //CreateContactDetailsTable();
                    //ContactsList.ItemsSource = conn.Table<ContactDetails>().ToList();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Contact cannot be added", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Contact cannot be added to the database", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }

        private async void EditContactDetails()
        {
            ContactDetails contact = (ContactDetails)ContactsList.SelectedItem;
            contact.FirstName = txtfName.Text;
            contact.LastName = txtlName.Text;
            contact.CompanyName = txtcompName.Text;
            contact.PhoneNumber = txtPhone.Text;

            try
            {
                conn.Update(contact);
                
                MessageDialog dialog = new MessageDialog("Contact has been updated");
                await dialog.ShowAsync();
                //ContactsList.ItemsSource = conn.Table<ContactDetails>().ToList();
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Contact cannot be updated", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Contact cannot be updated due to same values", "Oops..!");
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
