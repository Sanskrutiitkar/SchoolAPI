

namespace SchoolApi.Core.Business.GlobalException
{
    public class UnauthorizedException: Exception
    {
        public UnauthorizedException(string message)
            : base(message) { }
    }    
}