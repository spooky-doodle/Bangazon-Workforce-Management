using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BangazonWorkforceManagement.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BangazonWorkforceManagement.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Employee
        public ActionResult Index()
        {
            var employees = GetAllEmployees();
            return View(employees);
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            var employee = GetOneEmployee(id);
            return View(employee);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            var viewModel = new EmployeeCreateViewModel();
            var departments = GetAllDepartments();

            var selectItems = departments
                .Select(dept => new SelectListItem
                {
                    Text = dept.Name,
                    Value = dept.Id.ToString()
                })
                .ToList();

            selectItems.Insert(0, new SelectListItem
            {
                Text = "Choose Department...",
                Value = "0"
            });
            viewModel.Departments = selectItems;
            return View(viewModel);
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                        INSERT INTO Employee (
                                            FirstName, 
                                            LastName, 
                                            DepartmentId, 
                                            IsSupervisor
                                        )
                                        VALUES (
                                            @firstName,
                                            @lastName,
                                            @departmentId,
                                            @isSupervisor
                                        )";

                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employee/Edit/5
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

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employee/Delete/5
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

        private Employee GetOneEmployee(int id)
        {
            Employee employee = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, 
                                               e.FirstName, 
                                               e.LastName, 
                                               e.DepartmentId,
                                               e.IsSupervisor,
                                               d.Name,
                                               c.Make, 
                                               c.Manufacturer, 
                                               c.PurchaseDate, 
                                               c.DecommissionDate,
                                               tp.Id AS TrainingId,
                                               tp.Name AS TrainingName,
                                               tp.StartDate,
                                               tp.EndDate,
                                               tp.MaxAttendees
                                        FROM Employee e
                                        LEFT JOIN Department d
                                        on d.Id = e.DepartmentId
                                        LEFT JOIN ComputerEmployee ce
                                        ON ce.EmployeeId = e.Id
                                        LEFT JOIN Computer c
                                        ON ce.ComputerId = c.Id
                                        LEFT JOIN EmployeeTraining et
                                        ON et.EmployeeId = e.Id
                                        LEFT JOIN TrainingProgram tp
                                        ON tp.Id = et.TrainingProgramId
                                        WHERE ce.UnassignDate IS NULL
                                        AND e.Id = @id
                                      ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (employee == null)
                        {
                            employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                                Department =
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                }

                            };
                        }

                        try
                        {
                            Computer computer = new Computer()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))
                            };
                            try
                            {
                                computer.DecommissionDate = reader.GetDateTime(reader.GetOrdinal("DecommissionDate"));
                            }
                            catch (SqlNullValueException){}
                            employee.Computers.Add(computer);

                        }
                        catch (SqlNullValueException)
                        {
                            // 
                        }
                        try
                        {
                           
                            TrainingProgram training = new TrainingProgram()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TrainingId")),
                                Name = reader.GetString(reader.GetOrdinal("TrainingName")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                            };

                            var idToCheck = training.Id;

                            if (employee.Trainings.Any(tr => tr.Id == idToCheck))
                            {
                                
                            } else
                            {
                                employee.Trainings.Add(training);
                            }
                        }
                        catch (SqlNullValueException) { }
                    }
                    reader.Close();
                }
            }
            return employee;
        }

        private List<Employee> GetAllEmployees()
        {
            var employees = new List<Employee>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor, d.Name
                                        FROM Employee e
                                        LEFT JOIN Department d
                                        on d.Id = e.DepartmentId
                                      ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        employees.Add(new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            Department =
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }
                        });
                    }
                    reader.Close();
                }
            }
            return employees;
        }
        public List<Department> GetAllDepartments()
        {
            var departments = new List<Department>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id, d.Name, d.Budget FROM Department AS d ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        departments.Add(new Department()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        });
                    }
                    reader.Close();
                }
            }
            return departments;
        }
    }
}