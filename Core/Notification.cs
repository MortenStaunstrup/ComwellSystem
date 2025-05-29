namespace Core;

public class Notification
{
    /*
     
     - Embedded i users-collection.
     - Elev sender notifikationer gennem en knap til at færdiggøre delmål, som bliver bekræftet af deres leder.
     - Notifikationer bliver embedded i lederen.
     
     */
    
    public int NotificationId { get; set; } // PK, hvordan identificerer vi notifikationen?
    public int? UserId { get; set; } // FK, hvordan finder modtageren til notifikationen?
    public int? SenderId { get; set; } // hvilken elev har sendt notifikationen?
    public string? MiniGoalName { get; set; } // FK, opret notifikation for færdigt minigoal
    public string? MiddleGoalName { get; set; } // FK, opret notifikation for færdigt middlegoal
    public string NotificationContent { get; set; } // hvilken besked får læreren når de næser notifikationen?
    
    public DateTime TimeStamp { get; set; } // hvornår er notifikationen oprettet?
    
    public bool IsConfirmed { get; set; } // er notifikationen bekræftet?
}