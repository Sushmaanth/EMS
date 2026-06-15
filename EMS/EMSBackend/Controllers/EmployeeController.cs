using Dtos;
using EMSBackend.Service.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EMSBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [Authorize(Roles = "Admin")]
        [Route("all")]
        [HttpGet]
        public IActionResult ViewEmployees()
        {
            var result = _employeeService.View();

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [Route("add")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            var result = await _employeeService.Create(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(CreateEmployee), result);
        }

        [Authorize(Roles = "Admin")]
        [Route("delete/{id}")]
        [HttpDelete]
        public IActionResult DeleteEmployee([FromRoute] int id)
        {
            var result = _employeeService.Delete(id);
            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [Route("update/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id, [FromBody] CreateEmployeeDto dto)
        {
            var result = await _employeeService.Update(id, dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [Route("employee/{id}")]
        [HttpGet]
        public IActionResult GetEmployeebyId([FromRoute] int id)
        {
            //throw new Exception("Database failure");
            var result = _employeeService.GetById(id);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result);
        }

        //[Route("employee/search")]
        //[HttpGet]
        //public IActionResult SearchEmployee([FromQuery] string? searchText)
        //{
        //        var found = employeeRepository.SearchEmployee(searchText);

        //        return Ok(found);
        //}
        [Authorize(Roles = "Admin")]
        [Route("employees")]
        [HttpGet]
        public IActionResult GetEmployees([FromQuery] string? searchText, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var result = _employeeService.GetEmployees(
                searchText,
                pageNumber,
                pageSize);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [Route("department/all")]
        [HttpGet]
        public IActionResult GetDepartments()
        {
            var result = _employeeService.GetDepartments();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [Route("roles/all")]
        [HttpGet]
        public IActionResult GetRoles()
        {
            var result = _employeeService.GetRoles();

            return Ok(result);
        }
        [Authorize(Roles = "Employee")]
        [RequestSizeLimit(62914560)]
        [RequestFormLimits(MultipartBodyLengthLimit = 62914560)]
        [Route("upload")]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] EmployeeDocumentUploadDto dto)
        {
            var result = await _employeeService.UploadDocumentAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            var result = _employeeService.GetByCategory(categoryId);

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("documentcategory")]
        public IActionResult GetAllCategory()
        {
            var result = _employeeService.GetAll();

            return Ok(result);
        }

        [Authorize(Roles = "Employee")]
        [HttpDelete("delete-document/{documentId}")]
        public async Task<IActionResult>DeleteDocument(int documentId)
        {
            var result =
                await _employeeService.DeleteDocumentAsync(documentId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Employee")]
        [HttpPut("edit-document")]
        public async Task<IActionResult> ReplaceDocument([FromForm] ReplaceDocumentDto dto)
        {
            var result = await _employeeService.ReplaceDocumentAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("view/{documentId}")]
        public async Task<IActionResult>ViewDocument(int documentId)
        {
            var result = await _employeeService.GetDocumentUrlAsync(documentId);

            return Ok(result);
        }

        [Authorize(Roles = "Employee")]
        [HttpGet("category/{categoryId}/employee/{employeeId}")]
        public async Task<IActionResult> GetDocumentTypesByCategory(int categoryId,int employeeId)
        {
            var result = await _employeeService.GetDocumentTypesByCategoryAsync(
                categoryId,
                employeeId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Employee")]
        [HttpGet("dashboard/{employeeId}")]
        public IActionResult GetDashboard(int employeeId)
        {
            var result = _employeeService.GetEmployeeDashboard(employeeId);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("upload-employees")]
        public async Task<IActionResult> UploadEmployeeData(IFormFile file)
        {
            var result = await _employeeService.UploadEmployeesAsync(file);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("download-template")]
        public IActionResult DownloadTemplate()
        {
            var result = _employeeService.DownloadTemplate();

            return File(result.Data,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "EmployeeUploadTemplate.xlsx");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("download-failed-records")]
        public IActionResult DownloadFailedRecords(List<UploadEmployeeExcelErrorDto> errors)
        {
            var result = _employeeService.DownloadFailedRecordsAsync(errors);

            return File(
                result.Data,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "FailedEmployeeRecords.xlsx");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("managers/all")]
        public IActionResult GetManagers()
        {
            var result = _employeeService.GetManagers();

            return Ok(result);
        }
    }
}
