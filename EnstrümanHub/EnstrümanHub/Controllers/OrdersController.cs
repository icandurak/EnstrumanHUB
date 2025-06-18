// Konum: Controllers/OrdersController.cs

using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EnstrümanHub.Models; // Model namespace'i
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth.OAuth2;
using System.Text.Json;

namespace EnstrümanHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "Admin")] // Authorization kontrolü kaldırıldı
    public class OrdersController : ControllerBase
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrdersController> _logger;
        private const string _collectionName = "orders";

        public OrdersController(IConfiguration configuration, ILogger<OrdersController> logger)
        {
            _logger = logger;
            _configuration = configuration;

            try
            {
                // Firebase kimlik bilgilerini yükle
                var credentialsPath = _configuration["Firestore:CredentialsFile"];
                if (string.IsNullOrEmpty(credentialsPath))
                {
                    credentialsPath = "enstrumand2-firebase-adminsdk.json";
                }

                if (!System.IO.File.Exists(credentialsPath))
                {
                    throw new InvalidOperationException($"Firebase kimlik bilgileri dosyası bulunamadı: {credentialsPath}");
                }

                var credentials = GoogleCredential.FromFile(credentialsPath);
                _firestoreDb = new FirestoreDbBuilder
                {
                    ProjectId = "enstrumand2",
                    Credential = credentials
                }.Build();

                _logger.LogInformation("Firestore bağlantısı başarıyla kuruldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firestore bağlantısı kurulurken hata oluştu");
                throw;
            }
        }

        // GET: api/orders/test
        [HttpGet("test")]
        public async Task<ActionResult> TestFirestoreConnection()
        {
            try
            {
                _logger.LogInformation("Test endpoint'i çağrıldı");
                
                // Koleksiyon listesini al
                var collections = await _firestoreDb.ListRootCollectionsAsync().ToListAsync();
                var collectionNames = collections.Select(c => c.Id).ToList();
                
                _logger.LogInformation("Mevcut koleksiyonlar: {Collections}", 
                    JsonSerializer.Serialize(collectionNames));
                
                // Orders koleksiyonunun varlığını kontrol et
                var ordersCollection = _firestoreDb.Collection(_collectionName);
                var snapshot = await ordersCollection.Limit(1).GetSnapshotAsync();
                
                var result = new
                {
                    IsConnected = true,
                    ProjectId = _configuration["Firestore:ProjectId"],
                    CredentialsPath = _configuration["Firestore:CredentialsFile"],
                    AvailableCollections = collectionNames,
                    OrdersCollectionExists = collections.Any(c => c.Id == _collectionName),
                    OrdersCount = snapshot.Count,
                    FirstOrder = snapshot.Documents.FirstOrDefault()?.ToDictionary()
                };
                
                _logger.LogInformation("Test sonuçları: {Results}", 
                    JsonSerializer.Serialize(result));
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test endpoint'inde hata oluştu");
                return StatusCode(500, new { 
                    Error = ex.Message,
                    StackTrace = ex.StackTrace,
                    IsConnected = false,
                    ProjectId = _configuration["Firestore:ProjectId"],
                    CredentialsPath = _configuration["Firestore:CredentialsFile"]
                });
            }
        }

        // GET: api/orders
        // Tüm siparişleri getirir.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            try
            {
                _logger.LogInformation("GetOrders endpoint'i çağrıldı");
                
                // Dashboard'daki gibi basit bir şekilde koleksiyonu al
                var ordersCollection = _firestoreDb.Collection(_collectionName);
                var snapshot = await ordersCollection.GetSnapshotAsync();
                
                _logger.LogInformation("Firestore'dan {Count} adet sipariş dökümanı alındı", snapshot.Documents.Count);
                
                // Her dökümanı basit bir şekilde dönüştür
                var orders = snapshot.Documents.Select(doc => {
                    var data = doc.ToDictionary();
                    return new Order
                    {
                        Id = doc.Id,
                        CustomerName = data.ContainsKey("customerName") ? data["customerName"].ToString() : 
                                     data.ContainsKey("userName") ? data["userName"].ToString() : "İsimsiz",
                        Date = data.ContainsKey("date") ? data["date"].ToString() : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Total = data.ContainsKey("total") ? Convert.ToDecimal(data["total"]) : 0,
                        Status = data.ContainsKey("status") ? data["status"].ToString() : "Beklemede"
                    };
                }).ToList();
                
                _logger.LogInformation("Toplam {Count} sipariş başarıyla dönüştürüldü", orders.Count);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetOrders metodunda hata oluştu");
                return StatusCode(500, "Siparişler alınırken bir sunucu hatası oluştu.");
            }
        }

        // GET: api/orders/{id}
        // Tek bir siparişi getirir.
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                DocumentSnapshot doc = await docRef.GetSnapshotAsync();

                if (!doc.Exists)
                {
                    return NotFound($"'{id}' ID'li sipariş bulunamadı.");
                }

                Order order = doc.ConvertTo<Order>();
                order.Id = doc.Id;
                return Ok(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOrder({id}): {ex.Message}");
                return StatusCode(500, "Sipariş alınırken bir sunucu hatası oluştu.");
            }
        }

        // PATCH: api/orders/{id}
        // Mevcut bir siparişin SADECE durumunu günceller.
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] OrderStatusUpdateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.Status))
                {
                    return BadRequest("Status alanı boş olamaz.");
                }

                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                // UpdateAsync metodu, doküman yoksa hata fırlatmaz, bu yüzden önce varlığını kontrol etmek iyi bir pratik olabilir.
                await docRef.UpdateAsync("Status", request.Status);
                
                return NoContent(); // Başarılı, içerik döndürmeye gerek yok.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateOrderStatus({id}): {ex.Message}");
                return StatusCode(500, "Sipariş güncellenirken bir sunucu hatası oluştu.");
            }
        }

        // DELETE: api/orders/{id}
        // Belirtilen siparişi siler.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                await docRef.DeleteAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteOrder({id}): {ex.Message}");
                return StatusCode(500, "Sipariş silinirken bir sunucu hatası oluştu.");
            }
        }
    }

    // Sadece status güncellemesi için kullanılacak Data Transfer Object (DTO).
    public class OrderStatusUpdateRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}