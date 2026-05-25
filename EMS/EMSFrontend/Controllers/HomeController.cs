using EMSFrontend.Api.Abstraction;
using EMSFrontend.Models;
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

        private async Task LoadDepartmentsAsync(int? selectedDepartmentId = null)
        {
            var departments =
                await request.SendGetDepartmentsAsync();

            ViewBag.Departments =
                new SelectList(
                    departments,
                    "Id",
                    "DepartmentName",
                    selectedDepartmentId);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchText, int pageNumber = 1, int pageSize = 7)
        {
            string token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            //var employees  =  await request.SendViewAllEmployeeRequestAsync();
            var employees = await request.SendGetEmployeesAsync(searchText, pageNumber, pageSize);
            ViewBag.SearchText = searchText;
            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string searchText, int pageNumber = 1, int pageSize = 7)
        {

            var employees = await request.SendGetEmployeesAsync(searchText, pageNumber, pageSize);
            return PartialView("_EmployeeTable", employees);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDepartmentsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync();

                return View(model);
            }

            var createEmployee = await request.SendCreateEmployeeRequestAsync(model);

            TempData["SuccessfullyCreatedEmployee"] = "Employee Added Successfully";
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            await request.SendDeleteEmployeeRequestAsync(id);
            TempData["DeletedEmployee"] = "Employee Deleted Successfully";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await request.SendGetAEmployeeRequestAsync(id);

            await LoadDepartmentsAsync(employee.DepartmentId);

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync(model.DepartmentId);

                return View(model);
            }


            var updateEmployee = await request.SendUpdateEmployeeRequestAsync(id, model);
            TempData["employeeUpdateSuccessully"] = "Employee Updated Successfully";
            return RedirectToAction("Index", "Home");
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
    }
}
