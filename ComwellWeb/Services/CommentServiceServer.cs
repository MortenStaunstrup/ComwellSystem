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
    
    
    public async Task<List<Comment>?> GetCommentsBySubGoalId(int subGoalId)
    {
        Console.WriteLine($"Getting student {subGoalId} comments: service");
        var result = await _client.GetFromJsonAsync<List<Comment>?>($"{BaseURL}/getcomments/{subGoalId}");
        if (result == null || result.Count == 0)
        {
            Console.WriteLine("Comments null: service");
            return null;
        }

        return result;
    }

    public void AddComment(Comment comment)
    {
        Console.WriteLine($"Adding comment for subgoal {comment.CommentSubGoalId}: service");
        _client.PostAsJsonAsync($"{BaseURL}/addcomment", comment);
    }
}