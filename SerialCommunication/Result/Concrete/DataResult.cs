using Core.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Results.Concrete
{
    public class DataResult<TData>:Result,IDataResult<TData>
    {
        public DataResult(TData data) : this(data, string.Empty)
        {

        }

        public DataResult(TData data, string message) : this(data, message, false)
        {

        }

        public DataResult(TData data, bool isSuccess) : this(data, string.Empty, isSuccess)
        {

        }

        public DataResult(TData data, string message, bool isSuccess) : base(message, isSuccess)
        {
            Data = data;
        }

        public TData Data { get; }
    }
}
