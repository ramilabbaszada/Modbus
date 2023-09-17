﻿using Core.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Results.Concrete
{
    public class ErrorResult:Result,IResult
    {
        public ErrorResult(string message) : base(message, false)
        {

        }

        public ErrorResult() : base(false)
        {

        }
    }
}
