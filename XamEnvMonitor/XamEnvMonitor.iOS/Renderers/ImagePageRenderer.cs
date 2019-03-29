using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamEnvMonitor;
using XamEnvMonitor.iOS.Renderers;


[assembly: ExportRenderer(typeof(MainView), typeof(ImagePageRenderer))]

namespace XamEnvMonitor.iOS.Renderers
{
    public class ImagePageRenderer : PageRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }


            var page = e.NewElement as ContentPage;

            UIGraphics.BeginImageContext(View.Frame.Size);
            UIImage i = UIImage.FromFile(page.BackgroundImage);
            i = i.Scale(View.Frame.Size);

            View.BackgroundColor = UIColor.FromPatternImage(i);
        }
    }
}
