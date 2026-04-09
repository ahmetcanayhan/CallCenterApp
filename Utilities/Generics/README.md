# Generic Repository Pattern - .NET 8.0

Bu proje, Entity Framework Core ile kullanılan **Generic Repository Pattern** implementasyonudur. Veri erişim katmanını soyutlayarak, uygulamada kullanılacak olan tüm entity'ler için tekrar kullanılabilir bir yapı sağlar.

## 📋 İçindekiler

- [Genel Bakış](#genel-bakış)
- [Yapı](#yapı)
- [Kurulum](#kurulum)
- [Kullanım](#kullanım)
- [API Referansı](#api-referansı)
- [Örnekler](#örnekler)
- [Best Practices](#best-practices)

## 🎯 Genel Bakış

### Repository Pattern Nedir?

Repository Pattern, veri erişim işlemlerini bir soyutlama katmanı aracılığıyla yönetir. Bu sayede:

- **Veri erişim mantığı merkezi hale getirilir**
- **Veritabanı bağımlılığı azaltılır**
- **Birim testleri kolaylaştırılır**
- **Kod yeniden kullanılabilirliği artar**

### Avantajları

✅ **DRY (Don't Repeat Yourself)** - Tekrar eden kod yazılmaz  
✅ **Bakım Kolaylığı** - Değişiklikleri tek noktadan yapın  
✅ **Test Edilebilirlik** - Mock repository'ler oluşturulabilir  
✅ **Soyutlama** - Entity Framework Core'a bağımlılık azaltılır  
✅ **Esneklik** - Yeni entity'ler için yeni kod yazılmasına gerek yok  

## 📦 Yapı

### Proje Dosyaları

```
Utilities.Generics/
├── IRepository.cs        # Interface tanımı
└── Repository.cs         # Soyut implementasyon
```

### IRepository<T> Interface

Tüm repository işlemleri için bir sözleşme tanımlar.

**Generik Kısıtlama:** `where T : class`

#### Yöntemler

| Yöntem | Açıklama |
|--------|----------|
| `CreateAsync(T)` | Tek entity ekle |
| `CreateManyAsync(IEnumerable<T>)` | Çoklu entity ekle |
| `ReadByKeyAsync(object)` | Birincil anahtarla oku |
| `FindFirstAsync(Expression)` | Şarta göre ilk kaydı bul |
| `ReadManyAsync(Expression, includes)` | Çoklu kayıt oku, ilişkileri yükle |
| `UpdateAsync(T)` | Tek entity güncelle |
| `UpdateManyAsync(IEnumerable<T>)` | Çoklu entity güncelle |
| `UpdateManyAsync(Expression)` | Şarta göre kayıtları güncelle |
| `DeleteAsync(T)` | Tek entity sil |
| `DeleteManyAsync(IEnumerable<T>)` | Çoklu entity sil |
| `DeleteManyAsync(Expression)` | Şarta göre kayıtları sil |
| `CountAsync(Expression)` | Kayıt sayısını getir |
| `AnyAsync(Expression)` | Şarta uygun kayıt var mı kontrol et |

### Repository<T> Soyut Sınıf

`IRepository<T>` interface'ini implementasyon yapan soyut taban sınıftır.

**Önemli Özellikler:**

- `DbContext` bağımlılığı dependency injection yoluyla alınır
- `DbSet<T>` referansı tutulur
- Tüm yöntemler `virtual` olarak tanımlanmıştır (override için)
- Asenkron işlemler kullanılır

## 🔧 Kurulum

### Gereksinimler

- **.NET 8.0** veya daha üstü
- **Entity Framework Core 8.0**

### NuGet Paketleri

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### Proje Yapısı

```
YourProject/
├── Utilities/
│   └── Generics/
│       ├── IRepository.cs
│       └── Repository.cs
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Product.cs
│   │   └── Order.cs
│   └── Repositories/
│       ├── UserRepository.cs
│       ├── ProductRepository.cs
│       └── OrderRepository.cs
└── Program.cs
```

## 💻 Kullanım

### 1. DbContext Oluşturma

```csharp
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
}
```

### 2. Entity Modeli Oluşturma

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### 3. Concrete Repository Oluşturma

```csharp
public class UserRepository : Repository<User>
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    // Gerekirse özel yöntemler ekleyebilirsiniz
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await FindFirstAsync(u => u.Email == email);
    }
}
```

### 4. Dependency Injection Konfigürasyonu

```csharp
// Program.cs
var builder = WebApplicationBuilder.CreateBuilder(args);

// DbContext'i kaydet
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Repository'leri kaydet
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IRepository<Product>, ProductRepository>();
builder.Services.AddScoped<IRepository<Order>, OrderRepository>();

var app = builder.Build();
```

### 5. Service'te Kullanma

```csharp
public class UserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task CreateUserAsync(User user)
    {
        await _userRepository.CreateAsync(user);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.ReadByKeyAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
    {
        return await _userRepository.ReadManyAsync(u => u.IsActive);
    }

    public async Task UpdateUserAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _userRepository.ReadByKeyAsync(id);
        if (user != null)
        {
            await _userRepository.DeleteAsync(user);
        }
    }
}
```

## 📚 API Referansı

### Create (Oluşturma)

#### `CreateAsync(T entity)`
Veritabanına tek bir entity ekler.

```csharp
var user = new User { Name = "Ahmet", Email = "ahmet@example.com" };
await _userRepository.CreateAsync(user);
```

#### `CreateManyAsync(IEnumerable<T> entities)`
Veritabanına birden fazla entity ekler.

```csharp
var users = new List<User> 
{ 
    new User { Name = "Ahmet", Email = "ahmet@example.com" },
    new User { Name = "Fatma", Email = "fatma@example.com" }
};
await _userRepository.CreateManyAsync(users);
```

---

### Read (Okuma)

#### `ReadByKeyAsync(object entityKey)`
Birincil anahtar ile entity'yi bulur.

```csharp
var user = await _userRepository.ReadByKeyAsync(1);
```

#### `FindFirstAsync(Expression<Func<T, bool>>? expression = null)`
Şarta uygun ilk kaydı bulur. Şart null ise ilk kaydı döner.

```csharp
var user = await _userRepository.FindFirstAsync(u => u.Email == "ahmet@example.com");
```

#### `ReadManyAsync(Expression<Func<T, bool>>? expression = null, params string[] includes)`
Şarta uygun tüm kayıtları döner. İlişkili veriler yüklenebilir (eager loading).

```csharp
// Tüm aktif kullanıcıları getir
var users = await _userRepository.ReadManyAsync(u => u.IsActive);

// İlişkili Order'ları da yükle (Eager Loading)
var usersWithOrders = await _userRepository.ReadManyAsync(
    u => u.IsActive, 
    "Orders"  // navigation property adı
);
```

---

### Update (Güncelleme)

#### `UpdateAsync(T entity)`
Tek bir entity'yi günceller.

```csharp
var user = await _userRepository.ReadByKeyAsync(1);
user.Name = "Mehmet";
await _userRepository.UpdateAsync(user);
```

#### `UpdateManyAsync(IEnumerable<T> entities)`
Birden fazla entity'yi günceller.

```csharp
var users = new List<User> { user1, user2, user3 };
await _userRepository.UpdateManyAsync(users);
```

#### `UpdateManyAsync(Expression<Func<T, bool>>? expression = null)`
Şarta uygun tüm kayıtları günceller.

```csharp
// ⚠️ Not: Bu yöntem IQueryable desteklemiyor, 
// şarta uygun tüm entity'leri çeker ve günceller
var inactiveUsers = await _userRepository.ReadManyAsync(u => !u.IsActive);
// Güncelleme işlemini manuel yapmalısınız
```

---

### Delete (Silme)

#### `DeleteAsync(T entity)`
Tek bir entity'yi siler.

```csharp
var user = await _userRepository.ReadByKeyAsync(1);
await _userRepository.DeleteAsync(user);
```

#### `DeleteManyAsync(IEnumerable<T> entities)`
Birden fazla entity'yi siler.

```csharp
var users = new List<User> { user1, user2, user3 };
await _userRepository.DeleteManyAsync(users);
```

#### `DeleteManyAsync(Expression<Func<T, bool>>? expression = null)`
Şarta uygun tüm kayıtları siler.

```csharp
// Tüm inaktif kullanıcıları sil
await _userRepository.DeleteManyAsync(u => !u.IsActive);
```

---

### Count & Any (Kontrol)

#### `CountAsync(Expression<Func<T, bool>>? expression = null)`
Şarta uygun kayıt sayısını döner.

```csharp
var activeUserCount = await _userRepository.CountAsync(u => u.IsActive);
var totalCount = await _userRepository.CountAsync();
```

#### `AnyAsync(Expression<Func<T, bool>>? expression = null)`
Şarta uygun herhangi bir kayıt olup olmadığını kontrol eder.

```csharp
var hasActiveUsers = await _userRepository.AnyAsync(u => u.IsActive);
var hasAnyUser = await _userRepository.AnyAsync();
```

## 📖 Örnekler

### Örnek 1: Kullanıcı Ekleme

```csharp
var newUser = new User 
{ 
    Name = "Zeynep", 
    Email = "zeynep@example.com",
    CreatedAt = DateTime.UtcNow
};

await _userRepository.CreateAsync(newUser);
```

### Örnek 2: Koşullu Sorgulama

```csharp
// Email'i "example.com" ile biten kullanıcıları getir
var exampleUsers = await _userRepository.ReadManyAsync(
    u => u.Email.EndsWith("@example.com")
);
```

### Örnek 3: Sayfalamalı Veri Çekme

```csharp
public async Task<IEnumerable<User>> GetUsersPaginatedAsync(int page, int pageSize)
{
    var allUsers = await _userRepository.ReadManyAsync();
    return allUsers
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();
}
```

### Örnek 4: Toplu İşlemler

```csharp
// 100 adet yeni kullanıcı ekle
var newUsers = Enumerable.Range(1, 100)
    .Select(i => new User { Name = $"User{i}", Email = $"user{i}@example.com" })
    .ToList();

await _userRepository.CreateManyAsync(newUsers);
```

### Örnek 5: Koşullu Silme

```csharp
// 90 günden eski hiç login yapmayan kullanıcıları sil
var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);
await _userRepository.DeleteManyAsync(
    u => u.LastLoginAt == null && u.CreatedAt < ninetyDaysAgo
);
```

## ⚠️ Önemli Notlar

### 1. SaveChanges Yok

Bu implementasyonda `SaveChangesAsync()` çağrılmaz. Unit of Work Pattern ile kullanılması önerilir:

```csharp
public class UnitOfWork : IDisposable
{
    private readonly ApplicationDbContext _context;

    public IRepository<User> Users { get; }
    public IRepository<Product> Products { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Products = new ProductRepository(context);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
```

### 2. Eager Loading

`ReadManyAsync` yönteminin `includes` parametresi ilişkili verileri yüklemek için kullanılır:

```csharp
// Navigation property adını string olarak geçin
var users = await _userRepository.ReadManyAsync(
    null, 
    "Orders", "Orders.Items"  // nested ilişkiler
);
```

### 3. Performance Dikkatler

- **Büyük veri setleri için pagination kullanın**
- **Gereksiz `Include` çağrılarından kaçının**
- **Projection kullanarak sadece ihtiyacınız olan alanları çekin**

```csharp
var userEmails = await _userRepository.ReadManyAsync()
    .Select(u => new { u.Id, u.Email })
    .ToListAsync();
```

### 4. DbContext Lifecycle

Repository sınıfları `DbContext` bağımlılığı alması nedeniyle, **Dependency Injection container'a `Scoped` olarak kayıt edilmelidir**:

```csharp
builder.Services.AddScoped<IRepository<User>, UserRepository>();
```

## 🎓 Best Practices

### ✅ Yapılması Gerekenler

- **Concrete repository'ler oluşturun** - Entity başına specialized repository yazın
- **Interface'i inject edin** - Abstract'a bağımlılık oluşturun
- **Unit of Work kullanın** - Bir işlemde birden fazla repository var ise
- **Async/await kullanın** - Tüm yöntemler async'tir
- **Validation yapın** - Repository'e gelen verileri doğrulayın
- **Exception handling yazın** - Veritabanı hatalarını yönetin

```csharp
try
{
    await _userRepository.CreateAsync(user);
    await _unitOfWork.SaveAsync();
}
catch (DbUpdateException ex)
{
    // Veritabanı hatası yönetimi
    throw;
}
```

### ❌ Yapılmaması Gerekenler

- **Sync yöntemleri çağırmayın** - `.Result` veya `.Wait()` kullanmayın
- **Kalın repository yöntemleri yazmayın** - Kompleks iş mantığı service'e taşıyın
- **DbContext'i expose etmeyiz** - Entity Framework Core'a bağımlılık oluşur
- **Direct SQL kullanmayın** - SQL Injection riskidir
- **Çok fazla Include kullanmayın** - N+1 problem oluşur

## 📝 Lisans

Bu proje MIT Lisansı altında dağıtılmaktadır.

---

**Hazırlandı:** .NET 8.0  
**Pattern:** Generic Repository + Unit of Work  
**ORM:** Entity Framework Core
