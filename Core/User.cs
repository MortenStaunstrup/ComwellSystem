using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{
    public class User
    {
        [BsonId]
        [BsonElement("_id")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "En bruger skal have et navn")]
        public string UserName { get; set; }
        
        public List<SubGoal> StudentPlan { get; set; } = new();
        public List<Message> Messages { get; set; } = new();

        
        public ObjectId? PictureId { get; set; }
        public string? Picture { get; set; }
        [Required(ErrorMessage = "En bruger skal have en rolle")]
        public string Role { get; set; }
        [Required(ErrorMessage = "En bruger skal have et password")]
        public string UserPassword { get; set; }
        [Required(ErrorMessage = "En bruger skal have en email")]
        public string UserEmail { get; set; }
        [Required(ErrorMessage = "En bruger skal have et telefonnummer")]
        public string UserPhone { get; set; }
        public DateOnly? StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int? UserIdResponsible { get; set; }
    }
}