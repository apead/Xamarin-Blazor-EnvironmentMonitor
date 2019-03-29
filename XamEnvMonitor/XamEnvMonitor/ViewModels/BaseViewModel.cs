using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PropertyChangingEventArgs = Xamarin.Forms.PropertyChangingEventArgs;
using PropertyChangingEventHandler = Xamarin.Forms.PropertyChangingEventHandler;

namespace XamEnvMonitor.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /* public const string IconPropertyName = "Icon";
        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

    */

        protected void SetProperty<TU>(
            ref TU backingStore, TU value,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TU>.Default.Equals(backingStore, value))
                return;

            OnPropertyChanging(propertyName);

            backingStore = value;

            OnPropertyChanged(propertyName);
        }

        #region INotifyPropertyChanging implementation
        public event PropertyChangingEventHandler PropertyChanging;
        #endregion

        public void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }


        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
      }
}
