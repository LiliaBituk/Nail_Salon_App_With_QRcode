using SQLite;

namespace BusinessLogic
{
    public class VisitLogs
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int CustomerId { get; set; } 
        public int ServiceId { get; set; }

       // public int EmployeeId { get; set; } 

        public DateTime StartDateTime { get; set; }

        public DateTime EndTime { get; set; }

        public decimal Price { get; set; }

        public string StartDateTimeString => StartDateTime.ToString("yyyy-MM-dd HH:mm");
        public string EndTimeString => EndTime.ToString("yyyy-MM-dd HH:mm");

    }
}
