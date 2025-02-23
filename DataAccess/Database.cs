using BusinessLogic;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Database
{
    private readonly SQLiteAsyncConnection _database;

    public Database(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Customer>().Wait();
        _database.CreateTableAsync<Service>().Wait();
        _database.CreateTableAsync<VisitLogs>().Wait();
        _database.CreateTableAsync<Employee>().Wait();
    }

    // Получение всех услуг
    public Task<List<Service>> GetServicesAsync()
    {
        return _database.Table<Service>().ToListAsync();
    }

    // Сохранение нового клиента
    public Task<int> SaveCustomerAsync(Customer customer)
    {
        return _database.InsertAsync(customer);
    }

    // Сохранение новой услуги
    public Task<int> SaveServiceAsync(Service service)
    {
        return _database.InsertAsync(service);
    }

    // Сохранение записи посещения
    public Task<int> SaveVisitLogAsync(VisitLogs visitLog)
    {
        return _database.InsertAsync(visitLog);
    }

    // Получение клиента по ID
    public async Task<Customer> GetCustomerByIdAsync(int id)
    {
        return await _database.Table<Customer>().FirstOrDefaultAsync(c => c.Id == id);
    }

    // Получение услуги по ID
    public async Task<Service> GetServiceByIdAsync(int id)
    {
        return await _database.Table<Service>().FirstOrDefaultAsync(s => s.Id == id);
    }

    // Получение всех клиентов
    public Task<List<Customer>> GetCustomersAsync()
    {
        return _database.Table<Customer>().ToListAsync();
    }

    // Получение клиента по имени и номеру телефона
    public Task<Customer> GetCustomerByDetailsAsync(string fullName, string phoneNumber)
    {
        return _database.Table<Customer>()
            .FirstOrDefaultAsync(c => c.CustomerFullName == fullName && c.CustomerPhoneNumber == phoneNumber);
    }

    // Получение всех посещений
    public Task<List<VisitLogs>> GetVisitLogsAsync()
    {
        return _database.Table<VisitLogs>().ToListAsync();
    }

    // Удаление записи посещения
    public async Task DeleteVisitLogAsync(int visitLogId)
    {
        var visitLog = await _database.Table<VisitLogs>().FirstOrDefaultAsync(v => v.Id == visitLogId);
        if (visitLog != null)
        {
            await _database.DeleteAsync(visitLog);
        }
    }


    // Обновление записи посещения
    public Task<int> UpdateVisitLogAsync(VisitLogs visitLog)
    {
        return _database.UpdateAsync(visitLog);
    }

    public async Task<VisitLogs> GetVisitLogByIdAsync(int id)
    {
        return await _database.Table<VisitLogs>().FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<int> UpdateCustomerAsync(Customer customer)
    {
        return await _database.UpdateAsync(customer);
    }

    public async Task<Service> GetServiceByNameAsync(string serviceName)
    {
        return await _database.Table<Service>().FirstOrDefaultAsync(s => s.ServiceName == serviceName);
    }
}
