using Core;

namespace ComwellWeb.Services.Interfaces;

public interface ICommentService
{
    Task<List<Comment>?> GetCommentsBySubGoalId(int subGoalId);
    void AddComment(Comment comment);
}