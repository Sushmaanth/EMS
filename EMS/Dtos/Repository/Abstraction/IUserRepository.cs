using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Repository.Abstraction
{
    public interface IUserRepository<T> where T : class
    {
        ActivateAccountResponseDTO AccountActivation(T data);
    }
}
