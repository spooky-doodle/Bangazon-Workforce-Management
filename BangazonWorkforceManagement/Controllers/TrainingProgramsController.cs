using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;



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
            var active = true;
            var progs = await GetTrainingPrograms(active);
            return View(progs);
        }

        //  GET: Train
        public async Task<IActionResult> Archive()
        {
            var active = false;
            var progs = await GetTrainingPrograms(active);
            return View(progs);
        }

        // GET: TrainingPrograms/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT 
                                        tp.Id, tp.[Name], tp.StartDate, tp.EndDate, tp.MaxAttendees, e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor 
                                        FROM TrainingProgram AS tp
                                        LEFT JOIN EmployeeTraining AS et ON tp.Id = et.TrainingProgramID
                                        LEFT JOIN Employee AS e ON et.EmployeeId = e.Id
                                        WHERE tp.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;
                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        if (trainingProgram == null)
                        {
                            trainingProgram = new TrainingProgram()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))
                        {
                            employees.Add(new Employee()
                            {
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"))
                            });
                        }
                    }
                    trainingProgram.Attendees = employees;
                    return View(trainingProgram);
                }
            }
        }

        // GET: TrainingPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingProgram prog)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO TrainingProgram 
                            ([Name], StartDate, EndDate, MaxAttendees)
                            VALUES (@name, @startDate, @endDate, @maxAttendees)";

                        cmd.Parameters.AddWithValue("@name", prog.Name);
                        cmd.Parameters.AddWithValue("@startDate", prog.StartDate);
                        cmd.Parameters.AddWithValue("@endDate", prog.EndDate);
                        cmd.Parameters.AddWithValue("@maxAttendees", prog.MaxAttendees);

                        await cmd.ExecuteNonQueryAsync();

                    }

                }

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
        public async Task<IActionResult> Delete(int id)
        {
            var progToDelete = await GetOneTrainingProgram(id);

            if (progToDelete.IsCancelable) return View(progToDelete);
            else return RedirectToAction(nameof(Index));
        }

        // POST: TrainingPrograms/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var progToDelete = await GetOneTrainingProgram(id);

                if (progToDelete.IsCancelable == false) throw new Exception("Cannot delete events that have started");

                using (SqlConnection conn = Connection)
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        DELETE FROM EmployeeTraining WHERE TrainingProgramId = @id
                        DELETE FROM TrainingProgram WHERE Id = @id";
                        cmd.Parameters.AddWithValue("@id", id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private async Task<List<TrainingProgram>> GetTrainingPrograms(bool future = true)
        {
            var progs = new List<TrainingProgram>();
            using (SqlConnection conn = Connection)
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, [Name], StartDate, EndDate, MaxAttendees FROM TrainingProgram ";
                    cmd.CommandText += "WHERE StartDate ";
                    cmd.CommandText += future ? ">" : " <= ";
                    cmd.CommandText += " @compareDate";
                    cmd.Parameters.Add(new SqlParameter("@compareDate", SqlDbType.DateTime) { Value = DateTime.Today });

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

        private async Task<TrainingProgram> GetOneTrainingProgram(int id)
        {
            TrainingProgram prog = null;
            using (SqlConnection conn = Connection)
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                    Id, [Name], StartDate, EndDate, MaxAttendees 
                    FROM TrainingProgram 
                    WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    var reader = cmd.ExecuteReader();

                    if (await reader.ReadAsync())
                    {
                        prog = new TrainingProgram()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))

                        };
                    }
                    else throw new Exception("No program found");
                }
            }


            return prog;
        }


    }
}