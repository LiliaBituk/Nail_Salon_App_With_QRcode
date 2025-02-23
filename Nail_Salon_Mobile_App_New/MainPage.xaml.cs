using BusinessLogic;

namespace Nail_Salon_Mobile_App_New
{
    public partial class MainPage : ContentPage
    {
        private readonly Database _database;

        public MainPage()
        {
            InitializeComponent();
            _database = new Database(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NailSalon.db"));
            LoadCustomerLog();
            Design.ApplyTheme(Resources);
        }

        private async void LoadCustomerLog()
        {
            var visitLogs = await _database.GetVisitLogsAsync();
            var customerVisits = new List<CustomerVisit>();

            foreach (var visit in visitLogs)
            {
                var customer = await _database.GetCustomerByIdAsync(visit.CustomerId); 
                var service = await _database.GetServiceByIdAsync(visit.ServiceId);

                customerVisits.Add(new CustomerVisit
                {
                    Id = visit.Id, 
                    CustomerFullName = customer?.CustomerFullName ?? "Неизвестно",
                    CustomerPhoneNumber = customer?.CustomerPhoneNumber.ToString() ?? "Неизвестно",
                    ServiceName = service?.ServiceName ?? "Неизвестно",
                    Price = visit.Price,
                    StartDateTime = visit.StartDateTime
                });

            }

            CustomerLog.ItemsSource = customerVisits;
        }


        private async void OnCreateCustomerClicked(object sender, EventArgs e)
        {
            var fullName = FullNameEntry.Text;
            var phoneNumberText = PhoneNumberEntry.Text;
            var service = ServicePicker.SelectedItem?.ToString();
            var appointmentDate = AppointmentDatePicker.Date;
            var appointmentTime = AppointmentTimePicker.Time;

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phoneNumberText) || service == null)
            {
                await DisplayAlert("Ошибка", "Пожалуйста, заполните все поля.", "OK");
                return;
            }

            var existingCustomer = await _database.GetCustomerByDetailsAsync(fullName, phoneNumberText);
            bool isNewCustomer = existingCustomer == null;

            if (isNewCustomer)
            {
                existingCustomer = new Customer
                {
                    CustomerFullName = fullName,
                    CustomerBirthDate = BirthDatePicker.Date,
                    CustomerPhoneNumber = phoneNumberText,
                    CustomerIsNew = true
                };

                await _database.SaveCustomerAsync(existingCustomer);
            }
            else
            {
                existingCustomer.CustomerIsNew = false; 
            }

            var services = await _database.GetServicesAsync();
            var selectedService = services.FirstOrDefault(s => s.ServiceName == service);
            var discountedPrice = selectedService.GetDiscountedPrice(existingCustomer.CustomerIsNew);

            var visitLog = new VisitLogs
            {
                CustomerId = existingCustomer.Id,
                ServiceId = selectedService?.Id ?? 0,
                StartDateTime = appointmentDate.Add(appointmentTime),
                EndTime = appointmentDate.Add(appointmentTime).Add(selectedService?.ServiceExecutionTime ?? TimeSpan.Zero),
                Price = discountedPrice
            };

            await _database.SaveVisitLogAsync(visitLog);

            await DisplayAlert("Успешно", $"Клиент {existingCustomer.CustomerFullName} ({existingCustomer.CustomerPhoneNumber}) {visitLog.StartDateTime} {visitLog.Price}p был успешно добавлен.", "OK");

            LoadCustomerLog();

            FullNameEntry.Text = string.Empty;
            PhoneNumberEntry.Text = string.Empty;
            ServicePicker.SelectedItem = null;
        }

        private async Task InitializeServicesAsync()
        {
            var services = new List<Service>
            {
                new Service { ServiceName = "Маникюр", ServicePrice = 100, ServiceExecutionTime = TimeSpan.FromHours(1) },
                new Service { ServiceName = "Педикюр", ServicePrice = 150, ServiceExecutionTime = TimeSpan.FromHours(1.5) },
                new Service { ServiceName = "Наращивание", ServicePrice = 200, ServiceExecutionTime = TimeSpan.FromHours(2) }
            };

            foreach (var service in services)
            {
                var existingService = await _database.GetServicesAsync();
                var serviceExists = existingService.FirstOrDefault(s => s.ServiceName == service.ServiceName);
                if (serviceExists == null)
                {
                    await _database.SaveServiceAsync(service);
                }
            }
        }

