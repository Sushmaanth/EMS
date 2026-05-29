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
            var result =_employeeService.View();

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
            var result =_employeeService.Create(dto);

            return CreatedAtAction(nameof(CreateEmployee),result);
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
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id, [FromBody] EmployeeDto dto)
        {
            var result =_employeeService.Update(id, dto);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [Route("employee/{id}")]
        [HttpGet]
        public IActionResult GetEmployeebyId([FromRoute] int id)
        {
            //throw new Exception("Database failure");
            var result =_employeeService.GetById(id);

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
        public IActionResult GetEmployees([FromQuery] string? searchText,[FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 5)
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

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "Admin")]
        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            var result = _employeeService.GetByCategory(categoryId);

            return Ok(result);
        }
    }
}
