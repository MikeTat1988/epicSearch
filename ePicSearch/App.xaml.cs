using ePicSearch.Views;
using ePicSearch.Services;

namespace ePicSearch
{
    public partial class App : Application
    {
        public App(AppShell appShell)
        {
            InitializeComponent();
            MainPage = appShell;
        }
    }
}

