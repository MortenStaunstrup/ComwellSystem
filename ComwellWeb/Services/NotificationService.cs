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
    public async Task SendMiniGoalNotificationAsync(Notification notification) 
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseURL}/send-mini-goal", notification);
        response.EnsureSuccessStatusCode();
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Fejl i serverresponse: {errorContent}");
            throw new Exception($"Fejl fra server: {response.StatusCode}");
        }
    }
    
    public async Task SendMiddleGoalNotificationAsync(Notification notification) 
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseURL}/send-middle-goal", notification);
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
    public async Task ConfirmNotifiedMiniGoalAsync(int userId, int notificationId, string miniGoalName)
    {
        var url = $"{BaseURL}/confirm-mini-goal/{userId}/{notificationId}/{miniGoalName}";
        var response = await _httpClient.PostAsync(url, null);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task ConfirmNotifiedMiddleGoalAsync(int userId, int notificationId, string middleGoalName)
    {
        var url = $"{BaseURL}/confirm-middle-goal/{userId}/{notificationId}/{middleGoalName}";
        var response = await _httpClient.PostAsync(url, null);
        response.EnsureSuccessStatusCode();
    }


    
    // Id
    public async Task<int> GetMaxNotificationIdAsync()
    {
        return await _httpClient.GetFromJsonAsync<int>($"{BaseURL}/maxid");
    }
    
    public async Task<bool> NotificationExistsForMiddleGoalAsync(int userId, int senderId, string middleGoalName)
    {
        var response = await _httpClient.GetAsync($"{BaseURL}/exists-middle-goal?userId={userId}&senderId={senderId}&middleGoalName={middleGoalName}");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Fejl ved opslag af middel-mål notifikation: {response.StatusCode}");
            return false;
        }

        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<bool> NotificationExistsForMiniGoalAsync(int userId, int senderId, string miniGoalName)
    {
        var response = await _httpClient.GetAsync($"{BaseURL}/exists-mini-goal?userId={userId}&senderId={senderId}&miniGoalName={miniGoalName}");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Fejl ved opslag af mini-mål notifikation: {response.StatusCode}");
            return false;
        }

        return await response.Content.ReadFromJsonAsync<bool>();
    }


}