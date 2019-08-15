using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

//Given a user wants to view all training programs
//When the user clicks the Training Programs item in the navigation bar
//Then the user will see a list of all training programs that have not taken place yet

//Given the user is viewing all training programs
//When the user clicks the Create New link
//Then the user should be presented with a form in which the following information 
//can be entered

//Name
//Title
//Start day
//End day
//Maximum number of attendees

namespace BangazonWorkforceManagement.Controllers
{
    public class TrainingProgramsController : Controller
    {
        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: TrainingPrograms
        public async Task<IActionResult> Index()
        {
            var progs = await GetTrainingPrograms();
            return View(progs);
        }

        // GET: TrainingPrograms/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TrainingPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingPrograms/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TrainingPrograms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingPrograms/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainingPrograms/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private async Task<List<TrainingProgram>> GetTrainingPrograms(string filterText = "")
        {
            var progs = new List<TrainingProgram>();
            using (SqlConnection conn = Connection)
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, [Name], StartDate, EndDate, MaxAttendees FROM TrainingProgram ";
                    cmd.CommandText += filterText;  

                    var reader = cmd.ExecuteReader();

                    while (await reader.ReadAsync())
                    {
                        progs.Add(new TrainingProgram()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))

                        });
                    }
                }
            }


            return progs;
        }
    }
}