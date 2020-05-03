using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;

namespace HiperTrip.ObjectResults
{
    [DefaultStatusCode(404)]
    public class InternalServerErrorObjectResult : ObjectResult
    {
        /// <summary>
        /// Creates a new InternalServerErrorObjectResult instance.
        /// </summary>
        /// <param name="value"></param>
        public InternalServerErrorObjectResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}