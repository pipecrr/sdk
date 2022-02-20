using System.Collections.Generic;

namespace Siesa.SDK.Shared.Business
{
    public class ActionResult<T>
    {
        public bool Success { get; set; } = true;
        public T Data { get; set; }
        public ICollection<string> Errors { get; set; }

        public ActionResult()
        {
            Errors = new List<string>();
        }

    }

    public class ActionResult: ActionResult<object>
    {
    }

    public class NotFoundResult<T> : ActionResult<T>
    {
        public NotFoundResult()
        {
            Success = false;
            Errors.Add("Not found");
        }
    }

    public class BadRequestResult<T> : ActionResult<T>
    {
        public BadRequestResult()
        {
            Success = false;
            Errors.Add("Bad request");
        }
    }

    public class InternalServerErrorResult<T> : ActionResult<T>
    {
        public InternalServerErrorResult()
        {
            Success = false;
            Errors.Add("Internal server error");
        }
    }
    
}