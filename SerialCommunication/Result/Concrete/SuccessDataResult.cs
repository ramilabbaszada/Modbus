using Core.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Results.Concrete
{
    public class SuccessDataResult<TData>:DataResult<TData>,IDataResult<TData>
    {
        public SuccessDataResult(TData data, string message) : base(data, message, true)
        {

        }
        public SuccessDataResult(TData data) : base(data, true)
        {

        }
    }
}
