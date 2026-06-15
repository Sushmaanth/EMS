using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Repository.Abstraction
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);

        ICollection<Role> GetRoles();
    }
}
