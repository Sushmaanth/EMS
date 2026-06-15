using Dtos.Repository.Abstraction;
using Entities;
using Entities.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Repository.Implementation
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }
        public ICollection<Role> GetRoles()
        {
            return _context.Role.ToList();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Role.FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
