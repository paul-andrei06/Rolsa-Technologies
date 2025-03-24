using System.ComponentModel.DataAnnotations;

namespace RolsaTechnologies.Models
{
    public class EnergyTracker
    {
        public int Id { get; set; } // This is the primary key
        public string UserId { get; set; } // This is the foreign key linking to the default IdentityUser

        [Display(Name = "Monthly Consumption in kWh")]
        public double Consumption { get; set; } // This is the consumption of the user in kWh per month

        [Display(Name = "Energy Type")]
        public string EnergyType { get; set; } // This is the Energy Type that the customer has used (drop down list)

        public DateOnly Date { get; set; } // This is the date that the form has been submitted (will not be visible in the form)
    }
}
