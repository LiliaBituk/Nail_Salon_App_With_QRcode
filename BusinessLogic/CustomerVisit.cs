using System.ComponentModel;

public class CustomerVisit : INotifyPropertyChanged
{

    private string _customerFullName;
    private string _customerPhoneNumber;
    private string _serviceName;
    private decimal _price;
    private int _id;

    public int Id
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged(nameof(Id));
        }
    }
    public DateTime StartDateTime { get; set; }

    public string CustomerFullName
    {
        get => _customerFullName;
        set
        {
            _customerFullName = value;
            OnPropertyChanged(nameof(CustomerFullName));
        }
    }

    public string CustomerPhoneNumber
    {
        get => _customerPhoneNumber;
        set
        {
            _customerPhoneNumber = value;
            OnPropertyChanged(nameof(CustomerPhoneNumber));
        }
    }

    public string ServiceName
    {
        get => _serviceName;
        set
        {
            _serviceName = value;
            OnPropertyChanged(nameof(ServiceName));
        }
    }

    public decimal Price
    {
        get => _price;
        set
        {
            _price = value;
            OnPropertyChanged(nameof(Price));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}