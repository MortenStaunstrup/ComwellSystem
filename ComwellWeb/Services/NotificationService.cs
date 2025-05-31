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
    
    // Har delt kategoriseringen af mål op i forskellige metoder. hhv. middlegoals og minigoals
    
    // sender en notifikation til en leder om et færdigt minigoal
    // param: notification - objektet der skal sendes
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
    
    // sender en notifikation til en leder om et færdigt middlegoal.
    // param: notification - objektet der skal sendes
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
    
    // henter alle notifikationer tilknyttet en bestemt bruger
    // param: userId - id på brugeren
    // return: liste over notifikationer
    public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
    {
        var notifications = await _httpClient.GetFromJsonAsync<List<Notification>>($"{BaseURL}/notifications/user/{userId}");
        
        return notifications ?? new List<Notification>();
    }


    // Subgoals
    
    // bekræfter at et minigoal er gennemført, baseret på en notifikation
    // param: userId - id på lederen, notificationId - id på notifikationen, miniGoalName - navnet på målet
    public async Task ConfirmNotifiedMiniGoalAsync(int userId, int notificationId, string miniGoalName)
    {
        var url = $"{BaseURL}/confirm-mini-goal/{userId}/{notificationId}/{miniGoalName}";
        var response = await _httpClient.PostAsync(url, null);
        response.EnsureSuccessStatusCode();
    }
    
    // bekræfter at et middelmål er gennemført, baseret på en notifikation
    // param: userId - id på lederen, notificationId - id på notifikationen, middleGoalName - navnet på målet
    public async Task ConfirmNotifiedMiddleGoalAsync(int userId, int notificationId, string middleGoalName)
    {
        var url = $"{BaseURL}/confirm-middle-goal/{userId}/{notificationId}/{middleGoalName}";
        var response = await _httpClient.PostAsync(url, null);
        response.EnsureSuccessStatusCode();
    }


    
    // Id
    
    
    // henter det højeste notification-id fra systemet
    // return: største id som int
    public async Task<int> GetMaxNotificationIdAsync()
    {
        return await _httpClient.GetFromJsonAsync<int>($"{BaseURL}/maxid");
    }
    
    // tjekker om en notifikation eksisterer for et bestemt middelmål mellem to brugere
    // param: userId - modtagerens id, senderId - afsenderens id, middleGoalName - målnavn
    // return: true hvis den findes, ellers false
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
    
    // tjekker om en notifikation eksisterer for et bestemt minigoal mellem to brugere
    // param: userId - modtagerens id, senderId - afsenderens id, miniGoalName - målnavn
    // return: true hvis den findes, ellers false
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