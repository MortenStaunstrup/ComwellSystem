using System.Net.Http.Json;
using ComwellWeb.Services.Interfaces;
using Core;

namespace ComwellWeb.Services;

public class CommentServiceServer : ICommentService
{
    private HttpClient _client;
    private readonly string BaseURL = "http://localhost:5116/api/comments";

    public CommentServiceServer(HttpClient client)
    {
        
        _client = client;
    }
    
    
    public async Task<List<Comment>?> GetCommentsBySubGoalId(int subGoalId, int studentId)
    {
        Console.WriteLine($"Getting student {studentId} comments for subgoal {subGoalId}: service");
        var result = await _client.GetFromJsonAsync<List<Comment>?>($"{BaseURL}/getcomments/{subGoalId}/{studentId}");
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("Comments null: service");
            return null;
            
        }
        Console.WriteLine($"Returning comments for subgoal {subGoalId}: service");
        return result;
    }

    public void AddComment(Comment comment)
    {
        Console.WriteLine($"Adding comment to subgoal {comment.CommentSubGoalId} for student {comment.StudentId}: service");
        _client.PostAsJsonAsync($"{BaseURL}/addcomment", comment);
    }
}