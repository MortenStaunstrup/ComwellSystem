using API.Repositories.Interface;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentController : ControllerBase
{
    private ICommentRepository commentRepository;

    public CommentController(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    [HttpGet]
    [Route("getcomments/{subgoalId:int}/{studentId}")]
    public async Task<List<Comment>?> GetCommentsBySubGoalIdAsync(int subgoalId, int studentId)
    {
        var result = await commentRepository.GetCommentsBySubGoalId(subgoalId, studentId);
        if (result == null)
        {
            Console.WriteLine($"No comments found for {subgoalId} returning empty list: controller");
            return new List<Comment>();
        }
        Console.WriteLine($"Returning comments: controller");
        return result;
    }

    [HttpPost]
    [Route("addcomment")]
    public async void AddCommentAsync(Comment comment)
    {
        Console.WriteLine($"Adding comment for subgoal {comment.CommentSubGoalId} for student {comment.StudentId}: controller");
        commentRepository.AddComment(comment);
    }
    
}