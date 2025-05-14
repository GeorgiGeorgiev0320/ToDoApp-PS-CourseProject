using ToDoApp.Services;

namespace ToDoApp
{
    public partial class App : Application
    {
        private static TodoDatabase database;
        public static TodoDatabase Database =>
            database ??= new TodoDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "todo.db3"));

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new Views.MainPage());
        }
    }
}