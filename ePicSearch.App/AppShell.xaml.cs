using ePicSearch.Services;
using ePicSearch.Views;

namespace ePicSearch
{
    public partial class AppShell : Shell
    {
        public AppShell(MainPage mainPage)
        {
            InitializeComponent();
            Items.Add(new ShellContent
            {
                Title = "Home",
                Content = mainPage
            });
        }
    }
}