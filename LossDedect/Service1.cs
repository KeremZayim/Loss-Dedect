using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Timers;

namespace LossDedect
{
    public partial class Service1 : ServiceBase
    {
        private Timer _timer;
        private const string LogFilePath = "C:\\loss_log.txt";
        private double _lastPacketLoss = 0;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _timer = new Timer(10000); // 10 saniyede bir kontrol
                _timer.Elapsed += CheckPacketLoss;
                _timer.Start();

                WriteLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - İnternet takip servisi başlatıldı.");
            }
            catch (Exception ex)
            {
                WriteLog("Servis başlatılırken hata oluştu: " + ex.Message);
            }
        }

        protected override void OnStop()
        {
            _timer?.Stop();
            WriteLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - İnternet takip servisi durduruldu.");
        }

        private void CheckPacketLoss(object sender, ElapsedEventArgs e)
        {
                Ping ping = new Ping();
                PingOptions options = new PingOptions { DontFragment = true };
                byte[] buffer = new byte[32];
                int timeout = 1000;
                int packetCount = 4;
                int lostPackets = 0;

                for (int i = 0; i < packetCount; i++)
                {
                    PingReply reply = ping.Send("www.google.com", timeout, buffer, options);
                    if (reply.Status != IPStatus.Success)
                    {
                        lostPackets++;
                    }
                }

                double packetLoss = (double)lostPackets / packetCount * 100;

                if (packetLoss > 1.0 && Math.Abs(packetLoss - _lastPacketLoss) > 0.1)
                {
                    WriteLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Paket Kaybı: %{packetLoss:F2}");
                    _lastPacketLoss = packetLoss;
                }
        }

        private void WriteLog(string message)
        {
        
                File.AppendAllText(LogFilePath, message + Environment.NewLine);        
        }
    }
}
