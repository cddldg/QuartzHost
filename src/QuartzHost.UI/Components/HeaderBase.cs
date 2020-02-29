using Microsoft.AspNetCore.Components;
using QuartzHost.UI.Models;
using QuartzHost.UI.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuartzHost.UI.Components
{
    public class HeaderBase : ComponentBase
    {
        [Parameter]
        public List<Navbar> LeftNavbars { get; set; }
    }
}