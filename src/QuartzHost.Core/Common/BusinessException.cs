using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuartzHost.Contract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace QuartzHost.Core.Common
{
    public class BusinessException : Exception
    {
        public ResultStatus ResultStatus { get; set; } = ResultStatus.ServiceError;
        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.OK;
        public override string Message { get; }

        public BusinessException()
        {
        }

        public BusinessException(string msg)
        {
            Message = msg;
        }

        public BusinessException(ResultStatus status)
        {
            ResultStatus = status;
        }

        public BusinessException(ResultStatus status, string msg)
        {
            ResultStatus = status;
            Message = msg;
        }

        public BusinessException(ResultStatus status, HttpStatusCode code)
        {
            ResultStatus = status;
            HttpStatusCode = code;
        }

        public BusinessException(ResultStatus status, HttpStatusCode code, string msg)
        {
            ResultStatus = status;
            HttpStatusCode = code;
            Message = msg;
        }

        public BusinessException(ModelStateDictionary modelState)
        {
            ResultStatus = ResultStatus.ValidateError;
            //HttpStatusCode = HttpStatusCode.BadRequest;
            var state = modelState.FirstOrDefault();
            Message = string.Join(",",
                state.Value.Errors.Select(x =>
                    string.IsNullOrEmpty(x.ErrorMessage) ? x.Exception?.Message : x.ErrorMessage));
        }
    }
}