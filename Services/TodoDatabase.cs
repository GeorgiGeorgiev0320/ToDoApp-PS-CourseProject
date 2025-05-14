using SQLite;
using ToDoApp.Models;

namespace ToDoApp.Services
{
    public class TodoDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public TodoDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<TodoItem>().Wait();
        }

        public Task<List<TodoItem>> GetItemsAsync() =>
            _database.Table<TodoItem>().OrderBy(i => i.DueDate).ToListAsync();

        public Task<int> SaveItemAsync(TodoItem item) =>
            item.Id == 0 ? _database.InsertAsync(item) : _database.UpdateAsync(item);

        public Task<int> DeleteItemAsync(TodoItem item) =>
            _database.DeleteAsync(item);
    }
}
