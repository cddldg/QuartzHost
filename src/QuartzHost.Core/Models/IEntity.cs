using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzHost.Core.Models
{
    public interface IEntity
    {
    }

    public class BaseEntity
    {
        public int Total { get; set; }
    }
}