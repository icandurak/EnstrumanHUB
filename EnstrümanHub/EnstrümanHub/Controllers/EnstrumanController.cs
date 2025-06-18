using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EnstrümanHub.Models;
using EnstrümanHub.Data;
using EnstrümanHub.Services;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EnstrümanHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Şimdilik auth kontrolünü kaldırdık
    public class EnstrumanController : ControllerBase
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly ILogger<EnstrumanController> _logger;
        private const string GITAR_COLLECTION = "guitars";
        private const string BASS_COLLECTION = "bass";
        private const string DRUM_COLLECTION = "drums";

        public EnstrumanController(FirestoreDb firestoreDb, ILogger<EnstrumanController> logger)
        {
            _firestoreDb = firestoreDb;
            _logger = logger;
            _logger.LogInformation("EnstrumanController başlatıldı");
        }

        // GET: api/Enstruman/Gitar
        [HttpGet("Gitar")]
        public async Task<ActionResult<IEnumerable<Gitar>>> GetGitarlar()
        {
            try
            {
                _logger.LogInformation("Gitarlar getiriliyor...");
                var collection = _firestoreDb.Collection("guitars");
                var snapshot = await collection.GetSnapshotAsync();
                var gitarlar = snapshot.Documents.Select(doc =>
                {
                    var data = doc.ToDictionary();
                    return new Gitar
                    {
                        Id = int.Parse(doc.Id),
                        Name = data["name"].ToString() ?? string.Empty,
                        Brand = data["brand"].ToString() ?? string.Empty,
                        Price = Convert.ToInt32(data["price"]),
                        Description = data["description"].ToString() ?? string.Empty,
                        ImageUrl = data["imageUrl"].ToString() ?? string.Empty,
                        Stock = Convert.ToInt32(data["stock"])
                    };
                }).ToList();

                _logger.LogInformation("{Count} gitar başarıyla getirildi", gitarlar.Count);
                return Ok(gitarlar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gitarlar getirilirken hata oluştu");
                return StatusCode(500, "Gitarlar getirilirken bir hata oluştu");
            }
        }

        // POST: api/Enstruman/Gitar
        [HttpPost("Gitar")]
        public async Task<IActionResult> CreateGitar(Gitar gitar)
        {
            try
            {
                _logger.LogInformation("Yeni gitar ekleniyor...");

                // Tüm gitar koleksiyonunu al
                var allGuitars = await _firestoreDb.Collection("guitars").GetSnapshotAsync();
                
                // En yüksek ID'yi bul
                int maxId = 0;
                foreach (var doc in allGuitars)
                {
                    var data = doc.ToDictionary();
                    if (data.ContainsKey("id"))
                    {
                        int currentId = Convert.ToInt32(data["id"]);
                        if (currentId > maxId)
                        {
                            maxId = currentId;
                        }
                    }
                }

                // Yeni ID'yi ata (en yüksek ID + 1)
                int newId = maxId + 1;
                gitar.Id = newId;

                // Firestore için Dictionary oluştur
                var gitarData = new Dictionary<string, object>
                {
                    { "id", gitar.Id },
                    { "name", gitar.Name },
                    { "brand", gitar.Brand },
                    { "price", gitar.Price },
                    { "description", gitar.Description },
                    { "imageUrl", gitar.ImageUrl },
                    { "stock", gitar.Stock }
                };

                // Firestore'a ekle
                var docRef = _firestoreDb.Collection("guitars").Document(newId.ToString());
                await docRef.CreateAsync(gitarData);

                _logger.LogInformation("Yeni gitar başarıyla eklendi. ID: {Id}", newId);
                return CreatedAtAction(nameof(GetGitar), new { id = gitar.Id }, gitar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gitar eklenirken hata oluştu");
                return StatusCode(500, "Gitar eklenirken bir hata oluştu");
            }
        }

        // PUT: api/Enstruman/Gitar/{id}
        [HttpPut("Gitar/{id}")]
        public async Task<IActionResult> UpdateGitar(int id, Gitar gitar)
        {
            try
            {
                _logger.LogInformation("Gitar güncelleniyor. ID: {Id}", id);
                
                if (id != gitar.Id)
                {
                    return BadRequest("ID'ler eşleşmiyor");
                }

                var docRef = _firestoreDb.Collection("guitars").Document(id.ToString());
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Gitar bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan gitar bulunamadı");
                }

                var data = new Dictionary<string, object>
                {
                    { "name", gitar.Name },
                    { "brand", gitar.Brand },
                    { "price", gitar.Price },
                    { "description", gitar.Description },
                    { "imageUrl", gitar.ImageUrl },
                    { "stock", gitar.Stock }
                };

                await docRef.SetAsync(data);
                _logger.LogInformation("Gitar başarıyla güncellendi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gitar güncellenirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Gitar güncellenirken bir hata oluştu");
            }
        }

        // GET: api/Enstruman/Gitar/{id}
        [HttpGet("Gitar/{id}")]
        public async Task<ActionResult<Gitar>> GetGitar(int id)
        {
            try
            {
                _logger.LogInformation("Gitar getiriliyor. ID: {Id}", id);
                var docRef = _firestoreDb.Collection("guitars").Document(id.ToString());
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Gitar bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan gitar bulunamadı");
                }

                var data = snapshot.ToDictionary();
                var gitar = new Gitar
                {
                    Id = int.Parse(snapshot.Id),
                    Name = data["name"].ToString() ?? string.Empty,
                    Brand = data["brand"].ToString() ?? string.Empty,
                    Price = Convert.ToInt32(data["price"]),
                    Description = data["description"].ToString() ?? string.Empty,
                    ImageUrl = data["imageUrl"].ToString() ?? string.Empty,
                    Stock = Convert.ToInt32(data["stock"])
                };

                _logger.LogInformation("Gitar başarıyla getirildi. ID: {Id}", id);
                return Ok(gitar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gitar getirilirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Gitar getirilirken bir hata oluştu");
            }
        }

        // DELETE: api/Enstruman/Gitar/{id}
        [HttpDelete("Gitar/{id}")]
        public async Task<IActionResult> DeleteGitar(int id)
        {
            try
            {
                _logger.LogInformation("Gitar siliniyor. ID: {Id}", id);
                
                var docRef = _firestoreDb.Collection("guitars").Document(id.ToString());
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Gitar bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan gitar bulunamadı");
                }

                await docRef.DeleteAsync();
                _logger.LogInformation("Gitar başarıyla silindi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gitar silinirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Gitar silinirken bir hata oluştu");
            }
        }

        // Bass Gitar Endpoint'leri
        [HttpGet("BassGitar")]
        public async Task<ActionResult<IEnumerable<BassGitar>>> GetBassGitarlar()
        {
            try
            {
                _logger.LogInformation("Bass gitarlar getiriliyor...");
                var collection = _firestoreDb.Collection("bass");
                var snapshot = await collection.GetSnapshotAsync();
                var bassGitarlar = snapshot.Documents.Select(doc =>
                {
                    var data = doc.ToDictionary();
                    return new BassGitar
                    {
                        Id = int.Parse(doc.Id),
                        Name = data["name"].ToString() ?? string.Empty,
                        Brand = data["brand"].ToString() ?? string.Empty,
                        Price = Convert.ToInt32(data["price"]),
                        Description = data["description"].ToString() ?? string.Empty,
                        ImageUrl = data["imageUrl"].ToString() ?? string.Empty,
                        Stock = Convert.ToInt32(data["stock"])
                    };
                }).ToList();

                _logger.LogInformation("{Count} bass gitar başarıyla getirildi", bassGitarlar.Count);
                return Ok(bassGitarlar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bass gitarlar getirilirken hata oluştu");
                return StatusCode(500, "Bass gitarlar getirilirken bir hata oluştu");
            }
        }

        [HttpPost("BassGitar")]
        public async Task<IActionResult> CreateBassGitar(BassGitar bassGitar)
        {
            try
            {
                _logger.LogInformation("Yeni bass gitar ekleniyor...");

                var lastBassGitar = await _firestoreDb.Collection("bass")
                    .OrderByDescending("id")
                    .Limit(1)
                    .GetSnapshotAsync();

                int newId = 1;
                if (lastBassGitar.Count > 0)
                {
                    var lastData = lastBassGitar[0].ToDictionary();
                    newId = Convert.ToInt32(lastData["id"]) + 1;
                }

                bassGitar.Id = newId;

                var bassGitarData = new Dictionary<string, object>
                {
                    { "id", bassGitar.Id },
                    { "name", bassGitar.Name },
                    { "brand", bassGitar.Brand },
                    { "price", bassGitar.Price },
                    { "description", bassGitar.Description },
                    { "imageUrl", bassGitar.ImageUrl },
                    { "stock", bassGitar.Stock }
                };

                var docRef = _firestoreDb.Collection("bass").Document(newId.ToString());
                await docRef.CreateAsync(bassGitarData);

                _logger.LogInformation("Yeni bass gitar başarıyla eklendi. ID: {Id}", newId);
                return CreatedAtAction(nameof(GetBassGitar), new { id = bassGitar.Id }, bassGitar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bass gitar eklenirken hata oluştu");
                return StatusCode(500, "Bass gitar eklenirken bir hata oluştu");
            }
        }

        // GET: api/Enstruman/BassGitar/{id}
        [HttpGet("BassGitar/{id}")]
        public async Task<ActionResult<BassGitar>> GetBassGitar(int id)
        {
            try
            {
                _logger.LogInformation("Bass gitar getiriliyor. ID: {Id}", id);
                var docRef = _firestoreDb.Collection("bass").Document(id.ToString());
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Bass gitar bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan bass gitar bulunamadı");
                }

                var data = snapshot.ToDictionary();
                var bassGitar = new BassGitar
                {
                    Id = int.Parse(snapshot.Id),
                    Name = data["name"].ToString() ?? string.Empty,
                    Brand = data["brand"].ToString() ?? string.Empty,
                    Price = Convert.ToInt32(data["price"]),
                    Description = data["description"].ToString() ?? string.Empty,
                    ImageUrl = data["imageUrl"].ToString() ?? string.Empty,
                    Stock = Convert.ToInt32(data["stock"])
                };

                _logger.LogInformation("Bass gitar başarıyla getirildi. ID: {Id}", id);
                return Ok(bassGitar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bass gitar getirilirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Bass gitar getirilirken bir hata oluştu");
            }
        }

        [HttpPut("BassGitar/{id}")]
        public async Task<IActionResult> UpdateBassGitar(int id, BassGitar bassGitar)
        {
            try
            {
                _logger.LogInformation("Bass gitar güncelleniyor. ID: {Id}", id);
                
                if (id != bassGitar.Id)
                {
                    return BadRequest("ID'ler eşleşmiyor");
                }

                var docRef = _firestoreDb.Collection("bass").Document(id.ToString());
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Bass gitar bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan bass gitar bulunamadı");
                }

                var data = new Dictionary<string, object>
                {
                    { "name", bassGitar.Name },
                    { "brand", bassGitar.Brand },
                    { "price", bassGitar.Price },
                    { "description", bassGitar.Description },
                    { "imageUrl", bassGitar.ImageUrl },
                    { "stock", bassGitar.Stock }
                };

                await docRef.SetAsync(data);
                _logger.LogInformation("Bass gitar başarıyla güncellendi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bass gitar güncellenirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Bass gitar güncellenirken bir hata oluştu");
            }
        }

        // PATCH: api/Enstruman/BassGitar/{id}
        [HttpPatch("BassGitar/{id}")]
        public async Task<IActionResult> PatchBassGitar(int id, [FromBody] Dictionary<string, object> updates)
        {
            try
            {
                _logger.LogInformation("Bass gitar kısmi güncelleniyor. ID: {Id}", id);
                
                var docRef = _firestoreDb.Collection("bass").Document(id.ToString());
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Bass gitar bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan bass gitar bulunamadı");
                }

                await docRef.UpdateAsync(updates);
                _logger.LogInformation("Bass gitar başarıyla kısmi güncellendi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bass gitar kısmi güncellenirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Bass gitar kısmi güncellenirken bir hata oluştu");
            }
        }

        [HttpDelete("BassGitar/{id}")]
        public async Task<IActionResult> DeleteBassGitar(int id)
        {
            try
            {
                _logger.LogInformation("Bass gitar siliniyor. ID: {Id}", id);
                
                var docRef = _firestoreDb.Collection("bass").Document(id.ToString());
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Bass gitar bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan bass gitar bulunamadı");
                }

                await docRef.DeleteAsync();
                _logger.LogInformation("Bass gitar başarıyla silindi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bass gitar silinirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Bass gitar silinirken bir hata oluştu");
            }
        }

        // Bateri Endpoint'leri
        [HttpGet("Bateri")]
        public async Task<ActionResult<IEnumerable<Bateri>>> GetBateriler()
        {
            try
            {
                _logger.LogInformation("Bateriler getiriliyor...");
                var collection = _firestoreDb.Collection("drums");
                var snapshot = await collection.GetSnapshotAsync();
                var bateriler = snapshot.Documents.Select(doc =>
                {
                    var data = doc.ToDictionary();
                    return new Bateri
                    {
                        Id = int.Parse(doc.Id),
                        Name = data["name"].ToString() ?? string.Empty,
                        Brand = data["brand"].ToString() ?? string.Empty,
                        Price = Convert.ToInt32(data["price"]),
                        Description = data["description"].ToString() ?? string.Empty,
                        ImageUrl = data["imageUrl"].ToString() ?? string.Empty,
                        Stock = Convert.ToInt32(data["stock"])
                    };
                }).ToList();

                _logger.LogInformation("{Count} bateri başarıyla getirildi", bateriler.Count);
                return Ok(bateriler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bateriler getirilirken hata oluştu");
                return StatusCode(500, "Bateriler getirilirken bir hata oluştu");
            }
        }

        [HttpPost("Bateri")]
        public async Task<IActionResult> CreateBateri(Bateri bateri)
        {
            try
            {
                _logger.LogInformation("Yeni bateri ekleniyor...");

                var lastBateri = await _firestoreDb.Collection("drums")
                    .OrderByDescending("id")
                    .Limit(1)
                    .GetSnapshotAsync();

                int newId = 1;
                if (lastBateri.Count > 0)
                {
                    var lastData = lastBateri[0].ToDictionary();
                    newId = Convert.ToInt32(lastData["id"]) + 1;
                }

                bateri.Id = newId;

                var bateriData = new Dictionary<string, object>
                {
                    { "id", bateri.Id },
                    { "name", bateri.Name },
                    { "brand", bateri.Brand },
                    { "price", bateri.Price },
                    { "description", bateri.Description },
                    { "imageUrl", bateri.ImageUrl },
                    { "stock", bateri.Stock }
                };

                var docRef = _firestoreDb.Collection("drums").Document(newId.ToString());
                await docRef.CreateAsync(bateriData);

                _logger.LogInformation("Yeni bateri başarıyla eklendi. ID: {Id}", newId);
                return CreatedAtAction(nameof(GetBateri), new { id = bateri.Id }, bateri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bateri eklenirken hata oluştu");
                return StatusCode(500, "Bateri eklenirken bir hata oluştu");
            }
        }

        // GET: api/Enstruman/Bateri/{id}
        [HttpGet("Bateri/{id}")]
        public async Task<ActionResult<Bateri>> GetBateri(int id)
        {
            try
            {
                _logger.LogInformation("Bateri getiriliyor. ID: {Id}", id);
                var docRef = _firestoreDb.Collection("drums").Document(id.ToString());
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Bateri bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan bateri bulunamadı");
                }

                var data = snapshot.ToDictionary();
                var bateri = new Bateri
                {
                    Id = int.Parse(snapshot.Id),
                    Name = data["name"].ToString() ?? string.Empty,
                    Brand = data["brand"].ToString() ?? string.Empty,
                    Price = Convert.ToInt32(data["price"]),
                    Description = data["description"].ToString() ?? string.Empty,
                    ImageUrl = data["imageUrl"].ToString() ?? string.Empty,
                    Stock = Convert.ToInt32(data["stock"])
                };

                _logger.LogInformation("Bateri başarıyla getirildi. ID: {Id}", id);
                return Ok(bateri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bateri getirilirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Bateri getirilirken bir hata oluştu");
            }
        }

        [HttpPut("Bateri/{id}")]
        public async Task<IActionResult> UpdateBateri(int id, Bateri bateri)
        {
            try
            {
                _logger.LogInformation("Bateri güncelleniyor. ID: {Id}", id);
                
                if (id != bateri.Id)
                {
                    return BadRequest("ID'ler eşleşmiyor");
                }

                var docRef = _firestoreDb.Collection("drums").Document(id.ToString());
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Bateri bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan bateri bulunamadı");
                }

                var data = new Dictionary<string, object>
                {
                    { "name", bateri.Name },
                    { "brand", bateri.Brand },
                    { "price", bateri.Price },
                    { "description", bateri.Description },
                    { "imageUrl", bateri.ImageUrl },
                    { "stock", bateri.Stock }
                };

                await docRef.SetAsync(data);
                _logger.LogInformation("Bateri başarıyla güncellendi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bateri güncellenirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Bateri güncellenirken bir hata oluştu");
            }
        }

        // PATCH: api/Enstruman/Bateri/{id}
        [HttpPatch("Bateri/{id}")]
        public async Task<IActionResult> PatchBateri(int id, [FromBody] Dictionary<string, object> updates)
        {
            try
            {
                _logger.LogInformation("Bateri kısmi güncelleniyor. ID: {Id}", id);
                
                var docRef = _firestoreDb.Collection("drums").Document(id.ToString());
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Bateri bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan bateri bulunamadı");
                }

                await docRef.UpdateAsync(updates);
                _logger.LogInformation("Bateri başarıyla kısmi güncellendi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bateri kısmi güncellenirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Bateri kısmi güncellenirken bir hata oluştu");
            }
        }

        [HttpDelete("Bateri/{id}")]
        public async Task<IActionResult> DeleteBateri(int id)
        {
            try
            {
                _logger.LogInformation("Bateri siliniyor. ID: {Id}", id);
                
                var docRef = _firestoreDb.Collection("drums").Document(id.ToString());
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Bateri bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan bateri bulunamadı");
                }

                await docRef.DeleteAsync();
                _logger.LogInformation("Bateri başarıyla silindi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bateri silinirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Bateri silinirken bir hata oluştu");
            }
        }

        // PATCH: api/Enstruman/Gitar/{id}
        [HttpPatch("Gitar/{id}")]
        public async Task<IActionResult> PatchGitar(int id, [FromBody] Dictionary<string, object> updates)
        {
            try
            {
                _logger.LogInformation("Gitar kısmi güncelleniyor. ID: {Id}", id);
                
                var docRef = _firestoreDb.Collection("guitars").Document(id.ToString());
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    _logger.LogWarning("Gitar bulunamadı. ID: {Id}", id);
                    return NotFound($"ID'si {id} olan gitar bulunamadı");
                }

                await docRef.UpdateAsync(updates);
                _logger.LogInformation("Gitar başarıyla kısmi güncellendi. ID: {Id}", id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gitar kısmi güncellenirken hata oluştu. ID: {Id}", id);
                return StatusCode(500, "Gitar kısmi güncellenirken bir hata oluştu");
            }
        }
    }
}
