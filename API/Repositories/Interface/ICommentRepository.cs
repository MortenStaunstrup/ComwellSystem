using Core;

namespace API.Repositories.Interface;

public interface ICommentRepository
{
    Task<List<Comment>?> GetCommentsBySubGoalAndStudentId(int studentId, int subGoalId);
    void AddComment(Comment comment);
}