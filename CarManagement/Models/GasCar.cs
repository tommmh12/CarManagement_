namespace CarManagement.Models
{
    public class GasCar
    {
        public int Id { get; set; }
        public float FuelEff { get; set; }
        public string? Model { get; set; }
        public int BranchId { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
