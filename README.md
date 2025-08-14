# Huffman Cryptology Into Sound Files

![.NET](https://img.shields.io/badge/.NET-6.0-blue)
![License: MIT](https://img.shields.io/badge/License-MIT-green)

---

## İçindekiler

- [Proje Hakkında](#proje-hakkında)
- [Özellikler](#özellikler)
- [Başlangıç](#başlangıç)
- [Kullanım](#kullanım)
- [Teknolojiler](#teknolojiler)
- [Katkıda Bulunma](#katkıda-bulunma)
- [Lisans](#lisans)
- [İletişim](#iletişim)

---

## Proje Hakkında

Bu proje, **C#** ve **NAudio** kütüphanesi kullanarak WAV formatındaki ses dosyalarına Huffman kodlama ve steganografi teknikleriyle veri gizleme işlemi yapar. Ses dosyalarına gizli mesajlar Huffman algoritması ile sıkıştırılarak gömülür ve yeni bir ses dosyası oluşturulur.

---

## Özellikler

- WAV dosyalarını okuma ve binary formata çevirme
- Huffman algoritması ile veri sıkıştırma ve kodlama
- Fibonacci dizisi tabanlı LSB gizleme algoritması
- Gizli mesajı ses dosyasına gömme
- Yeni WAV dosyası oluşturma ve oynatma
- Gizlenen mesajı geri çıkarma
- Türkçe karakter kontrolü (Türkçe karakterler desteklenmemektedir)

---

## Başlangıç

### Gereksinimler

- [.NET Framework](https://dotnet.microsoft.com/en-us/download/dotnet-framework) (Windows Forms uygulaması için)
- [NAudio](https://github.com/naudio/NAudio) kütüphanesi (NuGet üzerinden eklenmeli)

### Kurulum

```bash
git clone https://github.com/leventDemirkaya/huffman-Cryptology-Into-Sound-Files.git
cd huffman-Cryptology-Into-Sound-Files
Visual Studio ile projeyi açın ve NuGet Paket Yöneticisi'nden NAudio paketini yükleyin.

### Kurulum

Programı çalıştırın.  
Ses Dosyası Ekle butonuna tıklayarak WAV formatında ses dosyaları seçin.  
Listeden bir ses dosyası seçin, ses otomatik olarak oynatılacaktır.  
Gizlemek istediğiniz mesajı metin kutusuna yazın (Türkçe karakter kullanmayınız).  
Mesajı Gizle butonuna tıklayın.  
Program, mesajı Huffman algoritması ile sıkıştırıp ses dosyasına gizleyecek ve yeni bir WAV dosyası oluşturacaktır.  
Gizlenen mesajı geri çıkarmak için ilgili butonu kullanabilirsiniz.

---

### Teknolojiler

- C#  
- Windows Forms  
- NAudio kütüphanesi  

---

### Katkıda Bulunma

Katkılarınızı memnuniyetle karşılarız! Lütfen bir sorun açın veya geliştirme önerilerinizi içeren pull request gönderin.

---

### Lisans

Bu proje MIT Lisansı ile lisanslanmıştır. Detaylar için LICENSE dosyasına bakınız.

---

### İletişim

Levent Demirkaya - [GitHub](https://github.com/leventDemirkaya) - leventdemirkaya@outlook.com
