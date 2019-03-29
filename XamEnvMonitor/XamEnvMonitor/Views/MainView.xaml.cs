using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamEnvMonitor.ViewModels;


namespace XamEnvMonitor
{
    public partial class MainView : ContentPage
    {
        private MainViewModel _viewModel;

        public MainView()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            this.BindingContext = _viewModel;
            ;

            // this.BackgroundImage = "igniteback.png";
        }

        protected async override void OnAppearing()
        {
            if (_viewModel != null)
                await Task.Run(() => _viewModel.RefreshCommand.Execute(null));
            
        }
    }
}