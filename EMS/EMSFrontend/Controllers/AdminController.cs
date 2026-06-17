using Dtos;
using EMSFrontend.Api.Abstraction;
using EMSFrontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace EMSFrontend.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRequest _request;
        //private readonly EmployeeValidator validator;

        public AdminController(IRequest request)
        {
            this._request = request;
            //this.validator = validator;
        }

        private async Task LoadDepartmentsAsync(int? selectedDepartmentId = null)
        {
            var departments =
                await _request.SendGetDepartmentsAsync();

            ViewBag.Departments =
                new SelectList(
                    departments,
                    "Id",
                    "DepartmentName",
                    selectedDepartmentId);
        }

        private async Task LoadRolesAsync(int? selectedRoleId = null)
        {
            var roles = await _request.SendGetRolesAsync();

            ViewBag.Roles = new SelectList(roles, "Id", "RoleName", selectedRoleId);
        }

        private async Task LoadManagersAsync(int? selectedManagerId = null)
        {
            var managers = await _request.SendGetManagersAsync();

            ViewBag.Managers = new SelectList(managers, "Id", "ManagerName", selectedManagerId);
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
            var employees = await _request.SendGetEmployeesAsync(searchText, pageNumber, pageSize);
            ViewBag.SearchText = searchText;
            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string searchText, int pageNumber = 1, int pageSize = 7)
        {

            var employees = await _request.SendGetEmployeesAsync(searchText, pageNumber, pageSize);
            return PartialView("_EmployeeTable", employees);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

            var employee =
                await _request.SendGetAEmployeeRequestAsync(id);

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
            await LoadRolesAsync();     
            await LoadManagersAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync();
                await LoadRolesAsync();      
                await LoadManagersAsync();
                return View(model);
            }

            var createEmployee = await _request.SendCreateEmployeeRequestAsync(model);

            TempData["SuccessfullyCreatedEmployee"] = "Employee Added Successfully";
            return RedirectToAction("Index", "Admin");

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            await _request.SendDeleteEmployeeRequestAsync(id);
            TempData["DeletedEmployee"] = "Employee Deleted Successfully";
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _request.SendGetAEmployeeRequestAsync(id);

            await LoadDepartmentsAsync(employee.DepartmentId);
            await LoadRolesAsync(employee.RoleId);
            await LoadManagersAsync(employee.ManagerId);

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync(model.DepartmentId); 
                await LoadRolesAsync(model.RoleId);
                await LoadManagersAsync(model.ManagerId);

                return View(model);
            }


            var updateEmployee = await _request.SendUpdateEmployeeRequestAsync(id, model);
            TempData["employeeUpdateSuccessully"] = "Employee Updated Successfully";
            return RedirectToAction("Index", "Admin");
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
                return RedirectToAction("Error", "Admin");
            }
        }*/

        [HttpGet]
        public IActionResult BulkUpload()
        {
            return View(new EmployeeBulkUploadViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> BulkUpload(EmployeeBulkUploadViewModel model)
        {
            if (model.File == null)
            {
                ModelState.AddModelError("File","Please select a file.");

                return View(model);
            }

            var result = await _request.UploadEmployeesAsync(model.File);

            model.Result = result.Data;

            TempData["Message"] = result.Message;
            TempData["Success"] = result.Success;

            if (result.Data?.Errors?.Any() == true)
            {
                HttpContext.Session.SetString("FailedEmployeeRecords", JsonSerializer.Serialize(result.Data.Errors));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFailedRecords()
        {
            var json = HttpContext.Session.GetString("FailedEmployeeRecords");

            if (string.IsNullOrEmpty(json))
            {
                TempData["Error"] = "No failed records found.";

                return RedirectToAction(nameof(BulkUpload));
            }

            var errors =JsonSerializer.Deserialize<List<UploadEmployeeExcelErrorDto>>(json);

            var fileBytes =
                await _request.DownloadFailedRecordsAsync(errors);

            return File(fileBytes,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","FailedEmployeeRecords.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadTemplate()
        {
            var fileBytes =await _request.DownloadTemplateAsync();

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "EmployeeUploadTemplate.xlsx");
        }
    }
}
