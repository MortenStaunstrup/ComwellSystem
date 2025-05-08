namespace Core;

public class Comment
{
    public int CommentId { get; set; }
    public string CommentContent { get; set; }
    public DateTime CommentDate { get; set; }
    public int CommentSenderId { get; set; }
}