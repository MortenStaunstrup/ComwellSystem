namespace Core;


// Dette er et 'data transfer object' som kun bliver brugt til 'InsertSpecificSubgoal' funktion i UserController
// da man kun kan sende ét object over http, bliver vi nødt til at samle de 2 felter i ét samlet object
public class SubGoalRequest
{
    public SubGoal SubGoal { get; set; }
    public List<int> StudentIds { get; set; }
}