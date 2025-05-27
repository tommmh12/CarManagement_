using System.Collections.Generic;

namespace CarManagement.Models
{
    public class CarsIndexViewModel
    {
        public List<ElectricCar> ElectricCars { get; set; }
        public List<GasCar> GasCars { get; set; }
    }
}
