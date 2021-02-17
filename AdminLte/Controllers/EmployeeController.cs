using AdminLte.Data;
using AdminLte.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly PostgreDbContext _db;
        public EmployeeController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("employee/table-data-view")]
        public async Task<IActionResult> GetAllEmployee()
        {
            try
            {
                var data = await _db.Employees.ToListAsync();
                ViewData["EmployeesList"] = data;
                return PartialView("~/Views/Employee/_TableData.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("employee")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("employee/add")]
        public IActionResult AddEmployeeForm()
        {
            try
            {
                ViewData["Title"] = "Add New Employee";
                ViewData["Employee"] = new Employee();
                return PartialView("~/Views/Employee/_AddForm.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        [HttpPost("employee/save")]
        public async Task<IActionResult> SaveEmployee(Employee employee)
        {
            try
            {
                Employee employeeFromDb = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);

                if (employeeFromDb == null)
                {
                    var group = _db.EmployeeGroup.First();
                    if (group == null)
                    {
                        group = new EmployeeGroup()
                        {
                            GroupID = 1,
                            Name = "Test"
                        };
                    }
                    employee.Group = group;
                    _db.Employees.Add(employee);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Created Successfully" });
                }
                else
                {
                    employeeFromDb.FirstName = employee.FirstName;
                    employeeFromDb.LastName = employee.LastName;
                    employeeFromDb.Position = employee.Position;
                    employeeFromDb.Department = employee.Department;
                    employeeFromDb.Salary = employee.Salary;
                    employeeFromDb.JobTitle = employee.JobTitle;
                    employeeFromDb.DateJoined = employee.DateJoined;
                    employeeFromDb.LastUpdated = employee.LastUpdated;
                    employeeFromDb.EmployeeId = employee.EmployeeId;
                    _db.Employees.Update(employeeFromDb);
                    _db.SaveChanges();
                    return Json(new { success = true, message = "Updated Successfully" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Error while saving" });
            }
        }

        [HttpPost("employee/delete")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employeeFromDb = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);

                if (employeeFromDb == null)
                {
                    return Json(new { success = false, message = "Error while deleting." });
                }

                _db.Employees.Remove(employeeFromDb);
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Delete Successfull!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Error while deleting." });
            }
        }

        [HttpGet("employee/edit")]
        public async Task<IActionResult> EditEmployee(int id)
        {
            try
            {
                Employee employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);
                if (employee == null)
                {
                    return NotFound();
                }

                ViewData["Title"] = "Edit Employee: " + employee.FirstName + " " + employee.LastName;
                ViewData["Employee"] = employee;
                return PartialView("~/Views/Employee/_AddForm.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Error while deleting." });
            }
        }

        [HttpGet("employee/search")]
        public async Task<IActionResult> SearchEmployee(string keyword)
        {

            try
            {
                var employees = await _db.Employees.Where(e =>
                e.LastName.ToLower().Contains(keyword.ToLower()) ||
                e.FirstName.ToLower().Contains(keyword.ToLower()) ||
                e.Position.ToLower().Contains(keyword.ToLower())
                ).ToListAsync();

                ViewData["EmployeesList"] = employees;
                return PartialView("~/Views/Employee/_TableData.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
