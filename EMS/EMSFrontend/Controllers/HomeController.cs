using Dtos.Validation;
using EMSFrontend.Api.Abstraction;
using EMSFrontend.Models;
using Entities;
using Entities.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMSFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRequest request;
        //private readonly EmployeeValidator validator;

        public HomeController(IRequest request)
        {
            this.request = request;
            //this.validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchText,int pageNumber=1, int pageSize = 7)
        {
            try
            {   
                //var employees  =  await request.SendViewAllEmployeeRequestAsync();
                var employees = await request.SendGetEmployeesAsync(searchText,pageNumber,pageSize);
                ViewBag.SearchText = searchText;
                return View(employees);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string searchText, int pageNumber = 1,int pageSize = 7)
        {
            try
            {
                var employees = await request.SendGetEmployeesAsync(searchText, pageNumber, pageSize);
                return PartialView("_EmployeeTable", employees);

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var employee =
                    await request.SendGetAEmployeeRequestAsync(id);

                if (employee == null)
                {
                    TempData["ErrorMessage"] =
                        "Employee not found";

                    return RedirectToAction("Index");
                }

                return View(employee);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;

                return RedirectToAction(
                    "Error",
                    "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var departments =
                    await request.SendGetDepartmentsAsync();

                ViewBag.Departments =
                    new SelectList(
                        departments,
                        "Id",
                        "DepartmentName");

                return View();
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;

                return RedirectToAction(
                    "Error",
                    "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                

                var createEmployee = await request.SendCreateEmployeeRequestAsync(model);
               
                TempData["SuccessfullyCreatedEmployee"] = "Employee Added Successfully";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {

                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await request.SendDeleteEmployeeRequestAsync(id);
                TempData["DeletedEmployee"]= "Employee Deleted Successfully";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var employee = await request.SendGetAEmployeeRequestAsync(id);

                var departments = await request.SendGetDepartmentsAsync();

                ViewBag.Departments =
            new SelectList(
                departments,
                "Id",
                "DepartmentName",
                employee.DepartmentId);

                return View(employee);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel model)
        {
            try
            {
                var updateEmployee = await request.SendUpdateEmployeeRequestAsync(id, model);
                TempData["employeeUpdateSuccessully"] = "Employee Updated Successfully";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        /*[HttpGet]
        public async Task<IActionResult> Search(string searchText)
        {
            try
            {
                var searchResult = await request.SendSearchEmployeeRequestAsync(searchText);
                return View("Index", searchResult);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }*/

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
    }
}
