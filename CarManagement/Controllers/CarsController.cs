using Microsoft.AspNetCore.Mvc;
using CarManagement.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace CarManagement.Controllers
{
    public class CarsController : Controller
    {
        private readonly IConfiguration _configuration;

        public CarsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnection() =>
            _configuration.GetConnectionString("DefaultConnection");

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

                var cmd1 = new MySqlCommand("SELECT * FROM ElectricCar", conn);
                using (var reader = cmd1.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.ElectricCars.Add(new ElectricCar
                        {
                            Id = reader.GetInt32("id"),
                            Battery = reader.GetInt32("battery"),
                            RangeKm = reader.GetInt32("range_km"),
                            Model = reader.GetString("model"),
                            BranchId = reader.GetInt32("branch_id"),
                            Price = reader.GetDecimal("price"),
                            ImageUrl = reader.IsDBNull("imageUrl") ? null : reader.GetString("imageUrl")
                        });
                    }
                }

                var cmd2 = new MySqlCommand("SELECT * FROM GasCar", conn);
                using (var reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.GasCars.Add(new GasCar
                        {
                            Id = reader.GetInt32("id"),
                            FuelEff = reader.GetFloat("fuel_eff"),
                            Model = reader.GetString("model"),
                            BranchId = reader.GetInt32("branch_id"),
                            Price = reader.GetDecimal("price"),
                            ImageUrl = reader.IsDBNull("imageUrl") ? null : reader.GetString("imageUrl")
                        });
                    }
                }
            }

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new Cars());
        }

        [HttpPost]
        public IActionResult Create(Cars model)
        {
            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();

                if (model.CarType == "Electric")
                {
                    var cmd = new MySqlCommand(@"
                        INSERT INTO ElectricCar (battery, range_km, model, branch_id, price, imageUrl)
                        VALUES (@battery, @range, @model, @branch, @price, @url)", conn);
                    cmd.Parameters.AddWithValue("@battery", model.Battery);
                    cmd.Parameters.AddWithValue("@range", model.RangeKm);
                    cmd.Parameters.AddWithValue("@model", model.Model);
                    cmd.Parameters.AddWithValue("@branch", model.BranchId);
                    cmd.Parameters.AddWithValue("@price", model.Price);
                    cmd.Parameters.AddWithValue("@url", model.ImageUrl ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
                else if (model.CarType == "Gas")
                {
                    var cmd = new MySqlCommand(@"
                        INSERT INTO GasCar (fuel_eff, model, branch_id, price, imageUrl)
                        VALUES (@eff, @model, @branch, @price, @url)", conn);
                    cmd.Parameters.AddWithValue("@eff", model.FuelEff);
                    cmd.Parameters.AddWithValue("@model", model.Model);
                    cmd.Parameters.AddWithValue("@branch", model.BranchId);
                    cmd.Parameters.AddWithValue("@price", model.Price);
                    cmd.Parameters.AddWithValue("@url", model.ImageUrl ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }
    }
}
