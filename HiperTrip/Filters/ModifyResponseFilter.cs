using Entities.Enums;
using HiperTrip.Interfaces;
using HiperTrip.ObjectResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Net;

namespace HiperTrip.Filters
{
    public class ModifyResponseFilter : ActionFilterAttribute
    {
        private readonly IResultService _resultService;

        public ModifyResponseFilter(IResultService resultService)
        {
            _resultService = resultService;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context != null)
            {
                if (context.Result is ObjectResult objectResult)
                {
                    FormatResult(context, objectResult);
                }
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //IActionResult result = filterContext.Result;

            //if (result is JsonResult json)
            //{
            //    var x = json.Value;
            //    var status = json.StatusCode;
            //}            
        }

        private void FormatResult(ActionExecutedContext filterContext, ObjectResult objectResult)
        {
            if (objectResult.Value is Dictionary<string, object> valorResult)
            {
                if (valorResult.ContainsKey("StatusCode"))
                {
                    switch (valorResult["StatusCode"])
                    {
                        case HttpStatusCode.OK:
                            {
                                filterContext.Result = new OkObjectResult(objectResult.Value);
                                break;
                            }

                        case HttpStatusCode.BadRequest:
                            {
                                filterContext.Result = new BadRequestObjectResult(objectResult.Value);
                                break;
                            }

                        case HttpStatusCode.NotFound:
                            {
                                filterContext.Result = new NotFoundObjectResult(objectResult.Value);
                                break;
                            }

                        case HttpStatusCode.NoContent:
                            {
                                filterContext.Result = new NoContentResult();
                                break;
                            }

                        case HttpStatusCode.PreconditionRequired:
                            {
                                filterContext.Result = new PreconditionRequiredObjectResult(objectResult.Value);
                                break;
                            }

                        case HttpStatusCode.InternalServerError:
                            {
                                filterContext.Result = new InternalServerErrorObjectResult(objectResult.Value);
                                break;
                            }

                        case HttpStatusCode.Unauthorized:
                            {
                                filterContext.Result = new UnauthorizedObjectResult(objectResult.Value);
                                break;
                            }

                        default:
                            {
                                filterContext.Result = new BadRequestObjectResult(objectResult.Value);
                                break;
                            }
                    }

                    valorResult.Remove("StatusCode");
                }
                else
                {
                    _resultService.AddValue(Resultado.Error, "No se encuentra el código de estado de respuesta.");

                    filterContext.Result = new BadRequestObjectResult(_resultService.GetProperties());
                }
            }
        }
    }
}