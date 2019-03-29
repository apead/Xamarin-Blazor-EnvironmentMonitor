using System;
using Xamarin.Forms.Platform.GTK;

namespace Microcharts.GTK.Sample.GTK
{
	public class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			SkiaForms.Gtk2.Init.Include();
			Gtk.Application.Init();
			Xamarin.Forms.Forms.Init();
			var app = new XamEnvMonitor.App();
			var window = new FormsWindow();
			window.LoadApplication(app);
			window.SetApplicationTitle("Gtk Environment Monitor");
			window.Show();
			Gtk.Application.Run();
		}
	}
}