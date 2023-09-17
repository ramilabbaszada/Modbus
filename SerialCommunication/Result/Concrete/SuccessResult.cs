using Core.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Results.Concrete
{
    public class SuccessResult:Result,IResult
    {
        public SuccessResult(string message) : base(message, true)
        {

        }

        public SuccessResult() : base(true)
        {

        }
    }
}
