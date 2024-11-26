using System.Net;

namespace ProjectBase.Domain.Exceptions
{
    public class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Code { get; }
        public List<string> Errors { get; set; } = [];
        public AppException(HttpStatusCode status, string code, string message) : base(message)
        {
            StatusCode = status;
            Code = code;
        }

        public AppException(HttpStatusCode status, string code, List<string> errors, string message) : base(message)
        {
            StatusCode = status;
            Code = code;
            Errors = errors;
        }
    }
}
