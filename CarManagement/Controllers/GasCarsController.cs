using Microsoft.AspNetCore.Mvc;
using CarManagement.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace CarManagement.Controllers
{
    public class GasCarsController : Controller
    {
        private readonly IConfiguration _configuration;

        public GasCarsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string? GetConnection() =>
            _configuration.GetConnectionString("DefaultConnection");

        public IActionResult Index()
        {
            var cars = new List<GasCar>();
            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM GasCar", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cars.Add(new GasCar
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
            return View(cars);
        }

        public IActionResult Edit(int id)
        {
            GasCar car = null;
            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM GasCar WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    car = new GasCar
                    {
                        Id = reader.GetInt32("id"),
                        FuelEff = reader.GetFloat("fuel_eff"),
                        Model = reader.GetString("model"),
                        BranchId = reader.GetInt32("branch_id"),
                        Price = reader.GetDecimal("price"),
                        ImageUrl = reader.IsDBNull("imageUrl") ? null : reader.GetString("imageUrl")
                    };
                }
            }
            return car == null ? NotFound() : View(car);
        }

        [HttpPost]
        public IActionResult Edit(GasCar car)
        {
            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"
                    UPDATE GasCar 
                    SET fuel_eff = @eff, model = @model, branch_id = @branch, 
                        price = @price, imageUrl = @url 
                    WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@eff", car.FuelEff);
                cmd.Parameters.AddWithValue("@model", car.Model);
                cmd.Parameters.AddWithValue("@branch", car.BranchId);
                cmd.Parameters.AddWithValue("@price", car.Price);
                cmd.Parameters.AddWithValue("@url", car.ImageUrl ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@id", car.Id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            GasCar car = null;
            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM GasCar WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    car = new GasCar
                    {
                        Id = reader.GetInt32("id"),
                        Model = reader.GetString("model")
                    };
                }
            }
            return car == null ? NotFound() : View(car);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM GasCar WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}
