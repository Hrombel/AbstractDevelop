using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Gemini.Framework.Behaviors
{
    public class KeyboardFocusBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            if (!AssociatedObject.IsLoaded)
                AssociatedObject.Loaded += (sender, e) =>
                {
                    if (AssociatedObject.IsVisible)
                        try { Keyboard.Focus(AssociatedObject); }
                        catch { }
                };
            else
                Keyboard.Focus(AssociatedObject);

            base.OnAttached();
        }
    }
}