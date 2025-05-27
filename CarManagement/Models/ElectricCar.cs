namespace CarManagement.Models
{
    public class ElectricCar
    {
        public int Id { get; set; }
        public int Battery { get; set; }
        public int RangeKm { get; set; }
        public string? Model { get; set; }
        public int BranchId { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
