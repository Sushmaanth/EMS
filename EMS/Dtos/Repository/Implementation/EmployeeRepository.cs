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
    public class EmployeeRepository :IEmployeeRepository
    {
        private readonly AppDbContext context;

        public EmployeeRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Employee Create(Employee employee)
        {
            try
            {
                context.Employees.Add(employee);

                int result = context.SaveChanges();

                if (result <= 0)
                {
                    throw new Exception("Unable to create employee");
                }

                return employee;
            }
            catch
            {
                throw;
            }
        }

        public ICollection<Employee> View()
        {
            try
            {
                return context.Employees.ToList();
            }
            catch
            {
                throw;
            }
        }

        public Employee Delete(Employee employee)
        {
            try
            {
                context.Employees.Remove(employee);

                int result = context.SaveChanges();

                if (result <= 0)
                {
                    throw new Exception("Unable to delete employee");
                }

                return employee;
            }
            catch
            {
                throw;
            }
            
        }

        public Employee Update(Employee employee)
        {
            try
            {
                context.Employees.Update(employee);

                int result = context.SaveChanges();

                if (result <= 0)
                {
                    throw new Exception("Unable to update employee");
                }

                return employee;
            }
            catch
            {
                throw;
            }
        }

        public Employee? GetById(int id)
        {
            try
            {
                return context.Employees.FirstOrDefault(e => e.Id == id);
            }
            catch
            {
                throw;
            }
        }

        //public IEnumerable<EmployeeDto> SearchEmployee(string? searchText)
        //{
        //    try
        //    {
        //        IQueryable<Employee> query = context.Employees;

        //        if (!string.IsNullOrWhiteSpace(searchText))
        //        {
        //            query = query.Where(e => e.Id.ToString().Contains(searchText) ||
        //            e.Name.ToLower().ToString().Contains(searchText));
        //        }

        //        var employees = query.Select(e => new EmployeeDto
        //        {
        //            Id = e.Id,
        //            Name = e.Name,
        //            EmailId = e.EmailId,
        //            Mobile = e.Mobile
        //        }).ToList();

        //        return employees;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public IQueryable<Employee> GetEmployees(string? searchText)
        {
            try
            {
                IQueryable<Employee> query =
                     context.Employees;

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(e =>
                        e.Id.ToString().Contains(searchText)
                        ||
                        e.Name.ToLower().Contains(searchText.ToLower()));
                }

                return query;
            }
            catch
            {
                throw;
            }
        }

        public ICollection<Department> GetDepartments()
        {
            try
            {
                return context.Departments.ToList();

            }
            catch
            {
                throw;
            }
        }
    }
}
