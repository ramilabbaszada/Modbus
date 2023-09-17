using Core.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Results.Concrete
{
    public class ErrorDataResult<TData>:DataResult<TData>,IDataResult<TData>
    {
        public ErrorDataResult(TData data, string message) : base(data, message, false)
        {

        }

        public ErrorDataResult(TData data) : base(data, false)
        {

        }
        public ErrorDataResult(string message) : base(default, message, false)
        {

        }
    }
}
