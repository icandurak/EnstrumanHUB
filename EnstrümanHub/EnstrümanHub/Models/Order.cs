// Konum: Models/Order.cs

using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace EnstrümanHub.Models
{
    // Firestore'a kaydedilecek Order nesnesinin yapısını tanımlar.
    [FirestoreData] // Bu nitelik, sınıfın Firestore ile uyumlu olduğunu belirtir.
    public class Order
    {
        // Not: Firestore'da doküman ID'si genellikle ayrı tutulur, bu yüzden Id alanı
        // [FirestoreDocumentId] olarak işaretlenebilir, ancak biz manuel yönetiyoruz.
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string UserId { get; set; } = string.Empty;
        
        [FirestoreProperty("customerName")] // Firestore'daki alan adını belirtir (camelCase standardı için)
        public string CustomerName { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string BillingAddress { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Email { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        [FirestoreProperty]
        public decimal Total { get; set; }
        
        [FirestoreProperty]
        public string Status { get; set; } = "Beklemede";
        
        [FirestoreProperty]
        public string PaymentMethod { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string? TrackingNumber { get; set; } // Null olabilir
        
        [FirestoreProperty]
        public string? Notes { get; set; } // Null olabilir

        // Bu alanlar Firestore'a doğrudan yazılmaz, sadece program içinde kullanılır.
        // Eğer bunları da yazmak isterseniz [FirestoreProperty] ekleyin.
        public virtual User User { get; set; } = null!; // CS8618 uyarısını giderir

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}