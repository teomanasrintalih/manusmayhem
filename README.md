# Manus Mayhem 🖐🎮

**Manus Mayhem**, MPU6050 sensörü ile el hareketlerini ve dönüşlerini okuyarak Unity içerisinde gerçek zamanlı olarak kontrol sağlayan bir projedir. Projede hem kablolu hem de Bluetooth üzerinden bağlantı desteği bulunmaktadır. El hareketlerine göre kamera yönlendirme ve parmak verisi analizi yapılmaktadır.

---

## 📦 Proje İçeriği

### 🎮 Unity Scripts
- **GyroCamera.cs**  
  Unity kamerasını MPU6050 sensöründen gelen kuaternion verisi ile döndürür.  
  Serial port üzerinden veri okunur ve kamera rotasyonu dinamik olarak güncellenir.

- **ESP32_com.cs**  
  Parmak verilerini (baş, işaret, orta, yüzük, serçe) ve kuaternion verilerini Bluetooth ya da serial port üzerinden okuyarak projede kullanılmasını sağlar.

### 🔌 Arduino / ESP32 Kodları
- MPU6050 ile IMU (6-axis) sensör verileri alınır.
- DMP (Digital Motion Processor) üzerinden quaternion hesaplaması yapılır.
- Parmak bükülmeleri ADC pinlerinden okunur.
- Bluetooth veya USB Serial üzerinden veri Unity'ye gönderilir.

---

## 🚀 Başlarken

### 📁 Gereksinimler
- Unity 2021 veya üzeri
- Arduino IDE
- ESP32 veya Arduino (MPU6050 ile)
- MPU6050 sensörü
- 5 Adet Flex sensör
- Seri bağlantı için USB kablosu veya Bluetooth bağlantısı

### 🔧 Kurulum

#### 1. Arduino/ESP32 Hazırlığı
- Arduino IDE'ye aşağıdaki kütüphaneleri yükleyin:
  - `MPU6050`
  - `I2Cdev`
  - `BluetoothSerial` (ESP32 için)
- Sensör bağlantılarını yapın:
  - MPU6050: `SDA`, `SCL`, `GND`, `VCC`
  - Parmak sensörleri: `A0-A4` gibi analog pinlere
- Arduino kodlarını yükleyin.

#### 2. Unity Tarafı
- Unity projesine `GyroCamera.cs` ve `ESP32_com.cs` scriptlerini ekleyin.
- `Main Camera` objesine `GyroCamera.cs` scriptini atayın.
- COM port isimlerini Unity Inspector'dan ayarlayın (örn: `COM5`).
- Parmak verileri için gerekli objeleri oluşturun (isteğe göre el modeli, animasyonlar vs.).

---

## 📡 Veri Formatı

Arduino tarafı Unity'e şu şekilde veri yollar:

### 📐 Quaternion (Rotation) Formatı:


Unity tarafında bu veriler parse edilerek kamera dönüşü ve el animasyonları için kullanılır.

---

## 🔄 Tuşlar

| Tuş        | İşlev                      |
|------------|----------------------------|
| `R`        | GyroCamera seri port seçimi |
| `F2`        | Eldiven seri port seçimi |

---

## 🐞 Hata Ayıklama

- `showDebugLogs` özelliğini **true** yaparak Unity konsoluna veri akışını görebilirsiniz.
- COM bağlantı hatası alıyorsanız doğru portu seçtiğinizden emin olun.
- MPU6050 düzgün çalışmıyorsa kabloları ve `DMP` ayarlarını kontrol edin.

---

## ✍️ Geliştirici

🛠️ Proje Sahibi ve Geliştiriciler:
**Teoman Asrın Talih, Furkan Murat Göncü**  

---

## ⚠️ Lisans

Bu proje kişisel kullanım ve araştırma amacıyla oluşturulmuştur. İzinsiz ticari amaçlı kullanımı yasaktır.

---

## ❤️ Katkı

Katkıda bulunmak isterseniz:
1. Forklayın 🍴
2. Branch oluşturun (`feature/yenilik`)
3. Değişiklikleri commit'leyin
4. Pull Request gönderin

---

## 📽️ Demo

https://github.com/user-attachments/assets/7996154f-6f19-4320-8381-27ebdbeebba7

---

## 🧠 Ekstra Notlar

- Unity'de quaternion verileri `(w, x, y, z)` olarak gelir. Ancak bazı sistemlerde dönüş farklılıkları olabilir. Gerekirse `Quaternion` yapısı ters çevrilerek test edilmelidir.
- Parmak verileri için threshold değerleri dinamik olarak değiştirilebilir.
- Gelecekte el modellemesi ve fiziksel animasyonlar eklenebilir.


