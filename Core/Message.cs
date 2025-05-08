namespace Core;

public class Message
{
    public int MessageId { get; set; }
    public string MessageContent { get; set; }
    public DateTime MessageDate { get; set; }
    public int MessegeSenderId { get; set; }
    public int? MessegeReceiverId { get; set; }
    
}