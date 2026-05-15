using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Repository.Abstraction
{
    public interface IRepository<T> where T : class
    {
        T Create(T data);

        T GetById(int id);

        ICollection<T> View();

        T Update(int id, T data);

        T Delete(int id);
        ICollection<DepartmentDto> GetDepartments();
    }
}
