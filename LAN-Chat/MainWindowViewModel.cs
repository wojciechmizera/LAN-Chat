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

        MainWindow window;

        #endregion


        #region Public Properties


        public IPAddress IPLocalAddress { get; set; }
        public IPAddress IPRemoteAddress { get; set; } = IPAddress.None;

        public int Port { get; set; } = 8000;

        public string IPLocalString
        {
            get => IPLocalAddress.ToString();
            set { }
        }

        public int ChatBlockHeight { get; set; } = 350;

        public bool IsServer { get; set; }

        public string MessagesList { get; set; }
        public string Message { get; set; }

        public bool ConnectionEstablished
        {
            get { return _ConnectionEstablished; }
            set
            {
                ButtonConnectContent = value ? "Disconnect" : "Connect";
                _ConnectionEstablished = value;

                ((RelayCommand)SendCommand).RaiseCanExecuteChanged();
            }
        }


        public string LogMessage { get; set; } = "Disconnected";

        public string ButtonConnectContent { get; set; } = "Connect";

        #endregion

        #region Commands

        public ICommand SendCommand { get; set; }

        public ICommand ConnectCommand { get; set; }

        #endregion



        public MainWindowViewModel(MainWindow w)
        {
            window = w;
            w.Closed += Window_Closed;

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



        private void SetUpConnection()
        {
            if (ConnectionEstablished)
            {
                writer.Write(Komunikaty.DISCONNECT);
                Disconnect();
                ButtonConnectContent = "Connect";
            }
            else
            {
                Connect();
                ButtonConnectContent = "Disconnect";
            }

            return;
        }


        private void Connect()
        {
            ConnectionWorker.RunWorkerAsync();
        }


        private void Disconnect()
        {
            if (ConnectionEstablished)
            {

                Application.Current.Dispatcher.Invoke(new Action(() => ConnectionEstablished = false));
                if (client != null)
                    client.Close();
                if (listener != null)
                    listener.Stop();
            }
            ConnectionWorker.CancelAsync();
            ReceivingWorker.CancelAsync();
        }


        private void ConnectionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Windows.Threading.Dispatcher dispatcher = Application.Current.Dispatcher;

            LogMessage = "Waiting for connection...";

            if (IsServer)
            {
                WaitForClient();

                LogMessage = "Connection requested";
            }
            else
            {
                ConnectToServer();

                LogMessage = "Connection Established / Requesting permission";
            }

            InitializeStreams();
        }


        private void InitializeStreams()
        {
            NetworkStream stream = client.GetStream();
            writer = new BinaryWriter(stream);
            reader = new BinaryReader(stream);

            if (reader.ReadString() == Komunikaty.REQUEST)
            {
                writer.Write(Komunikaty.OK);
                LogMessage = "Connected";
                Application.Current.Dispatcher.Invoke(new Action(() => ConnectionEstablished = true));

                ReceivingWorker.RunWorkerAsync();
            }
            else
            {
                LogMessage = "Network error, disconnected";
                Disconnect();
            }
        }


        private void ConnectToServer()
        {
            client = new TcpClient();
            client.Connect(IPRemoteAddress, Port);
        }

        private void WaitForClient()
        {
            listener = new TcpListener(IPLocalAddress, Port);
            listener.Start();

            while (!listener.Pending())
            {
                if (ConnectionWorker.CancellationPending)
                {
                    Disconnect();
                    return;
                }
                Thread.Sleep(200);
            }
            client = listener.AcceptTcpClient();
        }


        private void ReceivingWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            string text;
            while ((text = reader.ReadString()) != Komunikaty.DISCONNECT)
            {
                MessagesList += "Friend: " + text + "\n";
            }

            LogMessage = "Disconnected";

            Disconnect();

            ButtonConnectContent = "Connect";
        }



        private void SendMessage()
        {
            if (Message == "")
                return;
            if (Message[Message.Length - 1] == '\n')
                Message = Message.TrimEnd('\n');

            writer.Write(Message);
            MessagesList += "Me: " + Message + "\n";
            Message = "";
        }



        private IPAddress GetLocalIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress address = (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip).First();
            return address;

        }


        private void Window_Closed(object sender, EventArgs e)
        {
            Disconnect();
        }



    }
}
