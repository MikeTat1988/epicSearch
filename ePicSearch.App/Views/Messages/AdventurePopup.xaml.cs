using CommunityToolkit.Maui.Views;
using ePicSearch.Helpers;

namespace ePicSearch.Views
{
    public partial class AdventurePopup : Popup
    {
        private readonly TaskCompletionSource<bool> _completionSource;

        public AdventurePopup(TaskCompletionSource<bool> completionSource)
        {
            InitializeComponent();
            _completionSource = completionSource;
            CanBeDismissedByTappingOutsideOfPopup = false;
        }

        private void OnContinueClicked(object sender, EventArgs e)
        {
            _completionSource.SetResult(true);  // Continue Adventure
            Close();
        }

        private void OnFinishClicked(object sender, EventArgs e)
        {
            if (sender is View pressedButton)
            {
                AnimationHelper.AnimatePress(pressedButton);
            }

            _completionSource.SetResult(false); // Finish Adventure
            Close();
        }
    }
}
