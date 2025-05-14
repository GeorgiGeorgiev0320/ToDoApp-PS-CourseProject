using ToDoApp.Models;

namespace ToDoApp.Views
{
    public partial class TaskDetailPage : ContentPage
    {
        public TaskDetailPage(TodoItem item)
        {
            InitializeComponent();
            BindingContext = item;
        }
    }
}