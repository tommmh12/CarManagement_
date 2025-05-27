namespace CarManagement.Models
{
    public class Cars
    {
        public string? CarType { get; set; } // "Electric" hoặc "Gas"

        // Các trường dùng chung
        public string? Model { get; set; }
        public int BranchId { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        // Chỉ dùng cho xe điện
        public int? Battery { get; set; }
        public int? RangeKm { get; set; }

        // Chỉ dùng cho xe xăng
        public float? FuelEff { get; set; }
    }
}
