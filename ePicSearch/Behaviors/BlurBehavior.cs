using Microsoft.Maui.Controls;

namespace ePicSearch.Behaviors
{
    public partial class BlurBehavior 
    {
        public static readonly BindableProperty RadiusProperty = BindableProperty.Create(
            nameof(Radius), typeof(float), typeof(BlurBehavior), 5f, propertyChanged: OnRadiusChanged);

        public float Radius
        {
            get => (float)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        static void OnRadiusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (BlurBehavior)bindable;
            if (behavior.imageView is null)
            {
                return;
            }
            behavior.SetRendererEffect(behavior.imageView, Convert.ToSingle(newValue));
        }

        public async Task AnimateBlurEffect(int maxRadius = 10, int delay = 100)
        {
            for (int i = 0; i <= maxRadius; i++)
            {
                Radius = i;
                await Task.Delay(delay); // Adjust the delay if needed
            }
        }
    }
}
