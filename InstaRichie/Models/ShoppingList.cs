using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{
    public class ShoppingList
    {
        [PrimaryKey, AutoIncrement]
        public int ShoppingID { get; set; }

        [Unique]
        public string ShopName { get; set; }

        [NotNull]
        public string Item { get; set; }

        [NotNull]
        public string ShoppingDate { get; set; }

        [NotNull]
        public string PriceQuoted { get; set; }
    }
}
