using System.ComponentModel.DataAnnotations;

namespace RolsaTechnologies.Models
{
    public class ScheduleInstallation
    {
        public int Id { get; set; } // This is be the Primary Key for this model (Id of all the Installations scheduled)
        public string UserId { get; set; } // This is the Foreign Key linking to the default IdentityUser

        [Display(Name = "Date")]
        public DateTime ScheduledDate { get; set; } // This is the Date and Time scheduled for the installation

        [Display(Name = "Appliance Type")]
        public string ApplianceType { get; set; } // This is the appliance type which will be a drop down
        public string Address { get; set; }

        [Display(Name = "Phone Number")]
        public string Mobile { get; set; } // This is the phone number of the customer so that they can be contacted
        public string? Notes { get; set; } // Any additional notes from the customer to the professional, not required
    }
}
