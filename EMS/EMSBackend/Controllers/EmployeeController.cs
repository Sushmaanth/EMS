using Dtos;
using Dtos.Repository.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IRepository<EmployeeDto> repository;

        private readonly IEmployeeRepository<EmployeeDto> employeeRepository;

        

        public EmployeeController(IRepository<EmployeeDto> repository, IEmployeeRepository<EmployeeDto> employeeRepository)
        {
            this.repository = repository;
            this.employeeRepository = employeeRepository;
        }

        [Route("all")]
        [HttpGet]
        public IActionResult ViewEmployees()
        {
            try
            {
                var result = repository.View();

                if (!result.Any())
                {
                    return Problem("No Data found");
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }

        [Route("add")]
        [HttpPost]
        public IActionResult CreateEmployee([FromBody] EmployeeDto dto)
        {
            try
            {
                var created = repository.Create(dto);
                return CreatedAtAction(nameof(CreateEmployee), created);
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }

        [Route("delete/{id}")]
        [HttpDelete]
        public IActionResult DeleteEmployee([FromRoute] int id)
        {
            try
            {
                var deleted = repository.Delete(id);
                return Ok(deleted);
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }

        [Route("update/{id}")]
        [HttpPut]
        public IActionResult UpdateEmployee([FromRoute] int id, [FromBody] EmployeeDto dto)
        {
            try
            {
                var updated = repository.Update(id, dto);
                return Ok(updated);
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }

        [Route("employee/{id}")]
        [HttpGet]
        public IActionResult GetEmployeebyId([FromRoute] int id)
        {
            try
            {
                var found = repository.GetById(id);
                return Ok(found);
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }

        [Route("employee/search")]
        [HttpGet]
        public IActionResult SearchEmployee([FromQuery] string? searchText)
        {
            try
            {
                var found = employeeRepository.SearchEmployee(searchText);
                
                return Ok(found);
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }

        [HttpGet]
        [Route("employees")]
        public IActionResult GetEmployees([FromQuery] string? searchText,[FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 5)
        {
            try
            {
                var result = employeeRepository.GetEmployees(searchText,pageNumber, pageSize);

                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }

        [Route("department/all")]
        [HttpGet]
        public IActionResult GetDepartments()
        {
            try
            {
                var departments =
                    repository.GetDepartments();

                return Ok(departments);
            }
            catch (Exception e)
            {
                return Problem(
                    $"Exception: {e.Message}");
            }
        }
    }
}
