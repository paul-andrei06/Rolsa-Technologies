namespace RolsaTechnologies.Models
{
    public class Calculator
    {
        public int Id { get; set; } // This is the primary key
        public string UserId { get; set; } // This is the foreign key linking to the IdentityUser

        public double ElectricityUsage { get; set; } 
        public double GasUsage { get; set; } 
        public double CarMilesPerWeek { get; set; } // Average miles driven per week
        public double CarFuelEfficiency { get; set; } // Fuel efficiency (mpg)
        public double PublicTransportMilesPerWeek { get; set; } // weekly distance travelled by public transport (bus/train)
        public double WasteProducedPerWeek { get; set; } // Weekly waste produced (kg)
        public bool RecyclingHabits { get; set; } // Indicates if the user recycles (True or False)
        public double MeatConsumptionPerWeek { get; set; } //Weekly meat consumption (kg)

        public double CalculatedCarbonFootprint { get; set; } // final calculation based on inputs

        public DateTime DateCalculated { get; set; } = DateTime.Now; // This will automatically retrieve the Date and Time that the user submitted the form

        public void CalculateFootprint()
        {
            // Home Energy
            double electricityCarbon = (ElectricityUsage * 12) * 0.233;
            double gasCarbon = (GasUsage * 12 ) * 0.184; 

            // Transportation
            double carCarbon = (CarMilesPerWeek * 52) / CarFuelEfficiency * 2.31; 
            double publicTransportCarbon = PublicTransportMilesPerWeek * 52 * 0.105;  

            // Waste
            double wasteCarbon = WasteProducedPerWeek * 52 * 0.057;  
            double recyclingCredit = RecyclingHabits ? -wasteCarbon * 0.5 : 0; 

            // Diet
            double meatCarbon = MeatConsumptionPerWeek * 52 * 26.5;  

            // Total Annual Carbon Footprint in kg CO2
            double totalCarbonFootprint = electricityCarbon + gasCarbon + carCarbon + publicTransportCarbon + wasteCarbon + recyclingCredit + meatCarbon;

            // Convert to metric tons
            CalculatedCarbonFootprint = totalCarbonFootprint / 1000;
        }
    }
}
