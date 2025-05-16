using Core;

namespace ComwellWeb.Services.Interfaces;

public interface ICommentService
{
    Task<List<Comment>?> GetCommentsBySubGoalId(int subGoalId, int studentId);
    void AddComment(Comment comment);
}