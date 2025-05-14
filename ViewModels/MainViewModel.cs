using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ToDoApp.Models;
using ToDoApp.Services;
using ToDoApp.Views;

namespace ToDoApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TodoItem> Items { get; } = new();
        private readonly TodoDatabase _database;

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }

        private TodoItem _newItem = new();
        public TodoItem NewItem
        {
            get => _newItem;
            set
            {
                _newItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewItem)));
            }
        }

        private string _validationMessage;
        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                _validationMessage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValidationMessage)));
            }
        }

        public MainViewModel()
        {
            _database = App.Database;

            AddCommand = new Command(async () => await AddItem());
            DeleteCommand = new Command<TodoItem>(async (item) => await DeleteItem(item));
            SaveCommand = new Command(async () => await SaveNewItem());
            OpenCommand = new Command<TodoItem>(async (item) => await OpenItem(item));

            LoadItems();
        }

        private async void LoadItems()
        {
            Items.Clear();
            var items = await _database.GetItemsAsync();
            foreach (var item in items)
            {
                item.PropertyChanged += async (s, e) =>
                {
                    if (e.PropertyName == nameof(TodoItem.IsCompleted))
                    {
                        await _database.SaveItemAsync(item);
                    }
                };
                Items.Add(item);
            }
        }

        private async Task AddItem()
        {
            NewItem = new TodoItem { DueDate = DateTime.Today };
        }

        private async Task SaveNewItem()
        {
            if (string.IsNullOrWhiteSpace(NewItem.Title) || string.IsNullOrWhiteSpace(NewItem.Description))
            {
                ValidationMessage = "Title and Description must not be empty.";
                return;
            }

            ValidationMessage = string.Empty;

            await _database.SaveItemAsync(NewItem);

            NewItem.PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(TodoItem.IsCompleted))
                {
                    await _database.SaveItemAsync(NewItem);
                }
            };

            Items.Add(NewItem);
            NewItem = new TodoItem();
        }

        private async Task DeleteItem(TodoItem item)
        {
            await _database.DeleteItemAsync(item);
            Items.Remove(item);
        }

        private async Task OpenItem(TodoItem item)
        {
            await App.Current.MainPage.Navigation.PushAsync(new TaskDetailPage(item));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}