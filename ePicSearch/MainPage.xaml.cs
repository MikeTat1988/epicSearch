namespace ePicSearch
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Event handler for Start Preparation button
        private void OnStartPrepClicked(object sender, EventArgs e)
        {
            StartPrepBtn.Text = "Preparation Started!";
            // Add your logic here
        }

        // Event handler for Start Game button
        private void OnStartGameClicked(object sender, EventArgs e)
        {
            StartGameBtn.Text = "Game Started!";
            // Add your logic here
        }

        // Event handler for Exit App button
        private void OnExitAppClicked(object sender, EventArgs e)
        {
            Application.Current.Quit();  // Exits the app
        }
    }

}
