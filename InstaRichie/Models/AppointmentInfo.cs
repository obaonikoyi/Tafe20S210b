using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{
    public class AppointmentInfo
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Unique]
        public string EventName { get; set; }

        [NotNull]
        public string Location { get; set; }

        [NotNull]
        public DateTime EventDate { get; set; }

        [NotNull]
        public TimeSpan StartTime { get; set; }

        [NotNull]
        public TimeSpan EndTime { get; set; }

    }
}
