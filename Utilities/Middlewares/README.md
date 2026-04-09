# Global Exception Handling Middleware - .NET 8.0

Bu proje, **ASP.NET Core** uygulamalarında oluşan tüm hataları merkezi bir noktada yönetmek için tasarlanmış bir **Global Exception Handling Middleware** implementasyonudur.

## 📋 İçindekiler

- [Genel Bakış](#genel-bakış)
- [Yapı](#yapı)
- [Kurulum](#kurulum)
- [Konfigürasyon](#konfigürasyon)
- [Kullanım](#kullanım)
- [Hata Türleri](#hata-türleri)
- [Örnekler](#örnekler)
- [Loglama](#loglama)
- [Best Practices](#best-practices)
- [Sorun Giderme](#sorun-giderme)

## 🎯 Genel Bakış

### Global Exception Handling Nedir?

Middleware, HTTP istek pipeline'ında tüm exception'ları yakalayıp tutarlı bir formatta yanıt verir. Bu sayede:

- **Merkezi hata yönetimi** - Tüm hataları bir yerde yönetin
- **Tutarlı hata yanıtları** - Her endpoint aynı formatta hata döner
- **Logging** - Tüm hatalar otomatik olarak kaydedilir
- **Güvenlik** - Internal hata detayları kullanıcıya gösterilmez
- **Kod tekrarı azaltma** - Try-catch blokları yazılmaz

### Özellikleri

✅ **Otomatik Exception Yakalama** - Tüm unhandled exception'ları yakalar  
✅ **HTTP Status Code Eşlemesi** - Exception türüne göre uygun status code  
✅ **JSON Yanıt Formatı** - Yapılandırılmış hata yanıtları  
✅ **Logging Entegrasyonu** - ILogger ile otomatik loglama  
✅ **Dependency Injection** - IoC container ile kolay entegrasyon  
✅ **Genişletilebilir** - Custom exception'lar eklenebilir  
✅ **Production-Ready** - Güvenli hata mesajları  

## 📦 Yapı

### Proje Dosyaları

```
Utilities/
├── Middlewares/
│   └── GlobalExceptionHandlingMiddleware.cs    # Middleware implementasyonu
└── Models/
    └── ErrorDetails.cs                         # Hata detayları modeli
```

### ErrorDetails Sınıfı

Hata bilgilerini JSON formatında sunmak için kullanılır.

```csharp
public class ErrorDetails
{
    public int StatusCode { get; set; }        // HTTP Status Code (400, 404, 500, vb.)
    public string? Message { get; set; }       // Hata mesajı
    
    public override string ToString()
    {
        // JSON formatında string döndürür
        return JsonSerializer.Serialize(this);
    }
}
```

**Örnek JSON Çıktı:**
```json
{
  "statusCode": 400,
  "message": "Girdi null olamaz"
}
```

### GlobalExceptionHandlingMiddleware Sınıfı

Tüm HTTP isteklerini try-catch ile sarıp exception'ları yakalar.

**Namespace:** `Utilities.Middlewares`  
**Bağımlılıklar:** `RequestDelegate`, `ILogger`  

#### İş Akışı

```
1. Request gelir
   ↓
2. Middleware try bloğuna girer
   ↓
3. next(context) çağrılır (sonraki middleware/endpoint)
   ↓
4a. Exception oluşursa:
   - Logger ile log tutulur
   - HandleExceptionAsync çağrılır
   - Uygun status code ayarlanır
   - JSON yanıt döndürülür
   ↓
4b. Exception yoksa:
   - Normal yanıt döndürülür
```

#### Exception → Status Code Eşlemesi

| Exception Türü | HTTP Status Code | Açıklama |
|---|---|---|
| `ArgumentNullException` | 400 Bad Request | Geçersiz/boş parametre |
| `KeyNotFoundException` | 404 Not Found | Kayıt bulunamadı |
| `UnauthorizedAccessException` | 401 Unauthorized | Yetki yetersiz |
| Diğer tüm Exception'lar | 500 Internal Server Error | Sunucu hatası |

## 🔧 Kurulum

### Gereksinimler

- **.NET 8.0** veya daha üstü
- **Microsoft.AspNetCore.Http**

### NuGet Paketleri

```bash
dotnet add package Microsoft.Extensions.Logging
```

### Proje Yapısı

```
YourProject/
├── Utilities/
│   ├── Middlewares/
│   │   └── GlobalExceptionHandlingMiddleware.cs
│   └── Models/
│       └── ErrorDetails.cs
└── Program.cs
```

## ⚙️ Konfigürasyon

### 1. Program.cs'te Middleware'ı Kaydedin

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// Logging'i yapılandırın
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

// Diğer servisler...
builder.Services.AddControllers();

var app = builder.Build();

// ⭐ Middleware'ı pipeline'ın başına ekleyin
// NOT: Pipeline'ın en başında olması önemlidir!
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Diğer middleware'ler
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 2. appsettings.json'da Loglama Ayarları

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Utilities.Middlewares": "Error"
    }
  }
}
```

### 3. Using İfadeleri Ekleyin

```csharp
using Utilities.Middlewares;
using Utilities.Models;
using Microsoft.Extensions.Logging;
```

## 💻 Kullanım

### Temel Kullanım

Middleware eklendikten sonra herhangi bir controller action'ında exception oluştursanız otomatik olarak yakalanır:

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        // Eğer id 0 ise ArgumentNullException olur
        if (id <= 0)
            throw new ArgumentNullException(nameof(id));

        // Eğer kullanıcı bulunamazsa KeyNotFoundException olur
        var user = GetUserFromDatabase(id);
        if (user == null)
            throw new KeyNotFoundException($"ID {id} olan kullanıcı bulunamadı");

        return Ok(user);
    }
}
```

**Response (400 Bad Request):**
```json
{
  "statusCode": 400,
  "message": "id"
}
```

**Response (404 Not Found):**
```json
{
  "statusCode": 404,
  "message": "ID 999 olan kullanıcı bulunamadı"
}
```

### Service'te Kullanım

```csharp
public class UserService
{
    public User GetUserById(int id)
    {
        if (id <= 0)
            throw new ArgumentNullException(nameof(id), "Kullanıcı ID'si boş olamaz");

        var user = _repository.GetById(id);
        
        if (user == null)
            throw new KeyNotFoundException($"Kullanıcı bulunamadı: {id}");

        return user;
    }
}
```

## 📖 Hata Türleri

### 1. ArgumentNullException (400 Bad Request)

**Nedir?** Geçersiz veya boş parametre geçildiğinde  
**Kullanım:**
```csharp
public void CreateUser(User user)
{
    if (user == null)
        throw new ArgumentNullException(nameof(user), "Kullanıcı boş olamaz");
}
```

**Response:**
```json
{
  "statusCode": 400,
  "message": "Kullanıcı boş olamaz (Parameter name: 'user')"
}
```

### 2. KeyNotFoundException (404 Not Found)

**Nedir?** İstenilen kayıt veritabanında bulunamadığında  
**Kullanım:**
```csharp
public User GetUserById(int id)
{
    var user = _db.Users.FirstOrDefault(u => u.Id == id);
    
    if (user == null)
        throw new KeyNotFoundException($"ID {id} olan kullanıcı bulunamadı");
    
    return user;
}
```

**Response:**
```json
{
  "statusCode": 404,
  "message": "ID 999 olan kullanıcı bulunamadı"
}
```

### 3. UnauthorizedAccessException (401 Unauthorized)

**Nedir?** Kullanıcının yetki olmadığında  
**Kullanım:**
```csharp
public void DeleteUser(int userId, int currentUserId)
{
    if (userId != currentUserId)
        throw new UnauthorizedAccessException("Bu işlem için yetkiniz yok");
}
```

**Response:**
```json
{
  "statusCode": 401,
  "message": "Bu işlem için yetkiniz yok"
}
```

### 4. InvalidOperationException (500 Internal Server Error)

**Nedir?** Geçersiz bir işlem yapılmaya çalışıldığında  
**Kullanım:**
```csharp
public void ApproveOrder(Order order)
{
    if (order.Status != OrderStatus.Pending)
        throw new InvalidOperationException("Sadece Pending siparişler onaylanabilir");
}
```

**Response:**
```json
{
  "statusCode": 500,
  "message": "Sadece Pending siparişler onaylanabilir"
}
```

## 📖 Örnekler

### Örnek 1: Kullanıcı Alma

```csharp
[HttpGet("{id}")]
public IActionResult GetUser(int id)
{
    if (id <= 0)
        throw new ArgumentNullException(nameof(id), "Kullanıcı ID'si pozitif olmalı");

    var user = _userService.GetUserById(id);
    
    return Ok(user);
}
```

**Test:**
- `GET /api/users/0` → 400 Bad Request
- `GET /api/users/999` → 404 Not Found
- `GET /api/users/1` → 200 OK

### Örnek 2: Kullanıcı Oluşturma

```csharp
[HttpPost]
public IActionResult CreateUser([FromBody] CreateUserDto dto)
{
    if (dto == null)
        throw new ArgumentNullException(nameof(dto), "Kullanıcı bilgileri boş olamaz");

    if (string.IsNullOrWhiteSpace(dto.Email))
        throw new ArgumentNullException(nameof(dto.Email), "E-posta adresi zorunludur");

    var user = _userService.Create(dto);
    
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
}
```

### Örnek 3: Kullanıcı Güncelleme

```csharp
[HttpPut("{id}")]
public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto dto)
{
    if (id <= 0)
        throw new ArgumentNullException(nameof(id), "Geçersiz ID");

    if (dto == null)
        throw new ArgumentNullException(nameof(dto), "Güncelleme bilgileri boş olamaz");

    var user = _userService.Update(id, dto);
    
    return Ok(user);
}
```

### Örnek 4: Kullanıcı Silme (Yetkilendirme)

```csharp
[HttpDelete("{id}")]
[Authorize]
public IActionResult DeleteUser(int id)
{
    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    if (currentUserId == 0)
        throw new UnauthorizedAccessException("Kimlik doğrulama başarısız");

    if (id != currentUserId && !User.IsInRole("Admin"))
        throw new UnauthorizedAccessException("Sadece kendi hesabınızı silebilirsiniz");

    _userService.Delete(id);
    
    return NoContent();
}
```

### Örnek 5: Sipariş Onaylama

```csharp
[HttpPost("{orderId}/approve")]
public IActionResult ApproveOrder(int orderId)
{
    if (orderId <= 0)
        throw new ArgumentNullException(nameof(orderId), "Sipariş ID'si geçersiz");

    var order = _orderService.GetById(orderId);
    
    if (order == null)
        throw new KeyNotFoundException($"Sipariş bulunamadı: {orderId}");

    if (order.Status != OrderStatus.Pending)
        throw new InvalidOperationException("Sadece Pending siparişler onaylanabilir");

    _orderService.Approve(order);
    
    return Ok(new { message = "Sipariş onaylandı" });
}
```

## 📝 Loglama

### Otomatik Loglama

Middleware otomatik olarak tüm exception'ları kaydeder:

```csharp
logger.LogError(ex, "Sistemde global bir hata yakalandı!");
```

**Log Çıktısı:**
```
fail: Utilities.Middlewares.GlobalExceptionHandlingMiddleware[0]
      Sistemde global bir hata yakalandı!
System.ArgumentNullException: id
   at YourProject.Controllers.UsersController.GetUser(Int32 id) in UsersController.cs:line 25
```

### Custom Loglama Yapılandırması

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Utilities.Middlewares.GlobalExceptionHandlingMiddleware": "Error"
    },
    "Console": {
      "IncludeScopes": true,
      "TimestampFormat": "yyyy-MM-dd HH:mm:ss"
    }
  }
}
```

### File'a Loglama (Serilog ile)

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
```

```csharp
builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .WriteTo.File("logs/exceptions-.txt", rollingInterval: RollingInterval.Day)
);
```

## 🎓 Best Practices

### ✅ Yapılması Gerekenler

1. **Middleware'ı pipeline'ın başına ekleyin**
```csharp
// ✅ Doğru - En başında
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
```

2. **Anlamlı hata mesajları yazın**
```csharp
// ✅ Doğru
throw new ArgumentNullException(nameof(email), "E-posta adresi zorunludur");

// ❌ Yanlış
throw new ArgumentNullException(nameof(email));
```

3. **Uygun exception türlerini kullanın**
```csharp
// ✅ Doğru
if (user == null)
    throw new KeyNotFoundException("Kullanıcı bulunamadı");

// ❌ Yanlış
if (user == null)
    throw new Exception("Kullanıcı bulunamadı");
```

4. **Güvenli hata mesajları verin**
```csharp
// ✅ Doğru - Generic mesaj
throw new InvalidOperationException("İşlem başarısız");

// ❌ Yanlış - Internal detaylar sızdırma
throw new InvalidOperationException(
    $"Database bağlantısı başarısız: {ex.InnerException.Message}"
);
```

5. **Logging yapın**
```csharp
// ✅ Doğru
_logger.LogError(ex, "Sipariş işlenirken hata oluştu. Sipariş ID: {OrderId}", orderId);

// ❌ Yanlış
// Loglama yapılmaz
throw new InvalidOperationException("Hata");
```

### ❌ Yapılmaması Gerekenler

1. **Middleware'ı yanlış yere eklemeyin**
```csharp
// ❌ Yanlış - Autorization'dan sonra
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
```

2. **Exception'ları alt alta throw etmeyin**
```csharp
// ❌ Yanlış
try
{
    // kod
}
catch (Exception ex)
{
    throw ex;  // Stack trace kaybedilir
}
```

3. **Çok generic hata mesajları kullanmayın**
```csharp
// ❌ Yanlış
throw new Exception("Hata");

// ✅ Doğru
throw new InvalidOperationException("Sipariş durumu güncellenemedi");
```

4. **Internal exception'ları expose etmeyin**
```csharp
// ❌ Yanlış
throw new Exception($"SQL Error: {sqlException.Message}");

// ✅ Doğru
_logger.LogError(sqlException, "Veritabanı hatası");
throw new InvalidOperationException("İşlem başarısız");
```

## 🔒 Güvenlik Notları

### 1. Exception Detaylarını Sızdırmayın

```csharp
// ❌ Tehlikeli
public void ProcessPayment(Payment payment)
{
    try
    {
        // Ödeme işlemi
    }
    catch (Exception ex)
    {
        // Eksik! Internal detaylar sızdırılabilir
        throw;
    }
}

// ✅ Güvenli
public void ProcessPayment(Payment payment)
{
    try
    {
        // Ödeme işlemi
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Ödeme işlemi başarısız");
        throw new InvalidOperationException("Ödeme işlemi başarısız");
    }
}
```

### 2. Sensitive Bilgileri Loglama

```csharp
// ❌ Tehlikeli - Şifre loğa yazılıyor
_logger.LogInformation($"Kullanıcı giriş: {username}, Şifre: {password}");

// ✅ Güvenli
_logger.LogInformation($"Kullanıcı giriş başarısız: {username}");
```

### 3. Dosya Yollarını Vermeyin

```csharp
// ❌ Tehlikeli - Dosya yolu sızdırılıyor
throw new Exception($"File not found: C:\\Users\\Admin\\Database.sql");

// ✅ Güvenli
_logger.LogError("Veritabanı dosyası bulunamadı");
throw new KeyNotFoundException("Kaynağa erişilemiyor");
```

## 🔧 Sorun Giderme

### Middleware Devreye Girmedi

**Sorun:** Exception yakalanmıyor  
**Çözüm:**
```csharp
// Middleware'ın pipeline'ın başında olduğundan emin olun
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseHttpsRedirection();  // Diğer middleware'ler sonra gelsin
```

### Çift Hata Yanıtı Alınıyor

**Sorun:** Hata iki kez döndürülüyor  
**Çözüm:**
```csharp
// Controller'da try-catch blokları kaldırın
// Middleware zaten hataları yakalar

// ❌ Yanlış
[HttpGet("{id}")]
public IActionResult GetUser(int id)
{
    try
    {
        var user = _service.GetUser(id);
        return Ok(user);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = ex.Message });
    }
}

// ✅ Doğru
[HttpGet("{id}")]
public IActionResult GetUser(int id)
{
    var user = _service.GetUser(id);
    return Ok(user);
}
```

### Loglama Çalışmıyor

**Sorun:** Exception'lar loğa yazılmıyor  
**Çözüm:**
```csharp
// appsettings.json'da loglama seviyesini kontrol edin
{
  "Logging": {
    "LogLevel": {
      "Utilities.Middlewares.GlobalExceptionHandlingMiddleware": "Error"  // Error seviyesinde
    }
  }
}
```

### Custom Exception'lar Yakalanmıyor

**Sorun:** Kendi exception'larının status code'u 500  
**Çözüm:** Middleware'ı genişletin
```csharp
public static class GlobalExceptionHandlingMiddlewareExtensions
{
    public static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ArgumentNullException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            CustomBusinessException => StatusCodes.Status422UnprocessableEntity,
            _=> StatusCodes.Status500InternalServerError
        };

        var errorDetails = new ErrorDetails
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message,
        };
        return context.Response.WriteAsync(errorDetails.ToString());
    }
}
```

## 🚀 İleri Seviye - Custom Exception'lar

### Custom Exception Sınıfı Oluşturma

```csharp
public class BusinessException : Exception
{
    public int? StatusCode { get; set; }

    public BusinessException(string message, int? statusCode = null)
        : base(message)
    {
        StatusCode = statusCode;
    }
}

public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; set; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Doğrulama hatası")
    {
        Errors = errors;
    }
}
```

### Middleware'ı Genişletme

```csharp
public static Task HandleExceptionAsync(HttpContext context, Exception exception)
{
    context.Response.ContentType = "application/json";
    
    var statusCode = exception switch
    {
        ArgumentNullException => StatusCodes.Status400BadRequest,
        KeyNotFoundException => StatusCodes.Status404NotFound,
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        BusinessException be => be.StatusCode ?? StatusCodes.Status400BadRequest,
        ValidationException => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status500InternalServerError
    };

    context.Response.StatusCode = statusCode;
    
    var response = exception switch
    {
        ValidationException ve => new { statusCode, errors = ve.Errors },
        _ => new { statusCode, message = exception.Message }
    };

    return context.Response.WriteAsync(JsonSerializer.Serialize(response));
}
```

## 📝 Lisans

Bu proje MIT Lisansı altında dağıtılmaktadır.

---

**Hazırlandı:** .NET 8.0  
**Pattern:** Middleware  
**Amaç:** Global Exception Handling  
**Logging:** ILogger
