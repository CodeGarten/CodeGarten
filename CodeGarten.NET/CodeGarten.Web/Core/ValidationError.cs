using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CodeGarten.Web.Core
{
    public class ValidationError
    {
        public ValidationError(string field, string error)
        {
            Field = field;
            Error = error;
        }

        public string Field { get; set; }

        public string Error { get; set; }

        public static IEnumerable<ValidationError> Parse(ModelStateDictionary modelState)
        {
            return modelState.Where(ms => ms.Value.Errors.Count > 0).Select(
                ms => new ValidationError(ms.Key, ms.Value.Errors[0].ErrorMessage));
        }
    }
}