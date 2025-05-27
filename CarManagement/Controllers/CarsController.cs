using Microsoft.AspNetCore.Mvc;
using CarManagement.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System;

namespace CarManagement.Controllers
{
    public class CarsController : Controller
    {
        private readonly IConfiguration _configuration;

        public CarsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string? GetConnection() =>
            _configuration.GetConnectionString("DefaultConnection");

        // GET: Danh sách xe
        public IActionResult Index()
        {
            var model = new CarsIndexViewModel
            {
                ElectricCars = new List<ElectricCar>(),
                GasCars = new List<GasCar>()
            };

            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();
                model.ElectricCars = GetElectricCars(conn);
                model.GasCars = GetGasCars(conn);
            }

            return View(model);
        }

        // GET: Form thêm xe
        public IActionResult Create()
        {
            ViewBag.Branches = GetBranches();
            return View(new Cars());
        }

        // POST: Thêm xe mới
        [HttpPost]
        public IActionResult Create(Cars car)
        {
            ViewBag.Branches = GetBranches();

            if (!ModelState.IsValid || string.IsNullOrEmpty(car.Model))
            {
                if (string.IsNullOrEmpty(car.Model))
                    ModelState.AddModelError("Model", "Tên xe không được để trống");

                return View(car);
            }

            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();
                var type = car.CarType?.ToLower();

                if (type == "electric")
                {
                    var cmd = new MySqlCommand(@"
                        INSERT INTO ElectricCar (battery, range_km, model, branch_id, price, imageUrl)
                        VALUES (@battery, @range, @model, @branch, @price, @url)", conn);
                    cmd.Parameters.AddWithValue("@battery", car.Battery ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@range", car.RangeKm ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@model", car.Model);
                    cmd.Parameters.AddWithValue("@branch", car.BranchId);
                    cmd.Parameters.AddWithValue("@price", car.Price);
                    cmd.Parameters.AddWithValue("@url", string.IsNullOrEmpty(car.ImageUrl) ? DBNull.Value : car.ImageUrl);
                    cmd.ExecuteNonQuery();
                }
                else if (type == "gas")
                {
                    var cmd = new MySqlCommand(@"
                        INSERT INTO GasCar (fuel_eff, model, branch_id, price, imageUrl)
                        VALUES (@eff, @model, @branch, @price, @url)", conn);
                    cmd.Parameters.AddWithValue("@eff", car.FuelEff ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@model", car.Model);
                    cmd.Parameters.AddWithValue("@branch", car.BranchId);
                    cmd.Parameters.AddWithValue("@price", car.Price);
                    cmd.Parameters.AddWithValue("@url", string.IsNullOrEmpty(car.ImageUrl) ? DBNull.Value : car.ImageUrl);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        // Lấy danh sách hãng
        private List<Branch> GetBranches()
        {
            var branches = new List<Branch>();
            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM Branch", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        branches.Add(new Branch
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Country = reader.GetString("country")
                        });
                    }
                }
            }
            return branches;
        }

