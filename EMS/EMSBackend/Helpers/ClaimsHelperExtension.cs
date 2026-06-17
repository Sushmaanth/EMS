using System.Security.Claims;

namespace EMSBackend.Helpers
{
    public static class ClaimsHelperExtension
    {
        public static int GetEmployeeId(this ClaimsPrincipal user)        
        {
            string employeeId = user.FindFirst("EmployeeId").Value;

            if (!int.TryParse(employeeId, out int id))
            {
                throw new UnauthorizedAccessException("EmployeeId claim not found");
            }

            return id;
        }

        public static string GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }
    }
}
