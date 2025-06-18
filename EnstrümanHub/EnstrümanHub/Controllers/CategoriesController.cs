using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using EnstrümanHub.Models;
using Google.Apis.Auth.OAuth2;

namespace EnstrümanHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Şimdilik auth kontrolünü kaldırdık
    public class CategoriesController : ControllerBase
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly ILogger<CategoriesController> _logger;
        private readonly IConfiguration _configuration;

        public CategoriesController(IConfiguration configuration, ILogger<CategoriesController> logger)
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

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                _logger.LogInformation("Tüm kategoriler getiriliyor...");
                
                var categories = new List<Category>();
                var snapshot = await _firestoreDb.Collection("categories").GetSnapshotAsync();

                foreach (var doc in snapshot)
                {
                    var data = doc.ToDictionary();
                    var category = new Category
                    {
                        Id = doc.Id,
                        Name = data["name"].ToString() ?? string.Empty
                    };
                    categories.Add(category);
                }

                _logger.LogInformation("Toplam {Count} kategori bulundu", categories.Count);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kategoriler getirilirken hata oluştu");
                return StatusCode(500, "Kategoriler getirilirken bir hata oluştu");
            }
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(string id)
        {
            try
            {
                _logger.LogInformation("Kategori getiriliyor. ID: {Id}", id);
                
                var docRef = _firestoreDb.Collection("categories").Document(id);
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Kategori bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan kategori bulunamadı");
                }

                var data = snapshot.ToDictionary();
                var category = new Category
                {
                    Id = snapshot.Id,
                    Name = data["name"].ToString() ?? string.Empty
                };

                _logger.LogInformation("Kategori başarıyla getirildi. ID: {Id}", id);
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kategori getirilirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Kategori getirilirken bir hata oluştu");
            }
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            try
            {
                _logger.LogInformation("Yeni kategori ekleniyor...");

                // Firestore için Dictionary oluştur
                var categoryData = new Dictionary<string, object>
                {
                    { "name", category.Name }
                };

                // Firestore'a ekle
                var docRef = await _firestoreDb.Collection("categories").AddAsync(categoryData);
                category.Id = docRef.Id;

                _logger.LogInformation("Yeni kategori başarıyla eklendi. ID: {Id}", category.Id);
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kategori eklenirken hata oluştu");
                return StatusCode(500, "Kategori eklenirken bir hata oluştu");
            }
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, Category category)
        {
            try
            {
                _logger.LogInformation("Kategori güncelleniyor. ID: {Id}", id);

                if (id != category.Id)
                {
                    _logger.LogWarning("Kategori ID'si eşleşmiyor. Path ID: {PathId}, Body ID: {BodyId}", id, category.Id);
                    return BadRequest("Kategori ID'si eşleşmiyor");
                }

                var docRef = _firestoreDb.Collection("categories").Document(id);
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Kategori bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan kategori bulunamadı");
                }

                // Firestore için Dictionary oluştur
                var categoryData = new Dictionary<string, object>
                {
                    { "name", category.Name }
                };

                await docRef.SetAsync(categoryData, SetOptions.MergeAll);
                _logger.LogInformation("Kategori başarıyla güncellendi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kategori güncellenirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Kategori güncellenirken bir hata oluştu");
            }
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            try
            {
                _logger.LogInformation("Kategori siliniyor. ID: {Id}", id);

                var docRef = _firestoreDb.Collection("categories").Document(id);
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Kategori bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan kategori bulunamadı");
                }

                await docRef.DeleteAsync();
                _logger.LogInformation("Kategori başarıyla silindi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kategori silinirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Kategori silinirken bir hata oluştu");
            }
        }
    }
} 