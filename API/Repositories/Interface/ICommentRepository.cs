using Core;

namespace API.Repositories.Interface;

public interface ICommentRepository
{
    Task<List<Comment>?> GetCommentsBySubGoalId(int subGoalId, int studentId);
    void AddComment(Comment comment);
}