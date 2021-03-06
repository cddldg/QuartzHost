﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuartzHost.UI.Models;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using QuartzHost.UI.Common;

namespace QuartzHost.UI.Components
{
    public class DefaultLayoutBase : LayoutComponentBase
    {
        /// <summary>
        /// 获得 IJSRuntime 实例
        /// </summary>
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        public List<Navbar> LeftNavbars { get; set; } = new List<Navbar>
        {
            new Navbar{BarItem=new BarItem{ Link="#",Class="nav-item",Attributes=new Dictionary<string, object>{ { "data-widget","pushmenu" } }, Name="<i class=\"fas fa-bars\"></i>" } },
            new Navbar{BarItem=new BarItem{ Link="/",Class="nav-item d-none d-sm-inline-block", Name="Home" } },
            new Navbar{BarItem=new BarItem{ Link="#",Class="nav-item d-none d-sm-inline-block", Name="Contact" } },
        };

        protected override async Task OnInitializedAsync()
        {
            await JSRuntime.DoVoidAsync("init");
        }
    }
}