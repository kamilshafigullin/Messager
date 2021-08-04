using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientServerWPF.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private string _IP;
        public string IP
        {
            get
            {
                return _IP;
            }
            set
            {
                _IP = value;
                OnPropertyChanged("IP");
            }
        }

        private int _Port;
        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                _Port = value;
                OnPropertyChanged("Port");
            }
        }
        private string _Nick;
        public string Nick
        {
            get
            {
                return _Nick;
            }
            set
            {
                _Nick = value;
                OnPropertyChanged("Nick");
            }
        }

        public string _Chat;
        public string Chat
        {
            get
            {
                return _Chat;
            }
            set
            {
                _Chat = value;
                OnPropertyChanged("Chat");
            }
        }

        public string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
                OnPropertyChanged("Message");
            }
        }

        TcpClient client;
        StreamReader sr;
        StreamWriter sw;

        public MainViewModel()
        {
            IP = "127.0.0.1";
            Port = 5050;
            Nick = "Kamil";
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (client?.Connected == true)
                        {
                            var line = sr.ReadLine();

                            if (line != null)
                            {
                                Chat += line + "\n";
                            }
                            else
                            {
                                client.Close();
                            }
                        }
                        Task.Delay(10).Wait();
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                }
            });
        }

        public AsyncCommand ConnectCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                client = new TcpClient();
                                client.Connect(IP, Port);
                                sr = new StreamReader(client.GetStream());
                                sw = new StreamWriter(client.GetStream());
                                sw.AutoFlush = true;

                                sw.WriteLine($"Login: {Nick}");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        });
                }, () => client?.Connected == false || client?.Connected == null);
            }
        }

        public AsyncCommand DisconnectCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            sw.Write("Disconnect");
                            client?.Client.Disconnect(false);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                });
            }
        }

        public AsyncCommand SendCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            sw.WriteLine($"{Nick}: {Message}");
                            Message = "";
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }, () => client?.Connected == true && !string.IsNullOrWhiteSpace(Message));
            }
        }
    }
}