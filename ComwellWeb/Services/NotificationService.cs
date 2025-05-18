using Core;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class NotificationService : INotificationService
{
    private readonly HttpClient _httpClient;
    private string BaseURL = "http://localhost:5116/api/notification"; 

    public NotificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendNotificationAsync(Notification notification) 
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseURL}/send", notification);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<Notification>> GetNotificationsForUserAsync(int userId)
    {
        var notifications = await _httpClient.GetFromJsonAsync<List<Notification>>($"{BaseURL}/user/{userId}");
        return notifications ?? new List<Notification>();
    }

    public async Task ConfirmSubGoalCompletionAsync(int notificationId)
    {
        var response = await _httpClient.PostAsync($"{BaseURL}/confirm/{notificationId}", null);  
        response.EnsureSuccessStatusCode();
    }
    public async Task<int> GetMaxNotificationIdAsync()
    {
        return await _httpClient.GetFromJsonAsync<int>($"{BaseURL}/maxid");
    }

    
}