namespace CarManagement.Models
{
    public class CarsIndexViewModel
    {
        public List<ElectricCar> ElectricCars { get; set; } = new();
        public List<GasCar> GasCars { get; set; } = new();
    }
}
