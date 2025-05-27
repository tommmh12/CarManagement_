using Microsoft.AspNetCore.Mvc;
using CarManagement.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace CarManagement.Controllers
{
    public class BranchesController : Controller
    {
        private readonly IConfiguration _configuration;

        public BranchesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnection() =>
            _configuration.GetConnectionString("DefaultConnection");

        // GET: Branches
        public IActionResult Index()
        {
            List<Branch> branches = new List<Branch>();
            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();
                string query = "SELECT * FROM Branch";
                MySqlCommand cmd = new MySqlCommand(query, conn);
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
            return View(branches);
        }

        // GET: Branches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Branches/Create
        [HttpPost]
        public IActionResult Create(Branch model)
        {
            if (ModelState.IsValid)
            {
                using (var conn = new MySqlConnection(GetConnection()))
                {
                    conn.Open();
                    string query = "INSERT INTO Branch (name, country) VALUES (@name, @country)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", model.Name);
                    cmd.Parameters.AddWithValue("@country", model.Country);
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Branches/Edit/5
        public IActionResult Edit(int id)
        {
            Branch? branch = null;
            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();
                string query = "SELECT * FROM Branch WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        branch = new Branch
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Country = reader.GetString("country")
                        };
                    }
                }
            }
            return branch == null ? NotFound() : View(branch);
        }

        // POST: Branches/Edit/5
        [HttpPost]
        public IActionResult Edit(Branch model)
        {
            if (ModelState.IsValid)
            {
                using (var conn = new MySqlConnection(GetConnection()))
                {
                    conn.Open();
                    string query = "UPDATE Branch SET name = @name, country = @country WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", model.Name);
                    cmd.Parameters.AddWithValue("@country", model.Country);
                    cmd.Parameters.AddWithValue("@id", model.Id);
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Branches/Delete/5
        public IActionResult Delete(int id)
        {
            Branch branch = null;
            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();
                string query = "SELECT * FROM Branch WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        branch = new Branch
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Country = reader.GetString("country")
                        };
                    }
                }
            }
            return branch == null ? NotFound() : View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var conn = new MySqlConnection(GetConnection()))
            {
                conn.Open();
                string query = "DELETE FROM Branch WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}
