﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO; // Ses Dosyasından Okuma İşleminde Kullanılmaktadır.
using NAudio.Wave; // Wav Dosyasından Okuma İşleminde Kullanılmaktadır.

namespace sesVerileriÜzerineVeriGizleme
{
    public partial class Form1 : Form
    {
        #region Global Değişkenler
        List<string> sifirlar = new List<string> { "", "0", "00", "000", "0000", "00000", "000000", "0000000", "00000000" }; // ASCII koduna göre 8 bite tamamlanacak.
        List<string> path = new List<string>(); // Eklenen ses dosyalarının adresi bulunmaktadır
        List<string> sesDosyasiIkiTabani = new List<string>(); //sesDosyasındaki verinin binary değerleri bulunmaktadır
        List<string> saklanacakMesajIkiTabani = new List<string>(); //gizlenecek metinin karakterlerinin binary karşılıkları bulunmaktadır
        List<int> kullanilanIndisler = new List<int>(); // sırasıyla gizleme yapılan bitlerin bulunduğu indisler
        byte[] sesDosyasiOnTabani = null; // gizlenmiş mesajı içeren .wav formatına çevrilmede kullanılacak değişken
        byte[,] kareSesDosyasi = null, kareGoruntuDosyasi = null,kareVideoDosyasi=null;
        int calistirilmaSayisi = 0; // kaç kez forms'un çalıştığının takibi için kullanılacaktır.
        List<string> goruntuDosyasiIkiTabani = new List<string>(); //gizlenecek goruntunun isleminde kullanilacak olan degisken
        static List<int> levNumbers = new List<int>();//görüntü dosyasında indis gizlemesinde kullanılacak değişken
        List<string> videoDosyasiIkiTabani = new List<string>(); // gizlenecek video dosyası isleminde kullanilacak olan degisken
        static string[] huffmanKodTablosu = null;
        static string[] huffmanKarakter = null;
        static string huffmanResimVideo="";
        #endregion
        public Form1()
        {
            InitializeComponent();
        }
        private void bt_SesDosyasiEkle_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Ses Dosyaları|*.wav;|Tüm Dosyalar|*.*";
            openFileDialog1.Title = "Ses Dosyası Seç";
            openFileDialog1.ShowDialog();
            for (int i = 0; i < openFileDialog1.SafeFileNames.Length; i++)
                if (!lstbx_SesDosyalari.Items.Contains(openFileDialog1.SafeFileNames[i].ToString())) // Eğer Listbox'ta seçilen sesDosyası mevcut değilse ekleyecek
                {
                    lstbx_SesDosyalari.Items.Add(openFileDialog1.SafeFileNames[i].ToString()); // SafeFileNames ile sadece dosyaAdı gelmekte, adresi gelmemektedir.
                    path.Add(openFileDialog1.FileNames[i].ToString()); // sesDosyasının adresini de FileNames metodu ile path listesine ekle
                }
        }
        private void lstbx_SesDosyalari_SelectedIndexChanged(object sender, EventArgs e)
        {
            bt_rdBtSecimi.Enabled = true; bt_rdBtSecimi.Visible = true;
            rdbt_metinVerisi.Enabled = true; rdbt_metinVerisi.Visible = true;
            rdbt_goruntu.Enabled = true; rdbt_goruntu.Visible = true;
            rdbt_video.Enabled = true; rdbt_video.Visible = true;
        }
        private void IkiTabaninaCevir(byte[] dizi, byte dosyaTipi, byte sesGoruntuVideo)
        {
            if (dosyaTipi == 1) // orijinalDosya indisi 1 olarak belirlendi.
            {
                if (sesGoruntuVideo == 1)
                {
                    for (int i = 0; i < dizi.Length; i++)
                    { 
                        string temp = Convert.ToString(dizi[i], 2); // decimalden binary'e dönüşüm
                        if (temp.Length < 8) // 8 bitten kısa ise 8 bite ulaşması için sifirlar listesinden ekleme olacak
                            temp = sifirlar[8 - temp.Length] + temp;
                        sesDosyasiIkiTabani.Add(temp); // sonuç listesi kullanılarak veriGizleme yapılacaktır.
                    }
                    levNumbersCreator(sesDosyasiIkiTabani.Count());
                    MessageBox.Show("Ses Dosyası Binary Tabana Dönüştürülmüştür.", "İşlem Başarılı");
                }
                else if(sesGoruntuVideo == 2)
                {
                    for (int i = 0; i < dizi.Length; i++)
                    {
                        string temp = Convert.ToString(dizi[i], 2); // decimalden binary'e dönüşüm
                        if (temp.Length < 8) // 8 bitten kısa ise 8 bite ulaşması için sifirlar listesinden ekleme olacak
                            temp = sifirlar[8 - temp.Length] + temp;
                        goruntuDosyasiIkiTabani.Add(temp); // sonuç listesi kullanılarak veriGizleme yapılacaktır.
                    }
                    MessageBox.Show("Goruntu Dosyası Binary Tabanına Dönüştürülmüştür.", "İşlem Başarılı");
                }
                else if(sesGoruntuVideo == 3)
                {
                    for (int i = 0; i < dizi.Length; i++)
                    {
                        string temp = Convert.ToString(dizi[i], 2); // decimalden binary'e dönüşüm
                        if (temp.Length < 8) // 8 bitten kısa ise 8 bite ulaşması için sifirlar listesinden ekleme olacak
                            temp = sifirlar[8 - temp.Length] + temp;
                        videoDosyasiIkiTabani.Add(temp); // sonuç listesi kullanılarak veriGizleme yapılacaktır.
                    }
                    MessageBox.Show("Video Dosyası Binary Tabanına Dönüştürülmüştür.", "İşlem Başarılı");
                }
            }
            else if (dosyaTipi == 2)
            {
                StreamWriter sw = null;
                int sayac = 0;
                sw = File.AppendText(@"gizlenmisMesajlar.txt"); // gizlenmişMesaj.txt oluşturulup 8 bit binary formdaki gizlenmiş mesaj değerleri tutulacak.
                String mesaj = txt_GizlenecekMetin.Text;
                sw.Write($"Gizlenecek Mesaj: {mesaj}\nBinary Olarak Kodlanmış Hali :\n");
                for (int i = 0; i < mesaj.Length; i++)
                {
                    string temp = Convert.ToString(mesaj[i], 2); // dosyaTipi=1 parametresine göre tek değişiklik, dizi[i] değil de mesaj[i] parametre alınıyor.
                    if (temp.Length < 8) // 8 bitten kısa ise 8 bite ulaşması için sifirlar listesinden ekleme olacak
                        temp = sifirlar[8 - temp.Length] + temp;
                    saklanacakMesajIkiTabani.Add(temp);
                    sw.WriteLine(((++sayac) + ". eleman ---> " + mesaj[i] + " : " + temp));
                }
                sw.WriteLine("*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-");
                MessageBox.Show("Gizlenecek Mesaj Dosyaya Yazılmıştır.", "İşlem Başarılı");
                sw.Close();
            }
        }
        private void OnTabaninaCevir()
        {
            sesDosyasiOnTabani = new byte[sesDosyasiIkiTabani.Count];
            for (int i = 0; i < sesDosyasiIkiTabani.Count; i++)
            {
                string temp = ""; byte deger = 0; // iki tabanındaki dosyada değişiklik yapmamak için temp değişkeni tanımlandı
                for (int j = sesDosyasiIkiTabani[i].Length - 1; j >= 0; j--) // taban dönüşümü yapmak için string tersten yazıldı
                    temp += sesDosyasiIkiTabani[i][j];
                for (int j = 0; j < temp.Length; j++)
                {
                    if (temp[j] == '1') // 1 değerine sahipse binary --> decimal dönüşümü 2^j(i'nin basamak sayısı) olarak yapılacak.         
                        deger += (byte)(Math.Pow(2, j));
                }
                sesDosyasiOnTabani[i] = deger;
            }
        }
        private void bt_MesajGizle_Click(object sender, EventArgs e)
        {
            if (txt_GizlenecekMetin.Text != "") // Gizlenecek metin girilmişse çalışacak
            {
                IkiTabaninaCevir(null, 2, 1); // eğer boş metin değilse çalışacak
                txt_GizlenecekMetin.Enabled = false;
                bt_MesajGizle.Enabled = false;
            }
            else MessageBox.Show("Gizlenecek Metin Girilmedi, Tekrar Deneyiniz.", "HATA !");
            if (saklanacakMesajIkiTabani.Count != 0) // Gizlenecek metin girilmiş ve 2 tabanına çevrimi sağlanmışsa
            {
                LSBGizlemeYap(1);
                OnTabaninaCevir();
                WavDosyasiOlustur(1);
                DegiskenTemizle();
            }
        }
        private void LSBGizlemeYap(byte sesGoruntuVideo)
        {
            if (sesGoruntuVideo == 1)
            {
                int levIndis = 0, gizlenmisIndis = 0; // fiboIndis kaç karaktere kadar indexOutOfRange hatası yenmeden çalışacağını tutmakta, gizlenmiş indiste ise kaçıncı 8 bitlik karakterde olunduğunun bilgisi tutulmaktadır.
                String tempOrijinal, tempGizlenmisMesaj = saklanacakMesajIkiTabani[gizlenmisIndis++]; // tempOrijinal, orijinal ses dosyasında ; tempGizlenmişMesaj ise gizlenecek metin üzerindeki bit işlemlerinde kullanılacaktır.
                for (int i = 0; levNumbers[i] < sesDosyasiIkiTabani.Count; i++)
                    levIndis++; // orijinal ses dosyasında, fibonacci serisi kullanılarak ulaşılabilecek en son bitin indisi tutulmaktadır.
                int yazilacakEleman = saklanacakMesajIkiTabani[0].Length * saklanacakMesajIkiTabani.Count;
                for (int i = 0; i < yazilacakEleman; i++) // bit bazlı işlem yapıldığı için 8 bite dönüştürülen gizlenecekMesaj kadar dönecektir.
                {
                    int tutulanFiboDegeri = levNumbers[i];
                    if (i >= levIndis) // fibonacci sınırları orijinal mesaj için outOfRange hatası veriyorsa, düzenleme yapılması gerekli.
                        tutulanFiboDegeri = (levNumbers[i] % sesDosyasiIkiTabani.Count);
                    int tempSayac = i % 8;  // i sayacını döngü değişkeni olduğu için güncellenemez, o yüzden tempSayac oluşturuldu.
                    tempOrijinal = sesDosyasiIkiTabani[(int)tutulanFiboDegeri];   // Fibonacci sayısına denk gelen orijinal 8-bitlik dizginin LSB haricindeki dizgisi alınacak.
                    if (i % 8 == 0 && i > 0)    // gizlenecekMesajın ilgili karakterinin bütün bitleri gizlenip sıradaki karaktere geçilecekse
                        tempGizlenmisMesaj = saklanacakMesajIkiTabani[gizlenmisIndis++];    // gizlenmişIndis değeri arttırılarak sıradaki 8 bit tempGizlenmişMesaj içerisine yazılır.
                    char tempKarakter = tempGizlenmisMesaj[tempSayac];  // Karşılaştırılacak ve uyumsuz olursa değiştirilecek bit değeridir. tempSayac indis olarak alındı, çünkü i%8'den dolayı 0 değerinden başlıyor.
                    if (tempOrijinal[tempOrijinal.Length - 1] != tempKarakter)  // eğer orijinal LSB ile tempKarakter farklıysa, LSB değiştirilecek
                    {
                        tempOrijinal = tempOrijinal.Substring(0, tempOrijinal.Length - 1);  // orijinalMesajın sadece LSB'si değişeceği için ilk 7 biti korundu.
                        tempOrijinal += tempKarakter;    // gizlenecekKarakter orijinal mesaja ekleniyor.
                        sesDosyasiIkiTabani[(int)tutulanFiboDegeri] = tempOrijinal;   // Listedeki indisteki değer de güncelleniyor.
                    } // else yazılmasına gerek olmuyor, değiştirilme olmadığı için orijinal mesajın indisinin tutulması yeterli oluyor.     
                    kullanilanIndisler.Add((int)tutulanFiboDegeri);
                }
            }
            else if(sesGoruntuVideo==2)
            {
                int levIndis = 0, gizlenmisIndis = 0; // fiboIndis kaç karaktere kadar indexOutOfRange hatası yenmeden çalışacağını tutmakta, gizlenmiş indiste ise kaçıncı 8 bitlik karakterde olunduğunun bilgisi tutulmaktadır.
                String tempOrijinal, tempGizlenmisMesaj = goruntuDosyasiIkiTabani[gizlenmisIndis++]; // tempOrijinal, orijinal ses dosyasında ; tempGizlenmişMesaj ise gizlenecek metin üzerindeki bit işlemlerinde kullanılacaktır.
                for (int i = 0; levNumbers[i] < sesDosyasiIkiTabani.Count; i++)
                    levIndis++; // orijinal ses dosyasında, fibonacci serisi kullanılarak ulaşılabilecek en son bitin indisi tutulmaktadır.
                int yazilacakEleman = goruntuDosyasiIkiTabani[0].Length * goruntuDosyasiIkiTabani.Count;
                for (int i = 0; i < yazilacakEleman; i++) // bit bazlı işlem yapıldığı için 8 bite dönüştürülen gizlenecekMesaj kadar dönecektir.
                {
                    int tutulanFiboDegeri = levNumbers[i];
                    if (i >= levIndis) // fibonacci sınırları orijinal mesaj için outOfRange hatası veriyorsa, düzenleme yapılması gerekli.
                        tutulanFiboDegeri = (levNumbers[i] % sesDosyasiIkiTabani.Count);
                    int tempSayac = i % 8;  // i sayacını döngü değişkeni olduğu için güncellenemez, o yüzden tempSayac oluşturuldu.
                    tempOrijinal = sesDosyasiIkiTabani[(int)tutulanFiboDegeri];   // Fibonacci sayısına denk gelen orijinal 8-bitlik dizginin LSB haricindeki dizgisi alınacak.
                    if (i % 8 == 0 && i > 0)    // gizlenecekMesajın ilgili karakterinin bütün bitleri gizlenip sıradaki karaktere geçilecekse
                        tempGizlenmisMesaj = goruntuDosyasiIkiTabani[gizlenmisIndis++];    // gizlenmişIndis değeri arttırılarak sıradaki 8 bit tempGizlenmişMesaj içerisine yazılır.
                    char tempKarakter = tempGizlenmisMesaj[tempSayac];  // Karşılaştırılacak ve uyumsuz olursa değiştirilecek bit değeridir. tempSayac indis olarak alındı, çünkü i%8'den dolayı 0 değerinden başlıyor.
                    if (tempOrijinal[tempOrijinal.Length - 1] != tempKarakter)  // eğer orijinal LSB ile tempKarakter farklıysa, LSB değiştirilecek
                    {
                        tempOrijinal = tempOrijinal.Substring(0, tempOrijinal.Length - 1);  // orijinalMesajın sadece LSB'si değişeceği için ilk 7 biti korundu.
                        tempOrijinal += tempKarakter;    // gizlenecekKarakter orijinal mesaja ekleniyor.
                        sesDosyasiIkiTabani[(int)tutulanFiboDegeri] = tempOrijinal;   // Listedeki indisteki değer de güncelleniyor.
                    } // else yazılmasına gerek olmuyor, değiştirilme olmadığı için orijinal mesajın indisinin tutulması yeterli oluyor.     
                    kullanilanIndisler.Add((int)tutulanFiboDegeri);
                }
            }
            else if(sesGoruntuVideo==3)
            {
                int levIndis = 0, gizlenmisIndis = 0; // fiboIndis kaç karaktere kadar indexOutOfRange hatası yenmeden çalışacağını tutmakta, gizlenmiş indiste ise kaçıncı 8 bitlik karakterde olunduğunun bilgisi tutulmaktadır.
                String tempOrijinal, tempGizlenmisMesaj = videoDosyasiIkiTabani[gizlenmisIndis++]; // tempOrijinal, orijinal ses dosyasında ; tempGizlenmişMesaj ise gizlenecek metin üzerindeki bit işlemlerinde kullanılacaktır.
                for (int i = 0; levNumbers[i] < sesDosyasiIkiTabani.Count; i++)
                    levIndis++; // orijinal ses dosyasında, fibonacci serisi kullanılarak ulaşılabilecek en son bitin indisi tutulmaktadır.
                int yazilacakEleman = videoDosyasiIkiTabani[0].Length * videoDosyasiIkiTabani.Count;
                for (int i = 0; i < yazilacakEleman; i++) // bit bazlı işlem yapıldığı için 8 bite dönüştürülen gizlenecekMesaj kadar dönecektir.
                {
                    int tutulanFiboDegeri = levNumbers[i];
                    if (i >= levIndis) // fibonacci sınırları orijinal mesaj için outOfRange hatası veriyorsa, düzenleme yapılması gerekli.
                        tutulanFiboDegeri = (levNumbers[i] % sesDosyasiIkiTabani.Count);
                    int tempSayac = i % 8;  // i sayacını döngü değişkeni olduğu için güncellenemez, o yüzden tempSayac oluşturuldu.
                    tempOrijinal = sesDosyasiIkiTabani[(int)tutulanFiboDegeri];   // Fibonacci sayısına denk gelen orijinal 8-bitlik dizginin LSB haricindeki dizgisi alınacak.
                    if (i % 8 == 0 && i > 0)    // gizlenecekMesajın ilgili karakterinin bütün bitleri gizlenip sıradaki karaktere geçilecekse
                        tempGizlenmisMesaj = videoDosyasiIkiTabani[gizlenmisIndis++];    // gizlenmişIndis değeri arttırılarak sıradaki 8 bit tempGizlenmişMesaj içerisine yazılır.
                    char tempKarakter = tempGizlenmisMesaj[tempSayac];  // Karşılaştırılacak ve uyumsuz olursa değiştirilecek bit değeridir. tempSayac indis olarak alındı, çünkü i%8'den dolayı 0 değerinden başlıyor.
                    if (tempOrijinal[tempOrijinal.Length - 1] != tempKarakter)  // eğer orijinal LSB ile tempKarakter farklıysa, LSB değiştirilecek
                    {
                        tempOrijinal = tempOrijinal.Substring(0, tempOrijinal.Length - 1);  // orijinalMesajın sadece LSB'si değişeceği için ilk 7 biti korundu.
                        tempOrijinal += tempKarakter;    // gizlenecekKarakter orijinal mesaja ekleniyor.
                        sesDosyasiIkiTabani[(int)tutulanFiboDegeri] = tempOrijinal;   // Listedeki indisteki değer de güncelleniyor.
                    } // else yazılmasına gerek olmuyor, değiştirilme olmadığı için orijinal mesajın indisinin tutulması yeterli oluyor.     
                    kullanilanIndisler.Add((int)tutulanFiboDegeri);
                }
            }
        }
        private void txt_GizlenecekMetin_MouseClick(object sender, MouseEventArgs e)
        {
            txt_GizlenecekMetin.Clear(); // Gizlenebilecek karakter üst limiti mesajı kullanıcı tarafından okunduktan sonra silinmesi sağlanacaktır.
        }
        private String GizlenenMesajiGoster(byte sesGoruntuVideo)
        {
            if (sesGoruntuVideo == 1)
            {
                lbl_GizlenenMesaj.Visible = true; txt_GizlenenMesaj.Visible = true;
                string sonuc = "", tempTersten = "", tempNormal = "";
                char karakter; byte deger = 0, limit = 8;
                for (int i = 1; i <= kullanilanIndisler.Count; i++)
                {
                    karakter = sesDosyasiIkiTabani[kullanilanIndisler[i - 1]][sesDosyasiIkiTabani[kullanilanIndisler[i - 1]].Length - 1];
                    tempTersten += karakter;
                    if (i % limit == 0)
                    {
                        for (int j = tempTersten.Length - 1; j >= 0; j--)
                            tempNormal += tempTersten[j];
                        for (int j = 0; j < tempNormal.Length; j++)
                        {
                            if (tempNormal[j] == '1')
                                deger += (byte)Math.Pow(2, j);
                        }
                        sonuc += (char)deger;
                        tempTersten = ""; tempNormal = ""; deger = 0;
                    }
                }
                shiftKeyOlustur(sonuc, 0);
                return sonuc;
            }
            else if (sesGoruntuVideo==2)
            {
                txt_GizlenenMesaj.Visible = false; lbl_GizlenenMesaj.Visible = false;
                byte[] resimPikselleri = new byte[goruntuDosyasiIkiTabani.Count];
                StreamWriter sw;
                string tempTersten = "", tempNormal = "";
                char karakter; byte deger = 0, limit = 8;
                int sayac = 0;
                for (int i = 1; i <= kullanilanIndisler.Count; i++)
                {
                    karakter = sesDosyasiIkiTabani[kullanilanIndisler[i - 1]][sesDosyasiIkiTabani[kullanilanIndisler[i - 1]].Length - 1];
                    tempTersten += karakter;
                    if (i % limit == 0)
                    {
                        for (int j = tempTersten.Length - 1; j >= 0; j--)
                            tempNormal += tempTersten[j];
                        for (int j = 0; j < tempNormal.Length; j++)
                        {
                            if (tempNormal[j] == '1')
                                deger += (byte)Math.Pow(2, j);
                        }
                        resimPikselleri[sayac++] = deger;
                        tempTersten = ""; tempNormal = ""; deger = 0;
                    }
                }
                String path = Application.StartupPath + "\\gizlenenResim.txt";
                String yaz = Convert.ToBase64String(resimPikselleri);
                sw = new StreamWriter(path);
                sw.Write(yaz);
                sw.Close();
                gizlenenMesajToString(1,path); //Resim
                return path;
            }
            else
            {
                txt_GizlenenMesaj.Visible = false; lbl_GizlenenMesaj.Visible = false;
                byte[] videoPikselleri= new byte[videoDosyasiIkiTabani.Count];
                StreamWriter sw;
                string tempTersten = "", tempNormal = "";
                char karakter; byte deger = 0, limit = 8;
                int sayac = 0;
                for (int i = 1; i <= kullanilanIndisler.Count; i++)
                {
                    karakter = sesDosyasiIkiTabani[kullanilanIndisler[i - 1]][sesDosyasiIkiTabani[kullanilanIndisler[i - 1]].Length - 1];
                    tempTersten += karakter;
                    if (i % limit == 0)
                    {
                        for (int j = tempTersten.Length - 1; j >= 0; j--)
                            tempNormal += tempTersten[j];
                        for (int j = 0; j < tempNormal.Length; j++)
                        {
                            if (tempNormal[j] == '1')
                                deger += (byte)Math.Pow(2, j);
                        }
                        videoPikselleri[sayac++] = deger;
                        tempTersten = ""; tempNormal = ""; deger = 0;
                    }
                }
                String path = Application.StartupPath + "\\gizlenenVideo.txt";
                String yaz = Convert.ToBase64String(videoPikselleri);
                sw = new StreamWriter(path);
                sw.Write(yaz);
                sw.Close();
                gizlenenMesajToString(2,path); //Video
                return path;
            }
        }
        private void DegiskenTemizle()
        {
            // İlk çalıştırmadan sonra yeni değerlerin işlenebilmesi için temizlenmesi gereklidir.
            sesDosyasiIkiTabani.Clear();
            saklanacakMesajIkiTabani.Clear();
            kullanilanIndisler.Clear();
            sesDosyasiOnTabani = null;
            goruntuDosyasiIkiTabani.Clear();
            videoDosyasiIkiTabani.Clear();
            kareSesDosyasi = null; kareGoruntuDosyasi = null; kareVideoDosyasi = null;
            huffmanKodTablosu = null;
            huffmanKarakter = null;
            huffmanResimVideo = "";
        }
        private void WavDosyasiOlustur(byte sesGoruntuVideo)
        {
            String dosyaAdi = $"gizlenen{++calistirilmaSayisi}.wav";
            WavDosyasiIcerikOlustur(dosyaAdi);
            if (sesGoruntuVideo == 1)
                txt_GizlenenMesaj.Text = GizlenenMesajiGoster(1);
            else if (sesGoruntuVideo == 2)
                arrayToStego(GizlenenMesajiGoster(2), 1);
            else if (sesGoruntuVideo == 3)
                arrayToStego(GizlenenMesajiGoster(3), 2);
            sesOynatici.URL = path[lstbx_SesDosyalari.SelectedIndex];
            gizlenmisMesajOynatici.Visible = true;
            gizlenmisMesajOynatici.URL = dosyaAdi;
            gizlenmisMesajOynatici.Ctlcontrols.play();
            sesOynatici.Ctlcontrols.play();
        }       
        private void bt_rdBtSecimi_Click(object sender, EventArgs e)
        {
            try
            {
                if (calistirilmaSayisi != 0) // Başka bir seçim yapılınca hali hazırda çalan ses dosyaları duracak
                {
                    gizlenmisMesajOynatici.Ctlcontrols.stop();
                    sesOynatici.Ctlcontrols.stop();
                }
                else signTheSoftware();
                byte[] sesDosyasi;
                sesOynatici.URL = path[lstbx_SesDosyalari.SelectedIndex]; // listbox'tan seçilen indeksteki sesDosyasının adresi sesOynatıcıya gönderilir.
                sesOynatici.Ctlcontrols.play();
                sesDosyasi = File.ReadAllBytes(path[lstbx_SesDosyalari.SelectedIndex]); // sesDosyasının decimal tabanına dönüşümü sağlanır
                IkiTabaninaCevir(sesDosyasi, 1, 1); // Gizleme işlemi için 8 bit uzunluğunda binary tabanında sayıya dönüştürülür.
                if (rdbt_metinVerisi.Checked)
                {
                    lbl_GizlenecekMetin.Visible = true; txt_GizlenecekMetin.Visible = true; txt_GizlenecekMetin.Enabled = true;
                    bt_MesajGizle.Visible = true; bt_MesajGizle.Enabled = true;
                    kareSesDosyasi = KareMatriseCevir(sesDosyasi);
                }
                else if (rdbt_goruntu.Checked)
                {
                    byte[] goruntuDosyasi = fileTypeToArray(1);
                    IkiTabaninaCevir(goruntuDosyasi, 1, 2);
                    stegoDosyayaYaz(1);
                    OnTabaninaCevir();
                    WavDosyasiOlustur(2);
                    kareGoruntuDosyasi = KareMatriseCevir(sesDosyasi);
                    DegiskenTemizle();
                    lbl_GizlenecekMetin.Visible = false; txt_GizlenecekMetin.Visible = false; txt_GizlenecekMetin.Enabled = false;
                    bt_MesajGizle.Visible = false; bt_MesajGizle.Enabled = false;
                }
                else if (rdbt_video.Checked)
                {
                    byte[] videoDosyasi = fileTypeToArray(2);
                    IkiTabaninaCevir(videoDosyasi, 1, 3);
                    stegoDosyayaYaz(2);
                    OnTabaninaCevir();
                    WavDosyasiOlustur(3);
                    kareVideoDosyasi = KareMatriseCevir(sesDosyasi);
                    DegiskenTemizle();
                    lbl_GizlenecekMetin.Visible = false; txt_GizlenecekMetin.Visible = false; txt_GizlenecekMetin.Enabled = false;
                    bt_MesajGizle.Visible = false; bt_MesajGizle.Enabled = false;
                }
            }
            catch (Exception ex) // listbox içerisinde geçersiz bir alana tıklayınca oluşan exception'ı kontrol etmek için.
            {
                MessageBox.Show("Listeden Seçim Sırasında Hata Oluştu, Tekrar Deneyiniz : "+ex.Message);
                lbl_GizlenecekMetin.Visible = false; txt_GizlenecekMetin.Visible = false; txt_GizlenecekMetin.Enabled = false;
                bt_MesajGizle.Visible = false; bt_MesajGizle.Enabled = false; // gizleme yapılacak orijinal dosya doğru seçilmezse gizlenecekMetin özellikleri görünür olmayacak.
            }
        }
        private byte[,] KareMatriseCevir(byte[] dizi)
        {
            List<byte> tempDizi = dizi.ToList();
            int length = tempDizi.Count;
            int boyut = (int)Math.Ceiling(Math.Sqrt(tempDizi.Count));
            if (length < boyut * boyut)
            {
                boyut++;
                length = (boyut * boyut) - length;
                for (int i = 0; i < length; i++)
                    tempDizi.Add(0);
            }
            byte[,] kareMatris = new byte[boyut, boyut];
            int tmp = 0;
            for (int i = 0; i < kareMatris.GetLength(0); i++)
                for (int j = 0; j < kareMatris.GetLength(1); j++)
                    kareMatris[i, j] = tempDizi[tmp++];
            return kareMatris;
        }
        private void stegoDosyayaYaz(byte goruntuVideo)
        {
            if (goruntuVideo == 1)
                LSBGizlemeYap(2);
            else if(goruntuVideo==2)
                LSBGizlemeYap(3);
        }
        private void levNumbersCreator(int ustSinir)
        {
            for (int i = 0; i <= ustSinir*8; i+=2) // başlangıç kısımda stego veri yogunlugu
                levNumbers.Add(i);
        } 
        private void arrayToStego(String path, byte goruntuVideo)
        {
            if (goruntuVideo == 1)
            {
                if (File.Exists(@path))
                {
                    try
                    {
                        byte[] yeniResimByte = Convert.FromBase64String(File.ReadAllText(path));
                        using (MemoryStream ms = new MemoryStream(yeniResimByte))
                        {
                            Image okunanResim = Image.FromStream(ms);
                            okunanResim.Save($"stegoResim{calistirilmaSayisi}.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        MessageBox.Show("Resim Dosyası Oluşturuldu");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Resim Oluşturma Sırasında Hata Oluştu : {e.Message}");
                    }
                }
            }
            else if(goruntuVideo==2)
            {
                String benoit_ = $@"stegoVideo{calistirilmaSayisi}.mp4"; // Oluşturulacak yeni video dosyasının adı
                if (File.Exists(@path)) // Parametre olarak gelen path'te dosya bulunuyorsa
                {
                    try
                    {
                        byte[] yeniVideoByte = Convert.FromBase64String(File.ReadAllText(path));
                        using (MemoryStream ms = new MemoryStream(yeniVideoByte))
                            File.WriteAllBytes(benoit_, ms.ToArray());
                        MessageBox.Show("Video Dosyası Oluşturuldu");
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show($"Video Oluşturma Sırasında Hata Oluştu : {e.Message}");
                    }
                }
            }
        }
        private void WavDosyasiIcerikOlustur(string dosyaAdi)
        {
            string dosyaYolu = path[lstbx_SesDosyalari.SelectedIndex];
            int channels, bitsPerSample, sampleRate, dataSize, fileSize;
            using (WaveFileReader waveFileReader = new WaveFileReader(dosyaYolu))
            {
                channels = waveFileReader.WaveFormat.Channels;
                bitsPerSample = waveFileReader.WaveFormat.BitsPerSample;
                sampleRate = waveFileReader.WaveFormat.SampleRate;
                dataSize = (int)waveFileReader.Length - 44; // WAV dosyasının başlık boyutu genellikle 44 byte'dır.
                fileSize = (int)waveFileReader.Length;
            }
            byte[] riffHeader = new byte[44];
            Encoding.ASCII.GetBytes("RIFF").CopyTo(riffHeader, 0);
            BitConverter.GetBytes(fileSize - 8).CopyTo(riffHeader, 4);
            Encoding.ASCII.GetBytes("WAVE").CopyTo(riffHeader, 8);
            Encoding.ASCII.GetBytes("fmt ").CopyTo(riffHeader, 12);
            BitConverter.GetBytes(16).CopyTo(riffHeader, 16); // PCM için 'fmt ' chunk boyutu
            BitConverter.GetBytes((short)1).CopyTo(riffHeader, 20); // Audio formatı (PCM için 1)
            BitConverter.GetBytes(channels).CopyTo(riffHeader, 22); // Kanal sayısı
            BitConverter.GetBytes(sampleRate).CopyTo(riffHeader, 24); // Örnekleme hızı
            BitConverter.GetBytes(sampleRate * channels * bitsPerSample / 8).CopyTo(riffHeader, 28); // Byte hızı
            BitConverter.GetBytes((short)(channels * bitsPerSample / 8)).CopyTo(riffHeader, 32); // Block align
            BitConverter.GetBytes(bitsPerSample).CopyTo(riffHeader, 34); // Bits per sample
            Encoding.ASCII.GetBytes("data").CopyTo(riffHeader, 36);
            BitConverter.GetBytes(dataSize).CopyTo(riffHeader, 40); // Veri bölümünün boyutu
            byte[] wavVerisi = new byte[riffHeader.Length + sesDosyasiOnTabani.Length];
            Array.Copy(riffHeader, wavVerisi, riffHeader.Length);
            Array.Copy(sesDosyasiOnTabani, 0, wavVerisi, riffHeader.Length, sesDosyasiOnTabani.Length);
            File.WriteAllBytes(dosyaAdi, wavVerisi);
        }
        private byte[] fileTypeToArray(byte goruntuVideo)
        {
            try
            {
                if (goruntuVideo == 1)
                {
                    byte[] goruntuDosyasi = null;
                    openFileDialog1.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*";
                    openFileDialog1.Title = "Resim Dosyası Seç";
                    openFileDialog1.ShowDialog();
                    String path = openFileDialog1.FileName;
                    goruntuDosyasi = File.ReadAllBytes(path);
                    kareGoruntuDosyasi = KareMatriseCevir(goruntuDosyasi);    
                    return goruntuDosyasi;
                }
                else
                {
                    byte[] videoDosyasi = null;
                    openFileDialog1.Filter = "Video Dosyaları|*.mp4;*.avi;*.mkv|Tüm Dosyalar|*.*";
                    openFileDialog1.Title = "Video Dosyası Seç";
                    openFileDialog1.ShowDialog();
                    String path = openFileDialog1.FileName;
                    videoDosyasi = File.ReadAllBytes(path);
                    return videoDosyasi;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Dosya seçimi sırasında hata oluştu.");
                return null;
            }
        }
        private void signTheSoftware()
        {
            StreamWriter sw = new StreamWriter(@"signature.txt");
            for (int i = 45; i > 3; i -= 2)
            {
                for (int j = 0; j < i; j++)
                {
                    sw.Write("/");
                }
                sw.WriteLine();
            }
            sw.WriteLine("@Coded And Designed by Levent DEMIRKAYA, Copyrights Reserved.");
            for (int i = 0; i < 45; i++)
            {
                sw.Write("/");
            }
            sw.Close();
        }
        private (List<char>, List<int>) frekansTablosuOlustur(String plainText)
        {
            List<int> frekanslar = new List<int>();
            List<char> karakterler = new List<char>();
            for (int i = 0; i < plainText.Length - 1; i++)
            {
                int frekans = 1; // i.indeksteki karakter en az bir kere bulunacak
                if (!karakterler.Contains(plainText[i])) // eğer karakterin frekansı incelenmemişse döngüye girecek
                    for (int j = (i + 1); j < plainText.Length; j++)
                    {
                        if (plainText[i] == plainText[j])
                            frekans++;
                    }
                else // karaktere daha önceden tarama yapılmışsa frekans değişkeni
                    // listeye ekleme yapılabilmek için flag gibi kullanıyor ve 0 atanıyor.
                    frekans = 0;
                if (frekans != 0) // Frekans flag olarak atanmadıysa listelere ekleme yap.
                {
                    karakterler.Add(plainText[i]);
                    frekanslar.Add(frekans);
                }
            }
            #region Eger plainText'in sonKarakteri tek bir frekansa sahipse
            if (!karakterler.Contains(plainText[plainText.Length - 1]))
            {
                karakterler.Add(plainText[plainText.Length - 1]);
                frekanslar.Add(1);
            }
            #endregion
            return (karakterler, frekanslar);
        }
        private (List<char>, List<int>) siralamaYap(String plainText)
        {
            List<char> karakterler = new List<char>(); //huffmanMetoduna return edilecek tuple'da 1.degisken
            List<int> frekanslar = new List<int>(); //huffmanMetoduna return edilecek tuple'da 2. degisken
            var sonucTuple = frekansTablosuOlustur(plainText); // karakter~frekans verileri sonucTuple içinde
            for (int i = 0; i < sonucTuple.Item1.Count;) // döngü karakter sayısı kadar dönecek
            {
                char karakter = sonucTuple.Item1[i];
                int minimum = sonucTuple.Item2[i], indis = i; // en küçük frekansa sahip karakterin indisi tutuluyor.
                for (int j = (i + 1); j < sonucTuple.Item1.Count; j++)
                {
                    if (minimum > sonucTuple.Item2[j])
                    {
                        minimum = sonucTuple.Item2[j];
                        karakter = sonucTuple.Item1[j];
                        indis = j;
                        if (minimum == 1) break; //1'den düşük frekans olmayacağı için backtracking
                    }
                }
                karakterler.Add(karakter); // en küçük frekansa sahip karakter listeye yazılıyor
                frekanslar.Add(minimum);
                sonucTuple.Item1.RemoveAt(indis); // karakter ve frekans incelendiği için tuple'dan siliniyor.
                sonucTuple.Item2.RemoveAt(indis);
            }
            string path = Application.StartupPath + "\\frekansTablosu.txt";
            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int i = 0; i < karakterler.Count; i++)
                {
                    sw.WriteLine($"{i + 1}.karakter {karakterler[i]} : {frekanslar[i]}");
                }
            }
            return (karakterler, frekanslar);
        }
        private String shiftKeyOlustur(String plainText,byte secim)
        {
            String shiftKey = "";
            List<String> keys = new List<string>(); // tuple'dan char olarak gelen karakterler string'e çevrilecek.
            List<String> huffmanCreator = new List<string>();
            foreach (char key in siralamaYap(plainText).Item1)
                keys.Add(key + "");
            List<int> frekanslar = siralamaYap(plainText).Item2;
            int length = keys.Count; // keys listesinde silme işlemi olacağı için gereken length önceden alındı.
            for (int i = 0; frekanslar.Count != 1;)
            {
                int j = (i + 1);
                int tempFrekans = (frekanslar[i] + frekanslar[j]); // ikişerli şekilde frekanslar toplandı.
                String tempKarakter = keys[i] + keys[j]; // Huffman'a göre iki karakter birleştirildi
                int indis = j; // birleşen karakterin indisi tespit edilecek
                if (tempFrekans < frekanslar[frekanslar.Count - 1]) // eğer listenin en sonuna gelmeyecekse karakterin indisi bulunacak
                {
                    while (frekanslar[indis] <= tempFrekans)
                    {
                        indis++;
                    }
                    keys.Insert(indis, tempKarakter);
                    frekanslar.Insert(indis, tempFrekans);
                }
                else // karakterin frekansı bütün karakterlerden büyükse listenin sonuna ekle
                {
                    keys.Add(tempKarakter);
                    frekanslar.Add(tempFrekans);
                }
                huffmanCreator.Add(keys[i]);
                keys.RemoveAt(i);
                huffmanCreator.Add(keys[i]);
                keys.RemoveAt(i); // incelenen karakterleri frekansıyla birlikte sil
                frekanslar.RemoveAt(i); frekanslar.RemoveAt(i);
            }
            if (secim > 0)
            {
                for (int i = 0; i < plainText.Length; i++)
                {
                    int temp = (i % keys[0].Length); // shiftKey'i girilen plainText ile aynı uzunluğa sahip oluncaya dek güncelle
                    shiftKey += keys[0][temp];
                }
            } //Kriptografik Anahtar Oluşturmak İçin
            huffmanCreator.Add(huffmanCreator[huffmanCreator.Count - 2] + huffmanCreator[huffmanCreator.Count - 1]);
            int arananKelimeSayisi = huffmanCreator[huffmanCreator.Count - 1].Count();
            huffmanKodTablosu = new String[arananKelimeSayisi];
            huffmanKarakter = new String[arananKelimeSayisi];
            huffmanKodOlustur(huffmanCreator,arananKelimeSayisi);
            return shiftKey;
        }
        private void huffmanKodOlustur(List<string>birlestirmeler,int arananKelime)
        {
            List<string> huffmanKodlari = new List<string>();
            List<string> karakterler = new List<string>();
            int turSayisi = 0,i=birlestirmeler.Count-2,bulunan=0;
            while (arananKelime > 0)
            {
                int atanacakKod = (int)(Math.Pow(2, ++turSayisi));
                String temphfKod, karakter;
                bool tekKarakterMi;
                while (atanacakKod > 0)
                {
                    if (arananKelime == 0) break;
                    if (atanacakKod % 2 == 0)
                    {
                        var huffmanBool = huffmanKodAta(1, i, huffmanKodlari,birlestirmeler);
                        temphfKod = huffmanBool.Item1;
                        tekKarakterMi = huffmanBool.Item2;
                        karakter = huffmanBool.Item3;
                        huffmanKodlari.Add(temphfKod);
                        karakterler.Add(karakter);
                        if (tekKarakterMi)
                        {
                            arananKelime--;
                            huffmanKarakter[bulunan] = karakter;
                            huffmanKodTablosu[bulunan++] = temphfKod;
                        }
                        i--;
                    }
                    else if (atanacakKod % 2 == 1)
                    {
                        var huffmanBool = huffmanKodAta(0, i, huffmanKodlari,birlestirmeler);
                        temphfKod = huffmanBool.Item1;
                        tekKarakterMi = huffmanBool.Item2;
                        karakter = huffmanBool.Item3;
                        huffmanKodlari.Add(temphfKod);
                        karakterler.Add(karakter);
                        if (tekKarakterMi)
                        {
                            arananKelime--;
                            huffmanKarakter[bulunan] = karakter;
                            huffmanKodTablosu[bulunan++] = temphfKod;
                        }
                        i--;
                    }
                    atanacakKod--;
                }
            }
            string path = Application.StartupPath + "\\huffman.txt";
            using(StreamWriter sw = new StreamWriter(path))
            {
                for (int j = 0; j < huffmanKodTablosu.Length; j++)
                    sw.WriteLine($"{j + 1}.karakter {huffmanKarakter[j]} : {huffmanKodTablosu[j]}");
            }
        }
        private void gizlenenMesajToString(byte goruntuVideo,String goruntuVideoAdresi)
        {
            String gizlenecekMesaj = "",pathHuffman="";
            if(goruntuVideo==1)pathHuffman = Application.StartupPath + "\\huffmanResim.txt";
            else pathHuffman = Application.StartupPath + "\\huffmanVideo.txt";
            if (File.Exists(goruntuVideoAdresi))
                try
                {
                    using (StreamReader sr = new StreamReader(goruntuVideoAdresi))
                    {
                        int karakter;
                        while ((karakter = sr.Read()) != -1)
                            gizlenecekMesaj += (char)karakter;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Dosya okuma sırasında bir hata oluştu: " + ex.Message);
                }
            else
                MessageBox.Show("Dosya bulunamadı: " + goruntuVideoAdresi);
            shiftKeyOlustur(gizlenecekMesaj, 0);
            huffmanToStego(goruntuVideo,gizlenecekMesaj, pathHuffman);
        }
        private void huffmanToStego(byte goruntuVideo,string gizlenecekMesaj, string pathHuffman)
        {
            using (StreamWriter sw = new StreamWriter(pathHuffman))
            {
                int indis = 0;
                for (int i = 0; i < gizlenecekMesaj.Length; i++)
                {
                    for (int j = 0; j < huffmanKarakter.Length; j++)
                    {
                        if (gizlenecekMesaj[i] == Convert.ToChar(huffmanKarakter[j]))
                        {
                            indis = j;
                            break;
                        }
                    }
                    sw.Write(huffmanKodTablosu[indis] + "-");
                }
            } // HuffmanKodları TXT'ye yazılıyor
                if (File.Exists(pathHuffman))
                {
                    using (StreamReader sr = new StreamReader(pathHuffman))
                    {
                        String hfKod = "";
                        int karakter;
                        while ((karakter = sr.Read()) != -1)
                        {
                            char currentChar = (char)karakter;
                            if (currentChar == '-')
                            {
                                for (int i = 0; i < huffmanKodTablosu.Length; i++)
                                {
                                    if (hfKod == huffmanKodTablosu[i])
                                    {
                                        huffmanResimVideo += huffmanKarakter[i];
                                        hfKod = "";
                                        break;
                                    }
                                }
                            }
                            else
                                hfKod += currentChar;
                        }
                    }
                } //HuffmanResimVideo içinde normal char değerleri bulunmakta
                else
                    MessageBox.Show("Dosya bulunamadı: " + pathHuffman);
            String path="";
            if (goruntuVideo==1)
            {
                try
                {
                    path = Application.StartupPath + "\\hfKoddanResim.txt";
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        sw.Write(huffmanResimVideo);
                    }
                    byte[] huffmanResimByte = Convert.FromBase64String(File.ReadAllText(path));
                    using (MemoryStream ms = new MemoryStream(huffmanResimByte))
                    {
                        Image okunanResim = Image.FromStream(ms);
                        okunanResim.Save($"huffmanResim{calistirilmaSayisi}.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    MessageBox.Show("Huffman Kodlarından Resim Dosyası Oluşturuldu");
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Huffman Kodlarından Resim Oluşturma Sırasında Hata Oluştu : {e.Message}");
                }
            } // Görüntü Oluşturma
            else
            {
                String hfvideo_ = $@"huffmanStegoVideo{calistirilmaSayisi}.mp4"; // Oluşturulacak yeni video dosyasının adı
                try
                {
                    byte[] hfVideoByte = Convert.FromBase64String(huffmanResimVideo);
                    using (MemoryStream ms = new MemoryStream(hfVideoByte))
                        File.WriteAllBytes(hfvideo_, ms.ToArray());
                    MessageBox.Show("Huffman Kodlarından Video Dosyası Oluşturuldu");
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Huffman Kodlarından Video Dosyası Oluşturma Sırasında Hata Oluştu : {e.Message}");
                }
            } //Video Oluşturma
        }
        private (String,bool,String) huffmanKodAta(byte secim,int indis, List<string> huffmanKodlari, List<string> birlestirmeler)
        {
            String huffmanKod="",tempHuff="";
            bool tekKarakterMi = true;
            int index = 0;
            if (secim % 2 == 1)
            {
                tempHuff = birlestirmeler[indis];
                for (int i = indis + 1; i <= birlestirmeler.Count - 1; i++)
                {
                    index++;
                    if(birlestirmeler[i].Contains(tempHuff))
                    {
                        if (huffmanKodlari.Count>=2)
                        {
                            huffmanKod = huffmanKodlari[huffmanKodlari.Count - index];
                            huffmanKod += "1";
                            if (tempHuff.Count() == 1) tekKarakterMi = true;
                            else tekKarakterMi = false;
                            break;
                        }
                        else
                        {
                            huffmanKod += "1";
                            if (tempHuff.Count() == 1) tekKarakterMi = true;
                            else tekKarakterMi = false;
                        }
                    }                    
                }
            } // Sağ taraf 1 eklenecek
            else
            {
                tempHuff = birlestirmeler[indis];
                for (int i = indis + 1; i <= birlestirmeler.Count - 1; i++)
                {
                    index++;
                    if (birlestirmeler[i].Contains(tempHuff))
                    {
                        if (huffmanKodlari.Count>=2)
                        {
                            huffmanKod = huffmanKodlari[huffmanKodlari.Count - index];
                            huffmanKod += "0";
                            if (tempHuff.Count() == 1) tekKarakterMi = true;
                            else tekKarakterMi = false;
                            break;
                        }
                        else
                        {
                            huffmanKod += "0";
                            if (tempHuff.Count() == 1) tekKarakterMi = true;
                            else tekKarakterMi = false;
                        }
                    }
                }
            } // Sol taraf 0 eklenecek
            return (huffmanKod,tekKarakterMi,tempHuff);
        }
    }
}