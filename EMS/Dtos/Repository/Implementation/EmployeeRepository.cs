using Dtos.Repository.Abstraction;
using Entities;
using Entities.Data;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dtos.Repository.Implementation
{
    public class EmployeeRepository : IRepository<EmployeeDto>, IEmployeeRepository<EmployeeDto>
    {
        private readonly AppDbContext context;

        public EmployeeRepository(AppDbContext context)
        {
            this.context = context;
        }
        public EmployeeDto Create(EmployeeDto data)
        {
            try
            {
                Employee e = new()
                {
                    Name = data.Name,
                    Gender = data.Gender,
                    DateOfBirth = data.DateOfBirth,
                    EmailId = data.EmailId,
                    Mobile = data.Mobile,
                    Salary =  data.Salary,
                    DateOfJoining = data.DateOfJoining,
                    DepartmentId = data.DepartmentId
                };

                var entity = context.Add(e);
                int result = context.SaveChanges();

                if (result > 0)
                {
                    var employee = context.Employees
                 .Where(emp => emp.Id == e.Id)
                 .Select(emp => new EmployeeDto
                 {
                     Id = emp.Id,
                     Name = emp.Name,
                     Gender = emp.Gender,
                     DateOfBirth = emp.DateOfBirth,
                     EmailId = emp.EmailId,
                     Mobile = emp.Mobile,
                     Salary = emp.Salary,
                     DateOfJoining = emp.DateOfJoining,
                     DepartmentId = emp.DepartmentId,

                     DepartmentName =
                         emp.Department.DepartmentName
                 })
                 .FirstOrDefault();

                    return employee;
                }
                else
                {
                    throw new Exception("Can't create an employee");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ICollection<EmployeeDto> View()
        {
            try
            {
                var employees = context.Employees.ToList();

                return employees.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    EmailId = e.EmailId,
                    Mobile = e.Mobile,
                    DepartmentId = e.DepartmentId
                }).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public EmployeeDto Delete(int id)
        {
            try
            {
                var found = context.Employees.Find(id);
                if (found == null)
                {
                    return null;
                }
                else
                {
                    context.Remove(found);
                    int result = context.SaveChanges();
                    if (result > 0)
                    {
                        EmployeeDto dto = new()
                        {
                            Id = found.Id,
                            Name = found.Name,
                            Gender = found.Gender,
                            DateOfBirth = found.DateOfBirth,
                            EmailId = found.EmailId,
                            Mobile = found.Mobile,
                            Salary = found.Salary,
                            DateOfJoining = found.DateOfJoining,
                            DepartmentId = found.DepartmentId
                        };

                        return dto;
                    }
                    else
                    {
                        throw new Exception("Can't delete the employee");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

        public EmployeeDto Update(int id, EmployeeDto data)
        {
            try
            {
                var found = context.Employees.Find(id);
                if (found == null)
                {
                    return null;
                }
                else
                {
                    found.Name = data.Name;
                    found.Gender = data.Gender;
                    found.DateOfBirth = data.DateOfBirth;
                    found.EmailId = data.EmailId;
                    found.Mobile = data.Mobile;
                    found.Salary = data.Salary;
                    found.DateOfJoining = data.DateOfJoining;
                    found.DepartmentId = data.DepartmentId;

                    int result = context.SaveChanges();
                    if (result > 0)
                    {
                        EmployeeDto dto = new()
                        {
                            Id = found.Id,
                            Name = found.Name,
                            Gender = found.Gender,
                            DateOfBirth = found.DateOfBirth,
                            EmailId = found.EmailId,
                            Mobile = found.Mobile,
                            Salary = found.Salary,
                            DateOfJoining = found.DateOfJoining,
                            DepartmentId = found.DepartmentId,

                            DepartmentName =
                    found.Department != null
                        ? found.Department.DepartmentName
                        : null
                        };
                        return dto;
                    }
                    else
                    {
                        throw new Exception("can't update the employee");
                    }
                }
            
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public EmployeeDto? GetById(int id)
        {
            try
            {
                var employee = context.Employees
                    .Where(e => e.Id == id)
                    .Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Gender = e.Gender,
                        DateOfBirth = e.DateOfBirth,
                        EmailId = e.EmailId,
                        Mobile = e.Mobile,
                        Salary = e.Salary,
                        DateOfJoining = e.DateOfJoining,
                        DepartmentId = e.DepartmentId,

                        DepartmentName =
                            e.Department != null
                                ? e.Department.DepartmentName
                                : null
                    })
                    .FirstOrDefault();

                return employee;
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<EmployeeDto> SearchEmployee(string? searchText)
        {
            IQueryable<Employee> query = context.Employees;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(e => e.Id.ToString().Contains(searchText) ||
                e.Name.ToLower().ToString().Contains(searchText));
            }

            var employees = query.Select(e => new EmployeeDto
             {
                 Id = e.Id,
                 Name = e.Name,
                 EmailId = e.EmailId,
                 Mobile = e.Mobile
             }).ToList();

            return employees;
        }

        public PagenationDto<EmployeeDto> GetEmployees(string? searchText,int pageNumber,int pageSize)
        {
            IQueryable<Employee> query =
                context.Employees;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(e =>
                    e.Id.ToString().Contains(searchText)
                    ||
                    e.Name.ToLower().Contains(
                        searchText.ToLower()));
            }

            var totalRecords = query.Count();

            var employees = query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmployeeDto()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Gender = e.Gender,
                    DateOfBirth = e.DateOfBirth,
                    EmailId = e.EmailId,
                    Mobile = e.Mobile,
                    Salary = e.Salary,
                    DateOfJoining = e.DateOfJoining,
                    DepartmentId = e.DepartmentId,

                    DepartmentName =
                        e.Department != null
                            ? e.Department.DepartmentName
                            : null
                })
                .ToList();

            return new PagenationDto<EmployeeDto>
            {
                Data = employees,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchText = searchText
            };
        }

        public ICollection<DepartmentDto> GetDepartments()
        {
            try
            {
                var departments =
                    context.Departments
                    .Select(d => new DepartmentDto
                    {
                        Id = d.Id,
                        DepartmentName = d.DepartmentName
                    })
                    .ToList();

                return departments;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
