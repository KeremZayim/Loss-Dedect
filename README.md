# Loss-Dedect
İnternet bağlantımda yaşadığım sorun yüzünden paket kaybımı hangi tarihte ve hangi değerlere çıktığını kayıt altına almak için yazdığım basit bir ***Loss*** değeri kayıt servisi.

# Çalışma Mantığı
Gerçek zamanlı kontrol şeklinde çalışan bu servis internet loss değerinizin %1 in üstüne çıktığı **2025-02-23 14:35:12 - Paket Kaybı: %2.35** tarzında kayıt oluşturmaktadır. Kayıt dosya konumu ***C:\loss_log.txt*** dosyasıdır.

# Kurulum
Setup'ı kurun.
CMD -> Yönetici Olarak Çalıştır -> ***sc create PacketLossDedector binPath="C:\Program Files (x86)\KZ\Packet Loss Dedector\LossDedect.exe"***
(**Eğer dlosya konumunu değiştirirseniz CMD komutundaki yolu da değiştirin!**)
