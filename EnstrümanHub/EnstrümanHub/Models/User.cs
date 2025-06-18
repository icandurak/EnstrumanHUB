using Google.Cloud.Firestore;

namespace EnstrümanHub.Models
{
    [FirestoreData]
    public class User
    {
        public string Id { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Name { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Email { get; set; } = string.Empty;
    }
} 