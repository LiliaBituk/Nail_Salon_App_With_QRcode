using System.ComponentModel;

public class EditCustomerViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string _customerFullName;
    public string CustomerFullName
    {
        get => _customerFullName;
        set
        {
            _customerFullName = value;
            OnPropertyChanged(nameof(CustomerFullName));
        }
    }

    private string _customerPhoneNumber;
    public string CustomerPhoneNumber
    {
        get => _customerPhoneNumber;
        set
        {
            _customerPhoneNumber = value;
            OnPropertyChanged(nameof(CustomerPhoneNumber));
        }
    }

    private string _serviceName;
    public string ServiceName
    {
        get => _serviceName;
        set
        {
            _serviceName = value;
            OnPropertyChanged(nameof(ServiceName));
        }
    }

    private string _price;
    public string Price
    {
        get => _price;
        set
        {
            _price = value;
            OnPropertyChanged(nameof(Price));
        }
    }

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
