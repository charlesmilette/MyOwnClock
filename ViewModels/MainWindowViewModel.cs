using Livet;
using System;
using System.Windows.Threading;

namespace MyOwnClock.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel() => new DispatcherTimer(
            TimeSpan.FromSeconds(1),
            DispatcherPriority.Normal,
            (object sender, EventArgs e) =>
            {
                DateTime now = DateTime.Now;
                TimeString = now.ToShortTimeString();
                SecondString = now.Second.ToString("00");
                DateString = now.ToLongDateString();
            },
            Dispatcher.CurrentDispatcher
        ).Start();

        private string _TimeString;
        public string TimeString
        {
            get => _TimeString;
            set
            {
                if (_TimeString != value)
                {
                    _TimeString = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _SecondString;
        public string SecondString
        {
            get => _SecondString;
            set
            {
                if (_SecondString != value)
                {
                    _SecondString = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _DateString;
        public string DateString
        {
            get => _DateString;
            set
            {
                if (_DateString != value)
                {
                    _DateString = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}