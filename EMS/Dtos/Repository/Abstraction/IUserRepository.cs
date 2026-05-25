using System;

namespace Dtos.Repository.Abstraction
{
    public interface IUserRepository<T> where T : class
    {
        ActivateAccountResponseDTO AccountActivation(T data);
    }
}
