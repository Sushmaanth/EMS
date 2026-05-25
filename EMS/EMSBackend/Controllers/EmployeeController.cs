using Dtos;
using EMSBackend.Service.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EMSBackend.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

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

        [Route("add")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            var result =_employeeService.Create(dto);

            return CreatedAtAction(nameof(CreateEmployee),result);
        }

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

        [Route("employee/{id}")]
        [HttpGet]
        public IActionResult GetEmployeebyId([FromRoute] int id)
        {

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

        [HttpGet]
        [Route("employees")]
        public IActionResult GetEmployees([FromQuery] string? searchText,[FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 5)
        {
            var result = _employeeService.GetEmployees(
                searchText,
                pageNumber,
                pageSize);

            return Ok(result);
        }

        [Route("department/all")]
        [HttpGet]
        public IActionResult GetDepartments()
        {
            var result = _employeeService.GetDepartments();
            return Ok(result);
        }
    }
}