        private async Task LoadServicesAsync()
        {
            var services = await _database.GetServicesAsync();
            ServicePicker.ItemsSource = services.Select(s => s.ServiceName).ToList();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // await InitializeServicesAsync();
            LoadFilterOptionsAsync();
            await LoadServicesAsync();
        }

        private async void OnFilterSelected(object sender, EventArgs e)
        {
            if (FilterPicker.SelectedItem == null)
                return;

            string selectedFilter = FilterPicker.SelectedItem.ToString();

            var visitLogs = await _database.GetVisitLogsAsync();
            var customers = await _database.GetCustomersAsync();
            var services = await _database.GetServicesAsync();

            var filteredVisits = visitLogs
                .Join(customers, visit => visit.CustomerId, customer => customer.Id, (visit, customer) => new { visit, customer })
                .Join(services, vc => vc.visit.ServiceId, service => service.Id, (vc, service) => new CustomerVisit
                {
                    Id = vc.visit.Id, 
                    CustomerFullName = vc.customer.CustomerFullName,
                    CustomerPhoneNumber = vc.customer.CustomerPhoneNumber.ToString(),
                    ServiceName = service.ServiceName,
                    Price = vc.visit.Price,
                    StartDateTime = vc.visit.StartDateTime
                })
                .Where(visit => selectedFilter == "Все" || visit.ServiceName == selectedFilter)
                .ToList();

            CustomerLog.ItemsSource = filteredVisits;
        }


        private async void OnEditCustomerClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var customerVisit = button?.BindingContext as CustomerVisit;

            if (customerVisit != null)
            {
                var visitLog = await _database.GetVisitLogByIdAsync(customerVisit.Id);
                if (visitLog != null)
                {
                    await Navigation.PushAsync(new EditCustomerPage(visitLog));
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось загрузить данные визита", "OK");
                }
            }
        }


        private async void OnDeleteCustomerClicked(object sender, EventArgs e)
        {
            var selectedVisit = (sender as Button)?.BindingContext as CustomerVisit;
            if (selectedVisit == null) return;

            var confirm = await DisplayAlert("Удаление", "Вы уверены, что хотите удалить запись?", "Да", "Нет");
            if (!confirm) return;

            await _database.DeleteVisitLogAsync(selectedVisit.Id);
            LoadCustomerLog();
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchEntry.Text?.ToLower() ?? "";
            var visitLogs = await _database.GetVisitLogsAsync();
            var filteredVisits = new List<CustomerVisit>();

            foreach (var visit in visitLogs)
            {
                var customer = await _database.GetCustomerByIdAsync(visit.CustomerId);
                var service = await _database.GetServiceByIdAsync(visit.ServiceId);

                if (customer == null || service == null)
                    continue;

                if (customer.CustomerFullName.ToLower().Contains(searchText) ||
                  customer.CustomerPhoneNumber.ToString().Contains(searchText) ||
                  service.ServiceName.ToLower().Contains(searchText))
                {
                    filteredVisits.Add(new CustomerVisit
                    {
                        Id = visit.Id, 
                        CustomerFullName = customer.CustomerFullName,
                        CustomerPhoneNumber = customer.CustomerPhoneNumber.ToString(),
                        ServiceName = service.ServiceName,
                        Price = visit.Price,
                        StartDateTime = visit.StartDateTime
                    });
                }

            }

            CustomerLog.ItemsSource = filteredVisits;
        }

        private async void LoadFilterOptionsAsync()
        {
            var services = await _database.GetServicesAsync();
            FilterPicker.Items.Clear();
            FilterPicker.Items.Add("Все"); 

            foreach (var service in services)
            {
                FilterPicker.Items.Add(service.ServiceName);
            }

            FilterPicker.SelectedIndex = 0; 
        }
    }
}
