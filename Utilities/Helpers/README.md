# Utilities Helpers - .NET 8.0

Bu proje, .NET 8.0 uygulamaları için sık kullanılan yardımcı araçları barındırır. İçerisinde **ASP.NET Core Identity** ile uyumlu bir **Email Gönderme Yardımcısı** (`EmailSender`) ve çeşitli formatlardaki (CSV, Excel, JSON) dosyaları okumak için bir **Veri İçe Aktarma Yardımcısı** (`DataImporters`) implementasyonları bulunmaktadır.

## 📋 İçindekiler

- [Genel Bakış](#genel-bakış)
- [Yapı](#yapı)
- [Kurulum](#kurulum)
- [Konfigürasyon (Email)](#konfigürasyon-email)
- [Kullanım](#kullanım)
  - [EmailSender Kullanımı](#emailsender-kullanımı)
  - [DataImporters Kullanımı](#dataimporters-kullanımı)
- [Örnekler (Email)](#örnekler-email)
- [Hata Yönetimi](#hata-yönetimi)
- [Güvenlik](#güvenlik)
- [Best Practices](#best-practices)

## 🎯 Genel Bakış

### EmailSender Nedir?

`EmailSender`, **IEmailSender** interface'ini implemente eden bir sınıftır. SMTP sunucusu aracılığıyla asenkron olarak HTML formatında e-postalar gönderir.

**Özellikleri:**
✅ **IEmailSender Interface Uyumluluğu** - ASP.NET Core Identity ile entegre çalışır  
✅ **SMTP Desteği** - Gmail, Outlook, özel mail sunucuları v.b. ile uyumlu  
✅ **HTML Email Desteği** - Formatlı e-postalar gönderin  
✅ **Asenkron İşlem** - Non-blocking email gönderimi  
✅ **SSL/TLS Şifrelemesi** - Güvenli bağlantı  

### DataImporters Nedir?

`DataImporters`, `Stream` üzerinden asenkron olarak CSV, Excel ve JSON dosyalarını okuyarak strongly-typed listelere (`IEnumerable<T>`) dönüştüren statik bir sınıftır.

**Özellikleri:**
✅ **Çoklu Format Desteği** - CSV (`CsvHelper`), Excel (`MiniExcel`), ve JSON (`System.Text.Json`) dosyalarını destekler  
✅ **Asenkron Akış Okuma** - Büyük dosyalar için bellek dostu stream tabanlı okuma  
✅ **Case-Insensitive JSON** - Esnek JSON model eşleştirmesi ve enum dönüşüm desteği  

## 📦 Yapı

### Proje Dosyaları

```text
Utilities/
├── Helpers/
│   ├── DataImporters.cs       # CSV, Excel ve JSON okuma işlemleri
│   └── EmailSender.cs         # Email gönderme implementasyonu
└── Models/
    └── EmailSettings.cs       # SMTP ayarları modeli
```

## 🔧 Kurulum

### Gereksinimler

- **.NET 8.0** veya daha üstü

### NuGet Paketleri

Gerekli paketleri projenize ekleyin:

```bash
# EmailSender için
dotnet add package Microsoft.AspNetCore.Identity.UI

# DataImporters için
dotnet add package CsvHelper
dotnet add package MiniExcel
```

## ⚙️ Konfigürasyon (Email)

### 1. appsettings.json İçine Ekleyin

```json
{
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UserName": "your-email@gmail.com",
    "Password": "your-app-password",
    "DisplayName": "Proje Adı"
  }
}
```

### 2. Program.cs'te Yapılandırın

```csharp
using Utilities.Helpers;
using Utilities.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplicationBuilder.CreateBuilder(args);

// EmailSettings'i configuration'dan oku
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);

// EmailSender'ı dependency injection konteynerine kaydet
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();
```

*(Diğer SMTP sunucu ayarları ve App Password oluşturma detayları için projenin dökümantasyonunu inceleyebilirsiniz.)*

## 💻 Kullanım

### 📧 EmailSender Kullanımı

```csharp
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailSender _emailSender;

    public EmailController(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail(string email, string subject, string message)
    {
        await _emailSender.SendEmailAsync(email, subject, message);
        return Ok(new { message = "E-posta başarıyla gönderildi." });
    }
}
```

### 📁 DataImporters Kullanımı

Veri içe aktarılacak model örneği:
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

#### CSV İçe Aktarma
```csharp
[HttpPost("import-csv")]
public async Task<IActionResult> ImportCsv(IFormFile file)
{
    using var stream = file.OpenReadStream();
    var products = await DataImporters.ImportCsvAsync<Product>(stream);
    
    return Ok(products);
}
```

#### Excel İçe Aktarma
```csharp
[HttpPost("import-excel")]
public async Task<IActionResult> ImportExcel(IFormFile file)
{
    using var stream = file.OpenReadStream();
    var products = await DataImporters.ImportExcelAsync<Product>(stream);
    
    return Ok(products);
}
```

#### JSON İçe Aktarma
```csharp
[HttpPost("import-json")]
public async Task<IActionResult> ImportJson(IFormFile file)
{
    using var stream = file.OpenReadStream();
    var products = await DataImporters.ImportJsonAsync<Product>(stream);
    
    return Ok(products);
}
```

## 📖 Örnekler (Email)

### Hoş Geldiniz E-postası

```csharp
public async Task SendWelcomeEmailAsync(string email, string fullName)
{
    var subject = "Hoş Geldiniz!";
    var htmlMessage = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px;'>
            <h1>Hoş Geldiniz!</h1>
            <p>Merhaba <strong>{fullName}</strong>,</p>
            <p>Platformumuza kaydolduğunuz için çok teşekkürler.</p>
        </div>
    ";

    await _emailSender.SendEmailAsync(email, subject, htmlMessage);
}
```

## ⚠️ Hata Yönetimi

### EmailSender Hataları

| Hata | Açıklama | Çözüm |
|------|----------|-------|
| `SmtpException` | SMTP sunucusu hatası | Host, port ve kimlik bilgilerini kontrol edin |
| `ArgumentNullException` | Boş parametre | Email, subject veya message boş olmadığını kontrol edin |
| `TimeoutException` | Timeout hatası | Port numarasını ve SSL ayarlarını kontrol edin |

### DataImporters Hataları

| Hata | Açıklama | Çözüm |
|------|----------|-------|
| `CsvHelperException` | CSV format veya eşleştirme hatası | Başlıkların model ile uyuştuğunu kontrol edin |
| `JsonException` | Hatalı JSON formatı | Dosya içeriğinin geçerli bir JSON dizisi olduğundan emin olun |
| `InvalidDataException` | Desteklenmeyen Excel formatı | Dosyanın geçerli bir `.xlsx` veya `.xls` dosyası olduğundan emin olun |

## 🔒 Güvenlik & Best Practices

### ✅ Yapılması Gerekenler

- **Email Şifreleri:** Appsettings.json'a doğrudan şifre yazmayın. User Secrets veya Azure Key Vault kullanın.
- **Stream Yönetimi:** `DataImporters` kullanırken `IFormFile` stream'lerinin veya dosya stream'lerinin `using` bloğu ile sarmalandığından (Dispose edildiğinden) emin olun.
- **Asenkron İşlemler:** Sınıflardaki tüm metotlar asenkrondur, `await` kullanarak çağırın.
- **Validasyon:** Email gönderimi öncesi e-posta adreslerini ve DataImporters'a dosya göndermeden önce dosya uzantısını/MIME tipini doğrulayın.
- **Loglama:** Olası aktarım ve SMTP hatalarını loglayın.

### ❌ Yapılmaması Gerekenler

- **Synchronous çağrılar yapmayın** - `.Result` veya `.Wait()` kullanarak UI thread'ini bloklamayın.
- **Stream'leri açık bırakmayın** - Dosya okuma işlemi bittikten sonra Stream'leri mutlaka kapatın (DataImporters kendisi Stream'i dispose etmez, kaynağı yöneten sorumludur).

## 📝 Lisans

Bu proje MIT Lisansı altında dağıtılmaktadır.