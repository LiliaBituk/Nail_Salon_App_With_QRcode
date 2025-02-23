using BusinessLogic;

namespace Nail_Salon_Mobile_App_New
{
    public partial class EditCustomerPage : ContentPage
    {
        private readonly Database _database;
        private Customer _customer; 
        private VisitLogs _visitLog;
        private Service _service; 

        public EditCustomerPage(VisitLogs visitLog)
        {
            InitializeComponent();
            _database = new Database(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NailSalon.db"));
            _visitLog = visitLog; 

            BindingContext = _visitLog; 

            LoadVisitDataAsync();
            Design.ApplyTheme(Resources);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadVisitDataAsync();
            await LoadServicesAsync();

        }

        private async Task LoadVisitDataAsync()
        {
            _customer = await _database.GetCustomerByIdAsync(_visitLog.CustomerId);
            _service = await _database.GetServiceByIdAsync(_visitLog.ServiceId);

            if (_customer != null)
            {
                FullNameEntry.Text = _customer.CustomerFullName;
                PhoneNumberEntry.Text = _customer.CustomerPhoneNumber;
            }

            PriceEntry.Text = _visitLog.Price.ToString();

            await LoadServicesAsync();

            if (_service != null)
            {
                ServicePicker.SelectedItem = _service.ServiceName;
            }
        }


        private async Task LoadServicesAsync()
        {
            var services = await _database.GetServicesAsync();
            var serviceNames = services.Select(s => s.ServiceName).ToList();
            ServicePicker.ItemsSource = serviceNames;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FullNameEntry.Text) || string.IsNullOrEmpty(PhoneNumberEntry.Text) ||
                string.IsNullOrEmpty(PriceEntry.Text))
            {
                await DisplayAlert("Ошибка", "Пожалуйста, заполните все поля", "OK");
                return;
            }

            bool customerChanged = _customer.CustomerFullName != FullNameEntry.Text || _customer.CustomerPhoneNumber != PhoneNumberEntry.Text;
            bool serviceChanged = _service.ServiceName != ServicePicker.SelectedItem.ToString();

            if (customerChanged)
            {
                _customer.CustomerFullName = FullNameEntry.Text;
                _customer.CustomerPhoneNumber = PhoneNumberEntry.Text;
                await _database.UpdateCustomerAsync(_customer);
            }

            _visitLog.Price = Convert.ToDecimal(PriceEntry.Text);

            if (serviceChanged)
            {
                var newService = await _database.GetServiceByNameAsync(ServicePicker.SelectedItem.ToString());
                if (newService != null)
                {
                    _visitLog.ServiceId = newService.Id;
                }
            }

            await _database.UpdateVisitLogAsync(_visitLog); 

            await DisplayAlert("Успех", "Данные успешно обновлены", "OK");
            await Navigation.PopAsync();
        }


        private async void OnCancelClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Отмена", "Вы уверены, что хотите отменить редактирование?", "Да", "Нет");
            if (confirm)
            {
                await Navigation.PopAsync();
            }
        }
    }
}
