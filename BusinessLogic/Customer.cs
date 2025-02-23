using SQLite;

namespace BusinessLogic
{
    public class Customer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string CustomerFullName { get; set; }

        public DateTime CustomerBirthDate { get; set; }

        public string CustomerPhoneNumber { get; set; }

        public bool CustomerIsNew { get; set; }

    }
}
