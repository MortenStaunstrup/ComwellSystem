using Blazored.LocalStorage;
using Core;
using System.Net.Http;
using System.Net.Http.Json;

namespace ComwellWeb.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;           // Bruges til at sende HTTP-requests til backend API
        private readonly ILocalStorageService _localStorage; // Bruges til at gemme data i browserens localStorage
        private string BaseURL = "http://localhost:5116/api/users";  // Base URL til bruger-API

        public UserService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;       // Får HttpClient indsprøjtet
            _localStorage = localStorage;   // Får localStorage service indsprøjtet
        }

        // Hent alle brugere fra API'et
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await _httpClient.GetFromJsonAsync<List<User>>(BaseURL);
            return users ?? new List<User>();   // Returner tom liste hvis null
        }

        // Login: prøv at logge ind med email og password, og gem bruger i localStorage hvis OK
        public async Task<User?> Login(string email, string password)
        {
            var response = await _httpClient.GetAsync($"{BaseURL}/login/{email}/{password}");
            if (!response.IsSuccessStatusCode)
                return null;

            var user = await response.Content.ReadFromJsonAsync<User>();
            if (user != null && !string.IsNullOrEmpty(user.UserEmail))
            {
                await _localStorage.SetItemAsync("user", user);  // Gem brugeren lokalt i browseren
                return user;
            }

            return null;
        }

        // Hent den bruger der er logget ind, fra localStorage
        public async Task<User?> GetUserLoggedInAsync()
        {
            return await _localStorage.GetItemAsync<User>("user");
        }

        // Opret en ny bruger via API, og gem den i localStorage
        public async Task<User?> AddUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseURL}/opret", user);
            if (!response.IsSuccessStatusCode)
                return null;

            var createdUser = await response.Content.ReadFromJsonAsync<User>();
            if (createdUser != null)
                await _localStorage.SetItemAsync("user", createdUser);

            return createdUser;
        }

        // Opdater bruger i API (uden returnering)
        public async Task UpdateUser(User user)
        {
            await _httpClient.PutAsJsonAsync($"{BaseURL}/update", user);
        }

        // Log ud: fjern bruger-data fra localStorage
        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("user");
        }

        // Hent det højeste bruger-ID fra API
        public async Task<int> GetMaxUserId()
        {
            return await _httpClient.GetFromJsonAsync<int>($"{BaseURL}/maxid");
        }

        // (Ekstra) Hent bruger efter ID hvis nødvendigt
        public async Task<User> GetUserById(int id)
        {
            return await _httpClient.GetFromJsonAsync<User>($"{BaseURL}/{id}");
        }
    }
}
