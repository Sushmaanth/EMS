using Dtos.Repository.Abstraction;
using Entities;
using Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dtos.Repository.Implementation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }
        public Employee Create(Employee employee)
        {
            _context.Employees.Add(employee);

            int result = _context.SaveChanges();

            if (result <= 0)
            {
                throw new Exception("Unable to create employee");
            }

            return employee;
        }

        public ICollection<Employee> View()
        {
            return _context.Employees.ToList();
        }

        public Employee Delete(Employee employee)
        {

            _context.Employees.Remove(employee);

            int result = _context.SaveChanges();

            if (result <= 0)
            {
                throw new Exception("Unable to delete employee");
            }

            return employee;


        }

        public Employee Update(Employee employee)
        {

            _context.Employees.Update(employee);

            int result = _context.SaveChanges();

            if (result <= 0)
            {
                throw new Exception("Unable to update employee");
            }

            return employee;

        }

        public Employee? GetById(int id)
        {

            return _context.Employees.FirstOrDefault(e => e.Id == id);

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

            IQueryable<Employee> query =
                 _context.Employees;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(e =>
                    e.Id.ToString().Contains(searchText)
                    ||
                    e.Name.ToLower().Contains(searchText.ToLower()));
            }

            return query;

        }

        public ICollection<Department> GetDepartments()
        {

            return _context.Departments.ToList();


        }

        public async Task AddDocumentAsync(EmployeeDocument document)
        {
            await _context.EmployeeDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
        }
    }
}
