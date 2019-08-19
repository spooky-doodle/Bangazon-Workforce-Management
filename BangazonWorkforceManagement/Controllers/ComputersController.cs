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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace BangazonWorkforceManagement.Controllers
{
    public class ComputersController : Controller
    {
        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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
        // GET: Computers
        public ActionResult Index()
        {

            var computers = new List<Computer>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT c.Id, 
                        c.PurchaseDate,
                        c.DecommissionDate,
                        c.Make,
                        c.Manufacturer,
                        e.FirstName,
                        e.LastName
                        FROM Computer c
                        LEFT JOIN ComputerEmployee ce
                        ON c.Id = ce.ComputerId
                        LEFT JOIN Employee e
                        ON ce.EmployeeId = e.Id
                        WHERE ce.UnassignDate IS null
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        Computer newComputer = new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };

                        try
                        {
                            newComputer.DecommissionDate = reader.GetDateTime(reader.GetOrdinal("DecommissionDate"));
                        }
                        catch (SqlNullValueException)
                        { }  //  DecommissionDate defaults to null

                        try
                        {
                            newComputer.Employee = new Employee()
                            {
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };
                        }
                        catch (SqlNullValueException)
                        { }  //  DecommissionDate defaults to null

                        computers.Add(newComputer);
                    }
                }
            }
            return View(computers);
        }
        // GET: Computers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string userInput)
        {
            var userInputNotEmpty = !String.IsNullOrEmpty(userInput);
            

            var computers = new List<Computer>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (userInputNotEmpty)
                    {
                        cmd.CommandText = @"
                        SELECT c.Id, 
                        c.PurchaseDate,
                        c.DecommissionDate,
                        c.Make,
                        c.Manufacturer,
                        e.FirstName,
                        e.LastName
                        FROM Computer c
                        LEFT JOIN ComputerEmployee ce
                        ON c.Id = ce.ComputerId
                        LEFT JOIN Employee e
                        ON ce.EmployeeId = e.Id
                        WHERE ce.UnassignDate IS null
                        AND ((c.Make LIKE '%' + @searchString + '%') OR (c.Manufacturer LIKE '%' + @searchString + '%'))
                    ";
                        cmd.Parameters.Add(new SqlParameter("@searchString", userInput));
                    }
                    else {
                        cmd.CommandText = @"
                        SELECT c.Id, 
                        c.PurchaseDate,
                        c.DecommissionDate,
                        c.Make,
                        c.Manufacturer,
                        e.FirstName,
                        e.LastName
                        FROM Computer c
                        LEFT JOIN ComputerEmployee ce
                        ON c.Id = ce.ComputerId
                        LEFT JOIN Employee e
                        ON ce.EmployeeId = e.Id
                        WHERE ce.UnassignDate IS null
                    ";
                    }

                    
                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        Computer newComputer = new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };

                        try
                        {
                            newComputer.DecommissionDate = reader.GetDateTime(reader.GetOrdinal("DecommissionDate"));
                        }
                        catch (SqlNullValueException)
                        { }  //  DecommissionDate defaults to null

                        try
                        {
                            newComputer.Employee = new Employee()
                            {
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };
                        }
                        catch (SqlNullValueException)
                        { }  //  DecommissionDate defaults to null

                        computers.Add(newComputer);
                    }
                }
            }
            return View(computers);
        }




        // GET: Computers/Details/5
        public ActionResult Details(int id)
        {
            Computer computer = GetComputerById(id);

            return View(computer);
        }

        private Computer GetComputerById(int id)
        {
            Computer computer = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, 
                        PurchaseDate,
                        DecommissionDate,
                        Make,
                        Manufacturer
                        FROM Computer
                        Where Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        computer = new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };

                        try
                        {
                            computer.DecommissionDate = reader.GetDateTime(reader.GetOrdinal("DecommissionDate"));
                        }
                        catch (SqlNullValueException)
                        { }  //  DecommissionDate defaults to null.
                    }


                }
            }

            return computer;
        }

        // GET: Computers/Create
        public ActionResult Create()
        {
            var viewModel = new ComputerEmployeeViewModel();
            var employees = GetEmployees();
           List<SelectListItem> selectItem = employees
                .Select(employee => new SelectListItem
                {
                    Text = $"{employee.FirstName} {employee.LastName}",
                    Value = employee.Id.ToString()
                })
                .ToList();
 

            selectItem.Insert(0, new SelectListItem
            {
                Text = "Choose Employee...",
                Value = "0"
            });
            viewModel.Employees = selectItem;

            return View(viewModel);
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComputerEmployeeViewModel computerEmployees)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    int computerId = 0;

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                    INSERT INTO Computer(
                        Make,
                        Manufacturer,
                        PurchaseDate
                        )
                        OUTPUT INSERTED.Id
                        Values(   
                        @Make, 
                        @Manufacturer,
                        @PurchaseDate)";

                        cmd.Parameters.AddWithValue("@Make", computerEmployees.Computer.Make);
                        cmd.Parameters.AddWithValue("@Manufacturer", computerEmployees.Computer.Manufacturer);
                        cmd.Parameters.AddWithValue("@PurchaseDate", computerEmployees.Computer.PurchaseDate);
                        cmd.Parameters.AddWithValue("@ComputerId", computerEmployees.Computer.Id);
                        computerId = (Int32)cmd.ExecuteScalar();

                    }
                    if(computerEmployees.EmployeeId > 0)
                    { using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO ComputerEmployee(
                           ComputerId,
                           EmployeeId,
                           AssignDate
                           ) Values(
                           @ComputerId,
                           @EmployeeId,
                           @AssignDate
                           )";
                        cmd.Parameters.AddWithValue("@EmployeeId", computerEmployees.EmployeeId);
                        cmd.Parameters.AddWithValue("@ComputerId", computerId);
                        cmd.Parameters.Add(new SqlParameter("@AssignDate", SqlDbType.DateTime) { Value = DateTime.Today });

                        cmd.ExecuteNonQuery();
                    } }

                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {
            return View(GetComputerById(id));
        }

        // POST: Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Computer computer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        DELETE Computer                      
                        WHERE Id = @id
                        ";

                        cmd.Parameters.AddWithValue("@id", id);

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
        private List<Employee> GetEmployees()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName FROM Employee";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        });
                    }

                    reader.Close();

                    return employees;
                }
            }
        }
    }
}

   