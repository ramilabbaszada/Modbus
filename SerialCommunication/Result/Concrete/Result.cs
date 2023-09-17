using Core.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Results.Concrete
{
    public class Result:IResult
    {
        public Result(string message, bool isSuccess) : this(isSuccess)
        {
            Message = message;
        }

        public Result(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public string Message { get; }
        public bool IsSuccess { get; }
    }
}
