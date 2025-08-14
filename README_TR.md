# 🎵 Audio Steganography - Ses Dosyalarına Gizli Mesaj Gömme

![.NET](https://img.shields.io/badge/.NET-6.0-blue)
![License: MIT](https://img.shields.io/badge/License-MIT-green)

> ✨ **Açıklama:**  
> Bu proje, **C# .NET** kullanarak **LSB (Least Significant Bit)** yöntemiyle **.wav** formatındaki ses dosyalarına gizli metin, resim ve video gibi çeşitli dosya türlerini gömme işlemini gerçekleştirir.  
> Ses dosyalarının orijinal kalitesi korunarak, mesajlar güvenli ve görünmez şekilde saklanır.  
> Ayrıca Huffman kodlama algoritması ile gizlenen verilerin sıkıştırılması ve güvenliği artırılmıştır.

## 📑 İçindekiler  
- [📜 Proje Hakkında](#proje-hakkinda)  
- [⚡ Özellikler](#ozellikler)  
- [🧩 Algoritma ve Teknikler](#algoritma-ve-teknikler)  
- [⚙️ Kurulum ve Kullanım](#kurulum-ve-kullanim)  
- [📊 Örnek Çıktı](#ornek-cikti)  
- [🛠 Teknolojiler](#teknolojiler)  
- [🤝 Katkıda Bulunma](#katkida-bulunma)  
- [📄 Lisans](#lisans)  
- [📬 İletişim](#iletisim)

<a id="proje-hakkinda"></a>
## 📜 Proje Hakkında  
Bu proje, ses dosyalarının en düşük anlamlı bitlerini kullanarak gizli mesajların saklanmasını sağlayan bir ses steganografi uygulamasıdır.  
Metin, resim ve video gibi farklı dosya türleri, Fibonacci dizisi ve LSB yöntemiyle ses dosyasına gömülür.  
Ayrıca Huffman kodlama algoritması ile verilerin sıkıştırılması ve güvenliği sağlanır.

<a id="ozellikler"></a>
## ⚡ Özellikler  
- ✅ LSB yöntemi ile güvenli ve görünmez mesaj gizleme  
- ✅ Metin, resim ve video dosyalarını ses dosyasına gömme desteği  
- ✅ Fibonacci dizisi kullanarak bitlerin yerleştirilmesi  
- ✅ Huffman kodlama ile veri sıkıştırma ve güvenlik  
- ✅ Türkçe karakter desteği  
- ✅ Orijinal ve gizlenmiş ses dosyalarının yönetimi  
- ✅ Kullanıcı dostu Windows Forms arayüzü  
- ✅ Ses dosyası, resim ve video dosyalarının binary dönüşümü ve işlenmesi  

<a id="algoritma-ve-teknikler"></a>
## 🧩 Algoritma ve Teknikler  
1. Ses dosyası byte dizisine dönüştürülür ve binary formata çevrilir.  
2. Gizlenecek dosya türüne göre (metin, resim, video) içerik binary hale getirilir.  
3. Fibonacci dizisi kullanılarak, mesaj bitleri ses dosyasının belirli bitlerine gömülür.  
4. Huffman kodlama algoritması ile gizlenecek veriler sıkıştırılır ve kodlanır.  
5. Değiştirilen binary veriler tekrar byte dizisine çevrilir.  
6. Yeni .wav dosyası oluşturulur ve gizlenmiş mesaj oynatıcıda dinlenebilir.  
7. Gizlenen mesaj, ses dosyasından okunarak Huffman kodları çözülür ve doğrulanabilir.

<a id="kurulum-ve-kullanim"></a>
## ⚙️ Kurulum ve Kullanım  
1. 📥 Projeyi klonlayın:  
   ```bash
   git clone https://github.com/leventDemirkaya/audio-steganography.git
   cd audio-steganography
2. 💻 Projeyi Visual Studio veya dotnet CLI ile açın ve çalıştırın.
3. 🎵 Program arayüzünden .wav dosyası seçin, gizlenecek metin, resim veya video dosyasını seçin ve mesajı gömün.
4. 🔊 Oluşan yeni ses dosyasını dinleyerek veya mesajı çıkararak doğrulayabilirsiniz.

<a id="ornek-cikti"></a>
## 📊 Örnek Çıktı
### 🎙️ Seçilen Ses Dosyası: 
example.wav

### 📝 Gizlenecek Mesaj: 
"Merhaba, bu gizli bir mesajdır."

### 🔢 Binary Mesaj Parçaları: 
01001101 01100101 01110010 01101000 01100001 01100010 01100001 ...

### 🎧 Oluşan Gizlenmiş Ses Dosyası: 
gizlenen1.wav

### 📬 Çıkarılan Mesaj: 
"Merhaba, bu gizli bir mesajdır."

<a id="teknolojiler"></a>
## 🛠 Teknolojiler
- 💻 C#
- 🖥 .NET Framework / .NET 6.0
- 🎨 Windows Forms
- 🎵 NAudio kütüphanesi

<a id="katkida-bulunma"></a>
## 🤝 Katkıda Bulunma
💡 Katkılarınız çok değerlidir!
- 🐛 Hata bildirmek için Issues sekmesini kullanabilirsiniz.
- 🚀 Geliştirme önerileri için Pull Request açabilirsiniz.

<a id="lisans"></a> 
## 📄 Lisans
📝 Bu proje MIT Lisansı ile korunmaktadır. Detaylar için LICENSE dosyasına bakabilirsiniz.

<a id="iletisim"></a> 
## 📬 İletişim
📧 leventdemirkaya@outlook.com
