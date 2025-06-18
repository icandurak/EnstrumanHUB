using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EnstrümanHub.Services
{
    public interface IFirebaseAuthService
    {
        Task<string> VerifyTokenAsync(string idToken);
        Task<UserRecord> GetUserAsync(string uid);
        Task<UserRecord> CreateUserAsync(string email, string password);
        Task DeleteUserAsync(string uid);
    }

    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly FirebaseAuth _auth;

        public FirebaseAuthService()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("firebase-credentials.json")
                });
            }
            _auth = FirebaseAuth.DefaultInstance;
        }

        public async Task<string> VerifyTokenAsync(string idToken)
        {
            try
            {
                FirebaseToken decodedToken = await _auth.VerifyIdTokenAsync(idToken);
                return decodedToken.Uid;
            }
            catch (Exception ex)
            {
                throw new Exception("Token doğrulama hatası", ex);
            }
        }

        public async Task<UserRecord> GetUserAsync(string uid)
        {
            try
            {
                return await _auth.GetUserAsync(uid);
            }
            catch (Exception ex)
            {
                throw new Exception("Kullanıcı bulunamadı", ex);
            }
        }

        public async Task<UserRecord> CreateUserAsync(string email, string password)
        {
            try
            {
                var userArgs = new UserRecordArgs()
                {
                    Email = email,
                    Password = password,
                    EmailVerified = false
                };

                return await _auth.CreateUserAsync(userArgs);
            }
            catch (Exception ex)
            {
                throw new Exception("Kullanıcı oluşturma hatası", ex);
            }
        }

        public async Task DeleteUserAsync(string uid)
        {
            try
            {
                await _auth.DeleteUserAsync(uid);
            }
            catch (Exception ex)
            {
                throw new Exception("Kullanıcı silme hatası", ex);
            }
        }
    }
} 