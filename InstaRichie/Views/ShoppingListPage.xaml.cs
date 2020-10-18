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
    public sealed partial class ShoppingListPage : Page
    {
        SQLiteConnection conn; //adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ShoppingListPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            ///Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            //Creating table
            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<ShoppingList>();
            var query = conn.Table<ShoppingList>();
            shoppingList.ItemsSource = query.ToList();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //string SDay = ShoppingDate.Date.Value.Day.ToString();
            //string SMonth = ShoppingDate.Date.Value.Month.ToString();
            //string SYear = ShoppingDate.Date.Value.Year.ToString();
            //string FinalDate = "" + SDay + "/" + SMonth + "/" + SYear;

            try
            {
                // checks if account name is null
                if (ShopName.Text.ToString() == "" || ItemName.Text.ToString() == "" || PriceQuoted.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Please enter all fields !!");
                    await dialog.ShowAsync();
                }
                else
                {   // Inserts the data
                    conn.Insert(new ShoppingList()
                    {
                        ShopName = ShopName.Text,
                        Item = ItemName.Text,
                        ShoppingDate = ShoppingDate.Date.ToString("MM-dd-yyyy"),
                        PriceQuoted = PriceQuoted.Text

                    });
                    Results();
                }
            }
            catch (Exception exception)
            {   // Exception to display when amount is invalid or not numbers
                if (exception is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Insert all the data !");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (exception is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("The name already exist please try a different name");
                    await dialog.ShowAsync();
                }

            }
        }
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("If you delete this, it will delete all shopping details of the record");
            ShowConf.Commands.Add(new UICommand("Yes!!! Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                // checks if data is null else inserts
                try
                {
                    string ShoppingList1 = ((ShoppingList)shoppingList.SelectedItem).ShopName;
                    var querydel = conn.Query<ShoppingList>("Delete from ShoppingList where ShopName is ='" + ShoppingList1 + "'");
                    Results();
                    conn.CreateTable<ShoppingList>();
                    var querytable = conn.Query<ShoppingList>("Delete from ShoppingList where ShopName is ='" + ShoppingList1 + "'");

                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item you want to Delete");
                    await ClearDialog.ShowAsync();
                }
            }
        }
        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                string ShoppingList1 = ((ShoppingList)shoppingList.SelectedItem).ShopName;
                if (ShoppingList1 == "")
                {
                    MessageDialog dialog = new MessageDialog("Please select the one that should be edited");
                    await dialog.ShowAsync();
                }
                else
                {
                    //string SDay = ShoppingDate.Date.Value.Day.ToString();
                    //string SMonth = ShoppingDate.Date.Value.Month.ToString();
                    //string SYear = ShoppingDate.Date.Value.Year.ToString();
                    //string FinalDate = "" + SDay + "/" + SMonth + "/" + SYear;

                    //var FinalDate = ShoppingDate.Date.ToString("M/d/yyyy");

                    string shopname = ShopName.Text;
                    string itemname = ItemName.Text;
                    string shoppingdate = ShoppingDate.Date.ToString("mm-dd-yyyy");
                    string pricequoted = PriceQuoted.Text;

                    conn.CreateTable<ShoppingList>();
                    var query = conn.Table<ShoppingList>();
                    var queryEdit = conn.Query<ShoppingList>("update ShoppingList set ShopName ='" + shopname + "', Item ='" + itemname + "', ShoppingDate ='" + shoppingdate + "', PriceQuoted ='" + pricequoted + "' where ShopName ='" + ShoppingList1 + "'");
                    Results();
                }

            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Please select record to edit ", "Error !!");
                await dialog.ShowAsync();

            }

        }

        private async void shoppingList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                string selectShopName = ((ShoppingList)shoppingList.SelectedItem).ShopName.ToString();
                string selectItemName = ((ShoppingList)shoppingList.SelectedItem).Item.ToString();
                //DateTime selectShoppingDate = DateTime.ParseExact(shoppingList.)
                string selectShoppingDate = ((ShoppingList)shoppingList.SelectedItem).ShoppingDate.ToString();
                string selectPriceQuoted = ((ShoppingList)shoppingList.SelectedItem).PriceQuoted.ToString();

                //string dateString = Shopping.ShoppingDate;

                ShopName.Text = selectShopName;
                ItemName.Text = selectItemName;
                //ShoppingDate.SelectedDate = Convert.ToDateTime(selectShoppingDate);
                //ShoppingDate.Date = DateTime.ParseExact(selectShoppingDate, @"MM-dd-yyyy", provider: System.Globalization.CultureInfo.InvariantCulture);
                ShoppingDate.Date = DateTime.ParseExact(selectShoppingDate, @"MM-dd-yyyy", provider: System.Globalization.CultureInfo.InvariantCulture);
                PriceQuoted.Text = selectPriceQuoted;

            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Record Edited", "Important");
                await dialog.ShowAsync();
            }


            //var shoppinglist = shoppingList.SelectedItem as Shopping;

            //if (shoppinglist != null)
            //{
            //    var queryRead = conn.Table<Shopping>();

            //    string shopname = shoppinglist.ShopName;
            //    string itemname = shoppinglist.NameOfItem;
            //    string pricequoted = shoppinglist.PriceQuoted;

            //    //DateTime shoppingdate = DateTime.ParseExact(shoppinglist.ShoppingDate, @"MM-dd-yyyy", provider: System.Globalization.CultureInfo.InvariantCulture);

            //    ShopName.Text = shopname;
            //    ItemName.Text = itemname;
            //    PriceQuoted.Text = pricequoted;

            //    //ShoppingDate.Date = shoppingdate;

            //}
            //shoppingList.SelectedIndex = -1;
        }
    }
    
}
