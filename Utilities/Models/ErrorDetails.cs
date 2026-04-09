using System.Text.Json;

namespace Utilities.Models
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public override string ToString()
        {
            // Sınıfın kendisini JSON formatında döndürür. Bu, hata detaylarının daha okunabilir ve yapılandırılmış bir şekilde sunulmasını sağlar.
            return JsonSerializer.Serialize(this);
        }
    }
}
