using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Google.Cloud.Firestore;
using EnstrümanHub.Models;

namespace EnstrümanHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly ILogger<UsersController> _logger;

        public UsersController(FirestoreDb firestoreDb, ILogger<UsersController> logger)
        {
            _firestoreDb = firestoreDb;
            _logger = logger;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Tüm kullanıcılar getiriliyor...");
                
                var users = new List<User>();
                var snapshot = await _firestoreDb.Collection("users").GetSnapshotAsync();

                foreach (var doc in snapshot)
                {
                    var data = doc.ToDictionary();
                    var user = new User
                    {
                        Id = doc.Id,
                        Name = data["name"].ToString() ?? string.Empty,
                        Email = data["email"].ToString() ?? string.Empty
                    };
                    users.Add(user);
                }

                _logger.LogInformation("Toplam {Count} kullanıcı bulundu", users.Count);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcılar getirilirken hata oluştu");
                return StatusCode(500, "Kullanıcılar getirilirken bir hata oluştu");
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            try
            {
                _logger.LogInformation("Kullanıcı getiriliyor. ID: {Id}", id);
                
                var docRef = _firestoreDb.Collection("users").Document(id);
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Kullanıcı bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan kullanıcı bulunamadı");
                }

                var data = snapshot.ToDictionary();
                var user = new User
                {
                    Id = snapshot.Id,
                    Name = data["name"].ToString() ?? string.Empty,
                    Email = data["email"].ToString() ?? string.Empty
                };

                _logger.LogInformation("Kullanıcı başarıyla getirildi. ID: {Id}", id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı getirilirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Kullanıcı getirilirken bir hata oluştu");
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            try
            {
                _logger.LogInformation("Yeni kullanıcı ekleniyor...");

                var userData = new Dictionary<string, object>
                {
                    { "name", user.Name },
                    { "email", user.Email }
                };

                var docRef = await _firestoreDb.Collection("users").AddAsync(userData);
                user.Id = docRef.Id;

                _logger.LogInformation("Yeni kullanıcı başarıyla eklendi. ID: {Id}", user.Id);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı eklenirken hata oluştu");
                return StatusCode(500, "Kullanıcı eklenirken bir hata oluştu");
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, User user)
        {
            try
            {
                _logger.LogInformation("Kullanıcı güncelleniyor. ID: {Id}", id);

                if (id != user.Id)
                {
                    _logger.LogWarning("Kullanıcı ID'si eşleşmiyor. Path ID: {PathId}, Body ID: {BodyId}", id, user.Id);
                    return BadRequest("Kullanıcı ID'si eşleşmiyor");
                }

                var docRef = _firestoreDb.Collection("users").Document(id);
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Kullanıcı bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan kullanıcı bulunamadı");
                }

                var userData = new Dictionary<string, object>
                {
                    { "name", user.Name },
                    { "email", user.Email }
                };

                await docRef.SetAsync(userData, SetOptions.MergeAll);
                _logger.LogInformation("Kullanıcı başarıyla güncellendi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı güncellenirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Kullanıcı güncellenirken bir hata oluştu");
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                _logger.LogInformation("Kullanıcı siliniyor. ID: {Id}", id);

                var docRef = _firestoreDb.Collection("users").Document(id);
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Kullanıcı bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan kullanıcı bulunamadı");
                }

                await docRef.DeleteAsync();
                _logger.LogInformation("Kullanıcı başarıyla silindi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı silinirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Kullanıcı silinirken bir hata oluştu");
            }
        }

        // GET: api/users/{id}/orders
        [HttpGet("{id}/orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(string id)
        {
            try
            {
                _logger.LogInformation("Kullanıcının siparişleri getiriliyor. User ID: {Id}", id);

                var orders = new List<Order>();
                var query = _firestoreDb.Collection("orders").WhereEqualTo("userId", id);
                var snapshot = await query.GetSnapshotAsync();

                foreach (var doc in snapshot)
                {
                    var data = doc.ToDictionary();
                    var order = new Order
                    {
                        Id = doc.Id,
                        UserId = data["userId"].ToString() ?? string.Empty,
                        CustomerName = data["customerName"].ToString() ?? string.Empty,
                        ShippingAddress = data["shippingAddress"].ToString() ?? string.Empty,
                        BillingAddress = data["billingAddress"].ToString() ?? string.Empty,
                        PhoneNumber = data["phoneNumber"].ToString() ?? string.Empty,
                        Email = data["email"].ToString() ?? string.Empty,
                        Date = data["date"].ToString() ?? string.Empty,
                        Total = Convert.ToDecimal(data["total"]),
                        Status = data["status"].ToString() ?? string.Empty,
                        PaymentMethod = data["paymentMethod"].ToString() ?? string.Empty,
                        TrackingNumber = data.ContainsKey("trackingNumber") ? data["trackingNumber"].ToString() : null,
                        Notes = data.ContainsKey("notes") ? data["notes"].ToString() : null
                    };
                    orders.Add(order);
                }

                _logger.LogInformation("Kullanıcı için {Count} sipariş bulundu. User ID: {Id}", orders.Count, id);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı siparişleri getirilirken hata oluştu. User ID: {Id}", id);
                return StatusCode(500, "Kullanıcı siparişleri getirilirken bir hata oluştu");
            }
        }
    }
} 