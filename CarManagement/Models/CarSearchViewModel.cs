namespace CarManagement.Models
{
    public class CarSearchViewModel
    {
        public int? BranchId { get; set; }
        public string? CarType { get; set; } // "Gas", "Electric"
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }


        public List<ElectricCar> ElectricResults { get; set; } = new();
        public List<GasCar> GasResults { get; set; } = new();
    }
}
    