using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;

namespace Communicator
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        #region INottifyProperty interface

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Private members

        private IPAddress _IPLocalAddress;
        private bool _ConnectionExtablished;


        #endregion


        #region Public Properties


        public IPAddress IPLocalAddress { get => _IPLocalAddress; set => _IPLocalAddress = value; }


        public string IPLocalString
        {
            get => _IPLocalAddress.ToString();
            set { }
        }

        public int ChatBlockHeight { get; set; } = 350;


        public bool ConnectionEstablished
        {
            get => _ConnectionExtablished;
            set
            {
                _ConnectionExtablished = value;
                ((RelayCommand)SendCommand).RaiseCanExecuteChanged();
            }
        }

        public string LogMessage
        {
            get;
            set;
        } = "Disconnected";



        #endregion

        #region Commands

        public ICommand SendCommand { get; set; }

        public ICommand ConnectCommand { get; set; }

        #endregion



        public MainWindowViewModel()
        {
            IPLocalAddress = GetLocalIP();

            SendCommand = new RelayCommand((s) => SendMessage(), () => ConnectionEstablished);

            ConnectCommand = new RelayCommand((s) => Connect(), () => true);
            

        }

        private void Connect()
        {
            MessageBox.Show("Trying to connect");
            ConnectionEstablished = !ConnectionEstablished;

            return;
        }

        private void SendMessage()
        {
            MessageBox.Show("Sending message");
            return;
        }

        private IPAddress GetLocalIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress address = (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip).First();
            return address;

        }
    }
}
