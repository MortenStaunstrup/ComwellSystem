using Blazored.LocalStorage;
using Core;
using System.Net.Http;
using System.Net.Http.Json;

namespace ComwellWeb.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;         
        private readonly ILocalStorageService _localStorage;
        private string BaseURL = "http://localhost:5116/api/users"; 

        public UserService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await _httpClient.GetFromJsonAsync<List<User>>(BaseURL);
            return users ?? new List<User>();
        }
        
        public async Task<List<User>> GetAllStudentsAsync()
        {
            var users = await _httpClient.GetFromJsonAsync<List<User>>(BaseURL);
            return users?.Where(u => u.Role == "Student").ToList() ?? new List<User>();
        }

   
        public async Task<User?> Login(string email, string password)
        {
            var response = await _httpClient.GetAsync($"{BaseURL}/login/{email}/{password}");
            if (!response.IsSuccessStatusCode)
                return null;

            var user = await response.Content.ReadFromJsonAsync<User>();
            if (user != null && !string.IsNullOrEmpty(user.UserEmail))
            {
                await _localStorage.SetItemAsync("user", user);
                return user;
            }

            return null;
        }
        
        public async Task<User?> GetUserLoggedInAsync()
        {
            return await _localStorage.GetItemAsync<User>("user");
        }
        
        public async Task<User?> AddUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseURL}/opret", user);
            if (!response.IsSuccessStatusCode)
                return null;

            //Todo fejl her, når du har oprettet én elev, og prøver at oprette én mere uden at reloade siden
            var createdUser = await response.Content.ReadFromJsonAsync<User>();
            if (createdUser != null)
                await _localStorage.SetItemAsync("user", createdUser);

            return createdUser;
        }
        
        public async Task UpdateUser(User user)
        {
            await _httpClient.PutAsJsonAsync($"{BaseURL}/update", user);
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("user");
        }

    
        public async Task<int> GetMaxUserId()
        {
            return await _httpClient.GetFromJsonAsync<int>($"{BaseURL}/maxid");
        }
        
        public async Task<User> GetUserById(int userId)
        {
            Console.WriteLine($"Returning user: {userId}: service");
            return await _httpClient.GetFromJsonAsync<User>($"{BaseURL}/user/{userId}");
        }
        
        public async Task<List<User>?> GetAllStudentsByResponsibleIdAsync(int responisbleId)
        {
            return await _httpClient.GetFromJsonAsync<List<User>?>($"{BaseURL}/students/{responisbleId}");
        }
        public async Task<bool> AddNotificationToUserAsync(int userId, Notification notification)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseURL}/{userId}/add-notification", notification);
            return response.IsSuccessStatusCode;
        }
        public async Task<User?> GetUserByUserId(int userId)
        {
            Console.WriteLine($"Returning user: {userId}: service");
            return await _httpClient.GetFromJsonAsync<User>($"{BaseURL}/user/{userId}");
        }

    }
}
