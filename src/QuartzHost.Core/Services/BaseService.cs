using Quartz;
using QuartzHost.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzHost.Core.Services
{
    public abstract class BaseService
    {
        public BaseService()
        {
        }

        protected ServiceResponseMessage ServiceResult(ResultStatus status, string msg = "", object data = null)
        {
            return new ServiceResponseMessage(status, msg, data);
        }
    }
}