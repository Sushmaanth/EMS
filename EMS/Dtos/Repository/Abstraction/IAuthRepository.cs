using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Repository.Abstraction
{
    public interface IAuthRepository
    {
        ActivateAccountResponseDTO AccountActivation(ActivateAccountDTO data);
        User? GetUserByEmail(string email);

        void UpdateRefreshToken(User user, string refreshToken, DateTime expiryTime);

        void Save();

        void UpdatePasswordResetOtp(User user,string otp,DateTime expiryTime);

        void IncrementOtpFailedAttempts(User user);

        void ClearOtp(User user);

        void ResetOtpAttempts(User user);

        void UpdatePassword(User user, string passwordHash);
    }
}
