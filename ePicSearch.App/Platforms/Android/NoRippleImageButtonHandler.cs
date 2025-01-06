using Microsoft.Maui.Handlers;
using Google.Android.Material.ImageView;
using Android.Graphics;
using AG = Android.Graphics; // Alias for Android.Graphics.Color
using ePicSearch.Helpers;
using ePicSearch.Views;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace ePicSearch.Platforms.Android
{
    public class NoRippleImageButtonHandler : ImageButtonHandler
    {
        public new static IPropertyMapper<NoRippleImageButton, NoRippleImageButtonHandler> Mapper =
            new PropertyMapper<NoRippleImageButton, NoRippleImageButtonHandler>(ImageButtonHandler.Mapper)
            {
                [nameof(VisualElement.Background)] = MapBackground
            };

        public NoRippleImageButtonHandler() : base(Mapper)
        {
        }

        public static void MapBackground(NoRippleImageButtonHandler handler, NoRippleImageButton button)
        {
            handler.PlatformView.Background = null;
            handler.PlatformView.StateListAnimator = null;
            handler.PlatformView.Clickable = true;
            handler.PlatformView.Focusable = true;
            handler.PlatformView.SetBackgroundColor(AG.Color.Transparent);
        }

        protected override void ConnectHandler(ShapeableImageView nativeView)
        {
            base.ConnectHandler(nativeView);

            try
            {
                nativeView.Background = null;
                nativeView.SetColorFilter(AG.Color.Transparent, PorterDuff.Mode.SrcAtop);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ConnectHandler: {ex.Message}");
            }
        }


        protected override void DisconnectHandler(ShapeableImageView nativeView)
        {
            base.DisconnectHandler(nativeView);
        }
    }
}
