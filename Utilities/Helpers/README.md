# Email Sender Helper - .NET 8.0

Bu proje, **ASP.NET Core Identity** ile uyumlu bir **Email Gönderme Yardımcısı** (Email Helper) implementasyonudur. SMTP protokolünü kullanarak kolayca e-posta gönderebilirsiniz.

## 📋 İçindekiler

- [Genel Bakış](#genel-bakış)
- [Yapı](#yapı)
- [Kurulum](#kurulum)
- [Konfigürasyon](#konfigürasyon)
- [Kullanım](#kullanım)
- [Örnekler](#örnekler)
- [Hata Yönetimi](#hata-yönetimi)
- [Güvenlik](#güvenlik)
- [Best Practices](#best-practices)

## 🎯 Genel Bakış

### EmailSender Nedir?

`EmailSender`, **IEmailSender** interface'ini implementasyon yapan bir sınıftır. SMTP sunucusu aracılığıyla asenkron olarak HTML formatında e-postalar gönderir.

### Özellikleri

✅ **IEmailSender Interface Uyumluluğu** - ASP.NET Core Identity ile entegre çalışır  
✅ **SMTP Desteği** - Gmail, Outlook, özel mail sunucuları v.b. ile uyumlu  
✅ **HTML Email Desteği** - Formatlı e-postalar gönderin  
✅ **Asenkron İşlem** - Non-blocking email gönderimi  
✅ **SSL/TLS Şifrelemesi** - Güvenli bağlantı  
✅ **Dependency Injection** - Kolayca entegrasyon  
✅ **Options Pattern** - Ayarları configuration'dan çek  

## 📦 Yapı

### Proje Dosyaları

```
Utilities/
├── Helpers/
│   └── EmailSender.cs         # Email gönderme implementasyonu
└── Models/
    └── EmailSettings.cs       # SMTP ayarları modeli
```

### EmailSettings Sınıfı

SMTP sunucusu bağlantısı için gerekli bilgileri tutar.

```csharp
public class EmailSettings
{
    public string Host { get; set; }           // SMTP sunucusu (örn: smtp.gmail.com)
    public int Port { get; set; }              // Port numarası (örn: 587 veya 465)
    public string UserName { get; set; }       // Email adresi
    public string Password { get; set; }       // Şifre veya App Password
    public string DisplayName { get; set; }    // Gönderici adı (alıcı tarafında görünür)
}
```

### EmailSender Sınıfı

E-posta gönderme işlemlerini yönetir.

**Implementasyon:** `IEmailSender`  
**Namespace:** `Utilities.Helpers`  

#### Yöntemler

| Yöntem | Açıklama |
|--------|----------|
| `SendEmailAsync(email, subject, htmlMessage)` | E-posta gönder |

## 🔧 Kurulum

### Gereksinimler

- **.NET 8.0** veya daha üstü
- **Microsoft.AspNetCore.Identity.UI**

### NuGet Paketleri

```bash
dotnet add package Microsoft.AspNetCore.Identity.UI
```

### Proje Yapısı

```
YourProject/
├── Utilities/
│   ├── Helpers/
│   │   └── EmailSender.cs
│   └── Models/
│       └── EmailSettings.cs
├── appsettings.json
├── appsettings.Development.json
└── Program.cs
```

## ⚙️ Konfigürasyon

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

### 3. SMTP Sunucusu Seçin

#### Gmail İçin

```json
{
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UserName": "your-email@gmail.com",
    "Password": "your-16-digit-app-password",
    "DisplayName": "Your App Name"
  }
}
```

**Not:** Gmail App Password kullanmalısınız. [Google App Passwords](https://myaccount.google.com/apppasswords) adresinden oluşturun.

#### Outlook/Hotmail İçin

```json
{
  "EmailSettings": {
    "Host": "smtp-mail.outlook.com",
    "Port": 587,
    "UserName": "your-email@outlook.com",
    "Password": "your-password",
    "DisplayName": "Your App Name"
  }
}
```

#### SendGrid İçin

```json
{
  "EmailSettings": {
    "Host": "smtp.sendgrid.net",
    "Port": 587,
    "UserName": "apikey",
    "Password": "SG.your-api-key",
    "DisplayName": "Your App Name"
  }
}
```

#### Özel SMTP Sunucusu İçin

```json
{
  "EmailSettings": {
    "Host": "mail.example.com",
    "Port": 587,
    "UserName": "your-email@example.com",
    "Password": "your-password",
    "DisplayName": "Your Company Name"
  }
}
```

## 💻 Kullanım

### Temel Kullanım

```csharp
using Microsoft.AspNetCore.Identity.UI.Services;

public class UserService
{
    private readonly IEmailSender _emailSender;

    public UserService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task SendWelcomeEmailAsync(string email, string userName)
    {
        string subject = "Hoş Geldiniz!";
        string htmlMessage = $@"
            <h1>Merhaba {userName},</h1>
            <p>Platformumuza kaydolduğunuz için teşekkürler.</p>
            <p><a href='https://example.com'>Siteyi Ziyaret Edin</a></p>
        ";
        
        await _emailSender.SendEmailAsync(email, subject, htmlMessage);
    }
}
```

### Controller'da Kullanım

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
        try
        {
            await _emailSender.SendEmailAsync(email, subject, message);
            return Ok(new { message = "E-posta başarıyla gönderildi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

## 📖 Örnekler

### Örnek 1: Hoş Geldiniz E-postası

```csharp
public async Task SendWelcomeEmailAsync(string email, string fullName)
{
    var subject = "Hoş Geldiniz!";
    var htmlMessage = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
                .content {{ padding: 20px; background-color: #f8f9fa; }}
                .footer {{ text-align: center; margin-top: 20px; color: #666; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>Hoş Geldiniz!</h1>
                </div>
                <div class='content'>
                    <p>Merhaba <strong>{fullName}</strong>,</p>
                    <p>Platformumuza kaydolduğunuz için çok teşekkürler. Hesabınız hazır ve kullanıma açık.</p>
                    <p>
                        <a href='https://example.com/dashboard' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; display: inline-block;'>
                            Kontrol Paneline Git
                        </a>
                    </p>
                </div>
                <div class='footer'>
                    <p>&copy; 2024 Örnek Şirket. Tüm hakları saklıdır.</p>
                </div>
            </div>
        </body>
        </html>
    ";

    await _emailSender.SendEmailAsync(email, subject, htmlMessage);
}
```

### Örnek 2: Şifre Sıfırlama E-postası

```csharp
public async Task SendPasswordResetEmailAsync(string email, string resetLink)
{
    var subject = "Şifre Sıfırlama Talebi";
    var htmlMessage = $@"
        <h2>Şifre Sıfırlama</h2>
        <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:</p>
        <p>
            <a href='{resetLink}' style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; display: inline-block;'>
                Şifremi Sıfırla
            </a>
        </p>
        <p><strong>Uyarı:</strong> Bu bağlantı 24 saat geçerlidir.</p>
        <p>Eğer bu talebi siz yapmadıysanız, bu e-postayı görmezden gelebilirsiniz.</p>
    ";

    await _emailSender.SendEmailAsync(email, subject, htmlMessage);
}
```

### Örnek 3: Sipariş Onayı E-postası

```csharp
public async Task SendOrderConfirmationEmailAsync(string email, Order order)
{
    var subject = $"Sipariş Onayı - #{order.Id}";
    
    var itemsHtml = string.Join("", order.Items.Select(item => $@"
        <tr>
            <td>{item.ProductName}</td>
            <td>{item.Quantity}</td>
            <td>{item.Price:C}</td>
            <td>{(item.Price * item.Quantity):C}</td>
        </tr>
    "));

    var htmlMessage = $@"
        <h2>Siparişiniz Onaylandı</h2>
        <p><strong>Sipariş Numarası:</strong> {order.Id}</p>
        <p><strong>Tarih:</strong> {order.CreatedAt:dd.MM.yyyy HH:mm}</p>
        
        <h3>Sipariş Detayları</h3>
        <table border='1' cellpadding='10' style='width: 100%;'>
            <thead>
                <tr>
                    <th>Ürün</th>
                    <th>Adet</th>
                    <th>Birim Fiyat</th>
                    <th>Toplam</th>
                </tr>
            </thead>
            <tbody>
                {itemsHtml}
            </tbody>
        </table>
        
        <p><strong>Genel Toplam:</strong> {order.TotalPrice:C}</p>
        <p><strong>Kargo Durumu:</strong> Yakında gönderilecek</p>
    ";

    await _emailSender.SendEmailAsync(email, subject, htmlMessage);
}
```

### Örnek 4: Toplu E-posta Gönderme

```csharp
public async Task SendNewsletterAsync(List<string> emails, string newsContent)
{
    var subject = "Haberler - Haftalık Özet";
    var htmlMessage = $@"
        <h2>Bu Haftanın Haberleri</h2>
        {newsContent}
        <p>
            <a href='https://example.com/unsubscribe' style='color: #666; font-size: 12px;'>
                Abonelikten Çık
            </a>
        </p>
    ";

    var tasks = emails.Select(email => 
        _emailSender.SendEmailAsync(email, subject, htmlMessage)
    );

    await Task.WhenAll(tasks);
}
```

### Örnek 5: E-posta Doğrulama

```csharp
public async Task SendEmailVerificationAsync(string email, string verificationLink)
{
    var subject = "E-posta Adresinizi Doğrulayın";
    var htmlMessage = $@"
        <h2>E-posta Doğrulaması</h2>
        <p>E-posta adresinizi doğrulamak için aşağıdaki bağlantıya tıklayın:</p>
        <p>
            <a href='{verificationLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; display: inline-block;'>
                E-postamı Doğrula
            </a>
        </p>
        <p>veya bu bağlantıyı tarayıcınıza kopyalayıp yapıştırın:</p>
        <p>{verificationLink}</p>
    ";

    await _emailSender.SendEmailAsync(email, subject, htmlMessage);
}
```

## ⚠️ Hata Yönetimi

### Try-Catch ile Hata Yakalama

```csharp
public async Task<bool> SendEmailSafeAsync(string email, string subject, string htmlMessage)
{
    try
    {
        await _emailSender.SendEmailAsync(email, subject, htmlMessage);
        return true;
    }
    catch (SmtpException ex)
    {
        // SMTP sunucusu hatası
        Console.WriteLine($"SMTP Hatası: {ex.Message}");
        return false;
    }
    catch (ArgumentNullException ex)
    {
        // Boş değer hatası
        Console.WriteLine($"Geçersiz Parametre: {ex.Message}");
        return false;
    }
    catch (Exception ex)
    {
        // Diğer hatalar
        Console.WriteLine($"Bilinmeyen Hata: {ex.Message}");
        return false;
    }
}
```

### Hata Türleri

| Hata | Açıklama | Çözüm |
|------|----------|-------|
| `SmtpException` | SMTP sunucusu hatası | Host, port ve kimlik bilgilerini kontrol edin |
| `ArgumentNullException` | Boş parametre | Email, subject veya message boş olmadığını kontrol edin |
| `InvalidOperationException` | Geçersiz işlem | SMTP bağlantısını kontrol edin |
| `TimeoutException` | Timeout hatası | Port numarasını ve SSL ayarlarını kontrol edin |

## 🔒 Güvenlik

### Best Practices

#### 1. Şifrelemelerin Korunması

**❌ Yapmaması Gerekenler:**
```csharp
// Asla appsettings.json'a doğrudan şifre yazmayın
"Password": "my-secret-password"
```

**✅ Yapması Gerekenler:**
```csharp
// User Secrets kullanın (Development)
dotnet user-secrets init
dotnet user-secrets set "EmailSettings:Password" "your-password"

// Azure Key Vault kullanın (Production)
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential()
);
```

#### 2. Ortama Göre Konfigürasyon

```csharp
// appsettings.Development.json
{
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UserName": "dev-email@gmail.com",
    "Password": "dev-app-password",
    "DisplayName": "Dev App"
  }
}

// appsettings.Production.json
{
  "EmailSettings": {
    "Host": "smtp.sendgrid.net",
    "Port": 587,
    "UserName": "apikey",
    "Password": "${SENDGRID_API_KEY}",  // Environment variable
    "DisplayName": "Production App"
  }
}
```

#### 3. Rate Limiting

```csharp
public class RateLimitedEmailSender
{
    private readonly IEmailSender _emailSender;
    private readonly Dictionary<string, DateTime> _emailHistory = new();
    private readonly int _maxEmailsPerHour = 5;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        if (_emailHistory.ContainsKey(email))
        {
            var lastEmailTime = _emailHistory[email];
            var timeSinceLastEmail = DateTime.UtcNow - lastEmailTime;

            if (timeSinceLastEmail.TotalHours < 1)
            {
                throw new InvalidOperationException(
                    "Bu e-posta adresine 1 saat içerisinde çok fazla e-posta gönderildi."
                );
            }
        }

        await _emailSender.SendEmailAsync(email, subject, htmlMessage);
        _emailHistory[email] = DateTime.UtcNow;
    }
}
```

#### 4. E-posta Doğrulama

```csharp
public static bool IsValidEmail(string email)
{
    try
    {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
    }
    catch
    {
        return false;
    }
}

// Kullanım
if (!IsValidEmail(email))
{
    throw new ArgumentException("Geçersiz e-posta adresi");
}
```

## 🎓 Best Practices

### ✅ Yapılması Gerekenler

- **Asenkron operasyonları beklemeyin** - `await` kullanın
- **HTML template'leri oluşturun** - Tekrar eden kod yazmayın
- **E-posta adreslerini doğrulayın** - Gönderme öncesi check yapın
- **Hata yönetimi yapın** - Try-catch blokları kullanın
- **Şifreleri güvende tutun** - User Secrets veya Key Vault kullanın
- **Loglama yapın** - E-posta gönderme başarısını kaydedin

```csharp
public async Task SendEmailWithLoggingAsync(
    string email, 
    string subject, 
    string htmlMessage,
    ILogger<EmailSender> logger)
{
    try
    {
        if (!IsValidEmail(email))
        {
            throw new ArgumentException("Geçersiz e-posta adresi");
        }

        await _emailSender.SendEmailAsync(email, subject, htmlMessage);
        logger.LogInformation($"E-posta başarıyla gönderildi: {email}");
    }
    catch (Exception ex)
    {
        logger.LogError($"E-posta gönderimi başarısız: {email} - {ex.Message}");
        throw;
    }
}
```

### ❌ Yapılmaması Gerekenler

- **Synchronous çağrılar yapmayın** - `.Result` veya `.Wait()` kullanmayın
- **Plain text şifreler yazmayın** - Hiçbir zaman hardcode etmeyin
- **Validation'ı atlayın** - Her zaman e-posta doğrulayın
- **Aynı template'i tekrarlayın** - Helper method'lar oluşturun
- **Exception'ları yok sayın** - Her zaman handle edin
- **Synchronous operasyonlar** - UI thread'ini block etmeyin

```csharp
// ❌ Yanlış
var result = _emailSender.SendEmailAsync(email, subject, message).Result;

// ✅ Doğru
await _emailSender.SendEmailAsync(email, subject, message);
```

## 📞 Gmail App Password Oluşturma

1. [Google Hesabınıza](https://myaccount.google.com/) giriş yapın
2. **Güvenlik** sekmesine gidin
3. **2 Aşamalı Doğrulama**'yı etkinleştirin
4. **Uygulama şifreleri**'ne gidin
5. **E-posta** ve **Windows Bilgisayarı** seçin
6. 16 karakterlik şifreyi kopyalayın
7. appsettings.json'daki `Password` alanına yapıştırın

## 🔧 Sorun Giderme

### "Authentication failed" Hatası
```
Çözüm: Şifre veya e-posta adresini kontrol edin
```

### "The SMTP server requires a secure connection"
```
Çözüm: EnableSsl = true olduğundan emin olun (kodda zaten var)
```

### "Timeout occurred"
```
Çözüm: Port numarasını kontrol edin (Gmail: 587 veya 465)
```

### "5.7.1 Unauthorized"
```
Çözüm: Gmail ise App Password kullanın, normal şifreyi değil
```

## 📝 Lisans

Bu proje MIT Lisansı altında dağıtılmaktadır.

---

**Hazırlandı:** .NET 8.0  
**Interface:** IEmailSender  
**Protokol:** SMTP  
**Şifreleme:** SSL/TLS