        // Xe điện
        private List<ElectricCar> GetElectricCars(MySqlConnection conn)
        {
            var list = new List<ElectricCar>();
            var cmd = new MySqlCommand(@"
        SELECT ec.*, b.name AS branch_name
        FROM ElectricCar ec
        JOIN Branch b ON ec.branch_id = b.id", conn);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new ElectricCar
                    {
                        Id = reader.GetInt32("id"),
                        Model = reader.GetString("model"),
                        BranchId = reader.GetInt32("branch_id"),
                        BranchName = reader["branch_name"].ToString(),
                        Battery = reader.GetInt32("battery"),
                        RangeKm = reader.GetInt32("range_km"),
                        Price = reader.GetDecimal("price"),
                        ImageUrl = reader.IsDBNull("imageUrl") ? null : reader.GetString("imageUrl")
                    });
                }
            }
            return list;
        }


        // Xe xăng
        private List<GasCar> GetGasCars(MySqlConnection conn)
        {
            var list = new List<GasCar>();
            var cmd = new MySqlCommand(@"
        SELECT gc.*, b.name AS branch_name
        FROM GasCar gc
        JOIN Branch b ON gc.branch_id = b.id", conn);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new GasCar
                    {
                        Id = reader.GetInt32("id"),
                        Model = reader.GetString("model"),
                        BranchId = reader.GetInt32("branch_id"),
                        BranchName = reader["branch_name"].ToString(),
                        FuelEff = reader.GetFloat("fuel_eff"),
                        Price = reader.GetDecimal("price"),
                        ImageUrl = reader.IsDBNull("imageUrl") ? null : reader.GetString("imageUrl")
                    });
                }
            }
            return list;
        }

        // Giao diện tìm kiếm
        public IActionResult Search()
        {
            ViewBag.Branches = GetBranches(); // dùng cho dropdown
            return View(new CarSearchViewModel());
        }

        // Xử lý kết quả tìm kiếm
        [HttpGet]
        public IActionResult SearchResult(CarSearchViewModel search)
        {
            ViewBag.Branches = GetBranches();

            using var conn = new MySqlConnection(GetConnection());
            conn.Open();

            var model = new CarSearchViewModel
            {
                BranchId = search.BranchId,
                CarType = search.CarType,
                MinPrice = search.MinPrice,
                MaxPrice = search.MaxPrice,
                ElectricResults = new(),
                GasResults = new()
            };

            // Xe điện
            if (search.CarType == "Electric" || string.IsNullOrEmpty(search.CarType))
            {
                var query = "SELECT * FROM ElectricCar WHERE 1=1";
                var cmd = new MySqlCommand();
                cmd.Connection = conn;

                if (search.BranchId.HasValue)
                {
                    query += " AND branch_id = @branch";
                    cmd.Parameters.AddWithValue("@branch", search.BranchId);
                }
                if (search.MinPrice.HasValue)
                {
                    query += " AND price >= @min";
                    cmd.Parameters.AddWithValue("@min", search.MinPrice);
                }
                if (search.MaxPrice.HasValue)
                {
                    query += " AND price <= @max";
                    cmd.Parameters.AddWithValue("@max", search.MaxPrice);
                }

                cmd.CommandText = query;
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.ElectricResults.Add(new ElectricCar
                    {
                        Id = reader.GetInt32("id"),
                        Model = reader.GetString("model"),
                        BranchId = reader.GetInt32("branch_id"),
                        Battery = reader.GetInt32("battery"),
                        RangeKm = reader.GetInt32("range_km"),
                        Price = reader.GetDecimal("price"),
                        ImageUrl = reader.IsDBNull("imageUrl") ? null : reader.GetString("imageUrl")
                    });
                }
            }

            // Xe xăng
            if (search.CarType == "Gas" || string.IsNullOrEmpty(search.CarType))
            {
                var query = "SELECT * FROM GasCar WHERE 1=1";
                var cmd = new MySqlCommand();
                cmd.Connection = conn;

                if (search.BranchId.HasValue)
                {
                    query += " AND branch_id = @branch";
                    cmd.Parameters.AddWithValue("@branch", search.BranchId);
                }
                if (search.MinPrice.HasValue)
                {
                    query += " AND price >= @min";
                    cmd.Parameters.AddWithValue("@min", search.MinPrice);
                }
                if (search.MaxPrice.HasValue)
                {
                    query += " AND price <= @max";
                    cmd.Parameters.AddWithValue("@max", search.MaxPrice);
                }

                cmd.CommandText = query;
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.GasResults.Add(new GasCar
                    {
                        Id = reader.GetInt32("id"),
                        Model = reader.GetString("model"),
                        BranchId = reader.GetInt32("branch_id"),
                        FuelEff = reader.GetFloat("fuel_eff"),
                        Price = reader.GetDecimal("price"),
                        ImageUrl = reader.IsDBNull("imageUrl") ? null : reader.GetString("imageUrl")
                    });
                }
            }

            return View(model); // SearchResult.cshtml
        }


    }
}
