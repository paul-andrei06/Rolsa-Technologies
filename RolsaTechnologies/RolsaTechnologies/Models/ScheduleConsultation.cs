using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace RolsaTechnologies.Models
{
    public class ScheduleConsultation
    {
        public int Id { get; set; } // This is the Primary Key
        public string UserId { get; set; } // This is the Foreign Key linking to the default IdentityUser

        [Display(Name = "Date")]
        public DateTime ScheduledDate { get; set; } // This is the Date and time scheduled for the Consultation
        public string ContactMethod { get; set; } // This will be the users choice of contact method (Email or Phone)

        [Display(Name = "Phone Number")]
        public string? Mobile {  get; set; } // This is the phone number of the customer so that they can be contacted
        public string? ContactEmail { get; set; } // This is the email of the customer so that they can be contacted
        public string? Notes { get; set; } // Any additional notes from the customer, not required
    }
}
