using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;



namespace LAN_Chat
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

        bool _ConnectionEstablished = false;

        BackgroundWorker ConnectionWorker;
        BackgroundWorker ReceivingWorker;
        TcpClient client;
        TcpListener listener;
        BinaryReader reader;
        BinaryWriter writer;

        #endregion


        #region Public Properties


        public IPAddress IPLocalAddress { get; set; }


        public string IPLocalString
        {
            get => IPLocalAddress.ToString();
            set { }
        }

        public int ChatBlockHeight { get; set; } = 350;


        public bool ConnectionEstablished
        {
            get { return _ConnectionEstablished; }
            set { _ConnectionEstablished = value; ((RelayCommand)SendCommand).RaiseCanExecuteChanged(); }
        }


        public string LogMessage { get; set; } = "Disconnected";

        public string ButtonConnectContent { get; set; } = "Connect";

        #endregion

        #region Commands

        public ICommand SendCommand { get; set; }

        public ICommand ConnectCommand { get; set; }

        #endregion



        public MainWindowViewModel()
        {
            IPLocalAddress = GetLocalIP();

            SendCommand = new RelayCommand((s) => SendMessage(), () => ConnectionEstablished);
            ConnectCommand = new RelayCommand((s) => SetUpConnection(), () => true);

            ConnectionWorker = new BackgroundWorker();
            ReceivingWorker = new BackgroundWorker();

            ConnectionWorker.WorkerSupportsCancellation = true;
            ReceivingWorker.WorkerSupportsCancellation = true;

            ConnectionWorker.DoWork += ConnectionWorker_DoWork;
            ReceivingWorker.DoWork += ReceivingWorker_DoWork;
        }


        // // // ////////////////////////////////////
        delegate void setPropertyValue();

        private void ConnectionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            LogMessage = "Waiting for connection...";



            LogMessage = "Connected";
        }

        private void ReceivingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetUpConnection()
        {
            if (ConnectionEstablished)
                Disconnect();
            else
            {
                Connect();
                ConnectionEstablished = true;
            }

            //Thread.Sleep(200);

            ButtonConnectContent = ConnectionEstablished ? "Disconnect" : "Connect";

            return;
        }

        private void Connect()
        {
            ConnectionWorker.RunWorkerAsync();
            
            // wait ffor establishing connection
        }

        private void Disconnect()
        {
            if (ConnectionEstablished)
            {
                //writer.Write(Komunikaty.DISCONNECT);

                ConnectionEstablished = false;
                if (client != null)
                    client.Close();

            }
                LogMessage = "Disconnected";

            ConnectionWorker.CancelAsync();
            ReceivingWorker.CancelAsync();
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
