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
        private PerformanceCounter _sentPackets;
        private PerformanceCounter _lostPackets;
        private const string LogFilePath = "C:\\loss_log.txt";
        private double _lastPacketLoss = 0; // Önceki paket kaybı değeri

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                string adapterName = GetNetworkAdapterName();
                if (adapterName == "Unknown")
                {
                    WriteLog("Aktif ağ adaptörü bulunamadı!");
                    return;
                }

                _sentPackets = new PerformanceCounter("Network Interface", "Packets Sent/sec", adapterName);
                _lostPackets = new PerformanceCounter("Network Interface", "Packets Outbound Errors", adapterName);

                _timer = new Timer(1000); // 1 saniyede bir kontrol (Anlık değişimleri yakalamak için)
                _timer.Elapsed += CheckPacketLoss;
                _timer.Start();

                WriteLog("İnternet takip servisi başlatıldı.");
            }
            catch (Exception ex)
            {
                WriteLog("Servis başlatılırken hata oluştu: " + ex.Message);
            }
        }

        protected override void OnStop()
        {
            _timer?.Stop();
            WriteLog("İnternet takip servisi durduruldu.");
        }
        private void CheckPacketLoss(object sender, ElapsedEventArgs e)
        {
            try
            {
                float sent = _sentPackets.NextValue();
                float lost = _lostPackets.NextValue();

                if (sent > 0)
                {
                    double packetLoss = (lost / sent) * 100;

                    // Eğer paket kaybı %1'in üzerine çıktıysa ve önceki değere göre değiştiyse log kaydı yap
                    if (packetLoss > 1.0 && Math.Abs(packetLoss - _lastPacketLoss) > 0.1)
                    {
                        WriteLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Paket Kaybı: %{packetLoss:F2}");
                        _lastPacketLoss = packetLoss; // En son kayıt edilen değeri güncelle
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("Paket kaybı kontrol edilirken hata oluştu: " + ex.Message);
            }
        }

        private string GetNetworkAdapterName()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    return ni.Description;
                }
            }
            return "Unknown";
        }

        private void WriteLog(string message)
        {
            try
            {
                File.AppendAllText(LogFilePath, message + Environment.NewLine);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("InternetMonitorService", "Log yazma hatası: " + ex.Message, EventLogEntryType.Error);
            }
        }
    }
}
