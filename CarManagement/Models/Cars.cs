namespace CarManagement.Models
{
    public class Cars
    {
        public string? CarType { get; set; } // "Electric" hoặc "Gas"

        public string? Model { get; set; }
        public int BranchId { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        // Cho xe điện
        public int? Battery { get; set; }
        public int? RangeKm { get; set; }

        // Cho xe xăng
        public float? FuelEff { get; set; }
    }
}
