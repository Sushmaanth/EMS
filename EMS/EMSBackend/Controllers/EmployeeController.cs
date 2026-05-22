using Dtos;
using Dtos.Repository.Abstraction;
using Dtos.Repository.Implementation;
using Dtos.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EMSBackend.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IRepository<EmployeeDto> repository;

        private readonly IEmployeeRepository<EmployeeDto> employeeRepository;
        private readonly EmployeeValidator validator;
       

        public EmployeeController(IRepository<EmployeeDto> repository, IEmployeeRepository<EmployeeDto> employeeRepository, EmployeeValidator validator)
        {
            this.repository = repository;
            this.employeeRepository = employeeRepository;
            this.validator = validator;
           
        }

        [Route("all")]
        [HttpGet]
        public IActionResult ViewEmployees()
        {
           var result = repository.View();

           if (!result.Any())
           {
              return NotFound("No Data found");
           }
           else
           {
              return Ok(result);
           }
        }

        [Route("add")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDto dto)
        {
            //use fluent api - refer validators
            var created = repository.Create(dto);
            return CreatedAtAction(nameof(CreateEmployee), created);
        }

        [Route("delete/{id}")]
        [HttpDelete]
        public IActionResult DeleteEmployee([FromRoute] int id)
        {
           var deleted = repository.Delete(id);
           if (deleted ==null)
           {
              return NotFound("Employee not found");
           }
           return Ok(deleted);
        }

        [Route("update/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id, [FromBody] EmployeeDto dto)
        {
           var updated = repository.Update(id, dto);
           if (updated == null)
           {
              return NotFound("Employee not found");
           }
           return Ok(updated);
        }

        [Route("employee/{id}")]
        [HttpGet]
        public IActionResult GetEmployeebyId([FromRoute] int id)
        {
            
                var found = repository.GetById(id);
                if (found == null)
                {
                    return NotFound("Employee not found");
                }
            return Ok(found);
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
                var result = employeeRepository.GetEmployees(searchText,pageNumber, pageSize);

                return Ok(result);
        }

        [Route("department/all")]
        [HttpGet]
        public IActionResult GetDepartments()
        {
           var departments = repository.GetDepartments();
            return Ok(departments);
        }
    }
}
