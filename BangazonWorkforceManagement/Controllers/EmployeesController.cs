using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using BangazonWorkforceManagement.Models;
using BangazonWorkforceManagement.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

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
            var employee = GetOneEmployee(id);
            var departments = GetAllDepartments();
            var computers = GetSpecificComputers(id);
            var EmpComputers = GetSpecificComputers(id);

            var viewModel = new EmployeeEditViewModel();

            var DeptSelectItems = departments
                .Select(dept => new SelectListItem
                {
                    Text = dept.Name,
                    Value = dept.Id.ToString()
                })
                .ToList();

            DeptSelectItems.Insert(0, new SelectListItem
            {
                Text = "Choose Department...",
                Value = "0"
            });

            ////empComps

            viewModel.CompIds = new List<string>();

            var CompSelectItems = computers
                .Select(comp =>
                {

                    return new SelectListItem
                    {
                        Text = $"{comp.Make} {comp.Manufacturer}",
                        Value = comp.Id.ToString()
                    };
                })
                .ToList();


            var EmpCompSelectItems = EmpComputers
                .Where(compt =>
                {
                    if (compt.Employee != null)
                    {
                        return compt.Employee.Id == id;
                    } else {
                        return false;
                    };

                })
                .Select(comp =>
                {
                    viewModel.CompIds.Add(comp.Id.ToString());
                    return new SelectListItem
                    {
                        Text = $"{comp.Make} {comp.Manufacturer}",
                        Value = comp.Id.ToString()
                    };
                })
                .ToList();


            viewModel.Employee = employee;
            viewModel.Departments = DeptSelectItems;
            viewModel.Computers = CompSelectItems;
            viewModel.EmpComps = EmpCompSelectItems;
            viewModel.Comps = new MultiSelectList(viewModel.Computers, "Value", "Text", viewModel.EmpComps);
            return View(viewModel);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel emp)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                            UPDATE Employee
                                            SET 
                                                FirstName = @firstName,
                                                LastName = @lastName,
                                                DepartmentId = @departmentId,
                                                IsSupervisor = @isSupervisor
                                            WHERE Id = @id
                                            ";

                        //cmd.Parameters.Add(new SqlParameter("@id", id));
                        //cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        //cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        //cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        //cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                        //cmd.ExecuteNonQuery();
                    }
                }
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

        // GET: Employees/Assign/5
        public async Task<IActionResult> Assign(int id)
        {
            var viewModel = new AssignEmployeeTrainingViewModel();
            viewModel.TrainingOptions = CreateTrainingSelections(await GetAvailableTrainingPrograms(id));
            viewModel.EmployeeId = id;
            if (viewModel.TrainingOptions.Count > 0)
            {
                return View(viewModel);
            }
            else return RedirectToAction(nameof(Details), new { id = id });
        }

        // POST: Employees/Assign/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id, EmployeeTraining assign)
        {
            using (SqlConnection conn = Connection)
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO EmployeeTraining 
                        (EmployeeId, TrainingProgramId)
                        VALUES (@employeeId, @trainingProgramId)";
                    cmd.Parameters.AddWithValue("@employeeId", id);
                    cmd.Parameters.AddWithValue("@trainingProgramId", assign.TrainingProgramId);



                    await cmd.ExecuteNonQueryAsync();
                }
            }


            return RedirectToAction(nameof(Details), new { id = id });
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
                            catch (SqlNullValueException) { }

                            var idToCompare = computer.Id;
                            if (employee.Computers.Any(tr => tr.Id == idToCompare))
                            {

                            }
                            else
                            {
                                employee.Computers.Add(computer);
                            }


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

                            }
                            else
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

        private async Task<List<TrainingProgram>> GetEmployeeTrainingPrograms(int id)
        {
            List<TrainingProgram> progs = new List<TrainingProgram>();

            using (SqlConnection conn = Connection)
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id AS TrainingId,
                                               tp.Name AS TrainingName,
                                               tp.StartDate,
                                               tp.EndDate,
                                               tp.MaxAttendees
                                        FROM EmployeeTraining et
                                        LEFT JOIN TrainingProgram tp
                                        ON tp.Id = et.TrainingProgramId
                                        WHERE et.EmployeeId = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        progs.Add(ParseTrainingProgram(reader));
                    }
                }
            }

            return progs;
        }

        private async Task<List<TrainingProgram>> GetAvailableTrainingPrograms(int id)
        {
            List<TrainingProgram> allPrograms = null;
            List<TrainingProgram> employeePrograms = null;

            List<Task> tasks = new List<Task>()
            {
                Task.Run(async () => allPrograms = await GetAllTrainingPrograms()),
                Task.Run(async () => employeePrograms = await GetEmployeeTrainingPrograms(id))
            };
            await Task.WhenAll(tasks);
            List<TrainingProgram> availablePrograms = allPrograms.Where(prog =>
            {
                return employeePrograms.Find(empProg => empProg.Id == prog.Id) == null;
            }).ToList();

            return availablePrograms;


        }

        private List<SelectListItem> CreateTrainingSelections(List<TrainingProgram> progs)
        {
            return progs.Select(prog => new SelectListItem()
            {
                Text = prog.Name,
                Value = prog.Id.ToString()
            }).ToList();

        }

        private async Task<List<TrainingProgram>> GetAllTrainingPrograms()
        {
            List<TrainingProgram> progs = new List<TrainingProgram>();

            using (SqlConnection conn = Connection)
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id AS TrainingId,
                                               tp.Name AS TrainingName,
                                               tp.StartDate,
                                               tp.EndDate,
                                               tp.MaxAttendees
                                        FROM TrainingProgram tp";
                    var reader = cmd.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        progs.Add(ParseTrainingProgram(reader));
                    }
                }
            }

            return progs;
        }

        private TrainingProgram ParseTrainingProgram(SqlDataReader reader)
        {
            return new TrainingProgram()
            {
                Id = reader.GetInt32(reader.GetOrdinal("TrainingId")),
                Name = reader.GetString(reader.GetOrdinal("TrainingName")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
            };
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

        public List<Computer> GetAllComputersForOneEmployee(int id)
        {
            var computers = new List<Computer>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                            c.Id, 
                                            c.PurchaseDate, 
                                            c.DecommissionDate, 
                                            c.Make, 
                                            c.Manufacturer, 
                                            ce.Id AS CEId, 
                                            ce.EmployeeId, 
                                            ce.AssignDate, 
                                            ce.UnassignDate, 
                                            ce.ComputerId
                                        FROM Computer c
                                        LEFT JOIN ComputerEmployee ce
                                        ON ce.ComputerId = c.Id
                                        WHERE ce.EmployeeId = @id
                                        AND c.DecommissionDate IS NULL
                                        AND ce.UnassignDate IS NULL
                                      ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        computers.Add(new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                        });
                    }
                    reader.Close();
                }
            }
            return computers;
        }
        public List<Computer> GetSpecificComputers(int id)
        {
            var computers = new List<Computer>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT DISTINCT c.Id AS ComputerId, 
                        c.PurchaseDate,
                        c.DecommissionDate,
                        c.Make,
                        c.Manufacturer,
                        ce.EmployeeId
                        FROM Computer c
                        LEFT JOIN ComputerEmployee ce
                        ON ce.ComputerId = c.Id
                        LEFT JOIN Employee e
                        ON e.Id = ce.EmployeeId
                        WHERE c.DecommissionDate IS NULL
                        AND ce.EmployeeId = @id
                        OR ce.EmployeeId IS NULL
                                
                    ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        Computer newComputer = new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };

                        try
                        {
                            newComputer.DecommissionDate = reader.GetDateTime(reader.GetOrdinal("DecommissionDate"));
                        }
                        catch (SqlNullValueException)
                        { };  //  DecommissionDate defaults to null.
                        try
                        {

                            Employee employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId"))
                            };
                            newComputer.Employee = employee;

                        }
                        catch (Exception ex)
                        { }

                        computers.Add(newComputer);
                    }
                    reader.Close();
                }
            }
            return computers;
        }
    }
}