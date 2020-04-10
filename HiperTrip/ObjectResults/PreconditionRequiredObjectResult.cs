using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;

namespace HiperTrip.ObjectResults
{
    [DefaultStatusCode(404)]
    public class PreconditionRequiredObjectResult : ObjectResult
    {
        /// <summary>
        /// Creates a new PreconditionRequiredObjectResult instance.
        /// </summary>
        /// <param name="value"></param>
        public PreconditionRequiredObjectResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.PreconditionRequired;
        }
    }
}