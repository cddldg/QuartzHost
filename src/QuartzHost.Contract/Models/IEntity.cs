using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzHost.Contract.Models
{
    public interface IEntity
    {
    }

    public class BaseEntity
    {
        public int Total { get; set; }
    }
}