using Core;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class NotificationService : INotificationService
{
    private readonly HttpClient _httpClient;
    private string BaseURL = "http://localhost:5116/api/Notification"; 

    public NotificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    // Create
    public async Task SendNotificationAsync(Notification notification) 
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseURL}/send", notification);
        response.EnsureSuccessStatusCode();
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Fejl i serverresponse: {errorContent}");
            throw new Exception($"Fejl fra server: {response.StatusCode}");
        }
    }
    
    // User
    public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
    {
        var notifications = await _httpClient.GetFromJsonAsync<List<Notification>>($"{BaseURL}/notifications/user/{userId}");
        
        return notifications ?? new List<Notification>();
    }


    // Subgoals
    public async Task ConfirmNotifiedSubGoalAsync(int userId, int notificationId, string miniGoalName)
    {
        var url = $"{BaseURL}/confirm/{userId}/{notificationId}/{miniGoalName}";
        var response = await _httpClient.PostAsync(url, null);
        response.EnsureSuccessStatusCode();
    }




    
    // Id
    public async Task<int> GetMaxNotificationIdAsync()
    {
        return await _httpClient.GetFromJsonAsync<int>($"{BaseURL}/maxid");
    }
}