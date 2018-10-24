using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sample.api
{
    public class ValidationProblemDetails : ProblemDetails
    {
        public ICollection<ValidationError> ValidationErrors { get; set; }
    }

    public class ValidationError
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ValidationProblemDetailsResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            var modelStateEntries = context.ModelState.Where(e => e.Value.Errors.Count > 0).ToArray();
            var errors = new List<ValidationError>();

            var details = "See ValidationErrors for details";

            if (modelStateEntries.Any())
            {
                if (modelStateEntries.Length == 1 && modelStateEntries[0].Value.Errors.Count == 1 && modelStateEntries[0].Key == string.Empty)
                {
                    details = modelStateEntries[0].Value.Errors[0].ErrorMessage;
                }
                else
                {
                    foreach (var modelStateEntry in modelStateEntries)
                    {
                        foreach (var modelStateError in modelStateEntry.Value.Errors)
                        {
                            var error = new ValidationError
                            {
                                Name = modelStateEntry.Key,
                                Description = modelStateError.ErrorMessage
                            };

                            errors.Add(error);
                        }
                    }
                }
            }

            var problemDetails = new ValidationProblemDetails
            {
                Status = 400,
                Title = "Request Validation Error",
                Instance = $"urn:myorganization:badrequest:{Guid.NewGuid()}",
                Detail = details,
                ValidationErrors = errors
            };

            context.HttpContext.Response.ContentType = "application/problem+json";
            context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
            return Task.CompletedTask;
        }
    }
}
