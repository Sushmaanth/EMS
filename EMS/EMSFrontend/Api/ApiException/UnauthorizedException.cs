namespace EMSFrontend.Api.ApiException
{
        public class UnauthorizedException : Exception
        {
            public UnauthorizedException(string message)
                : base(message)
            {
            }
        }
}

