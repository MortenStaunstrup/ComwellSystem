namespace Core;

public class Message
{
    private int MessageId { get; set; }
    private string MessageContent { get; set; }
    private DateTime MessageDate { get; set; }
    private int MessegeSenderId { get; set; }
    private int? MessegeReceiverId { get; set; }
    
}