using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuartzHost.UI.Common;
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

        /// <summary>
        /// 获得 IJSRuntime 实例
        /// </summary>
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// 获得 IJSRuntime 实例
        /// </summary>
        [Inject]
        protected ISessionStorage SessionStorage { get; set; }

        /// <summary>
        /// 获得 IJSRuntime 实例
        /// </summary>
        [Inject]
        protected NavigationManager Nav { get; set; }

        public async System.Threading.Tasks.Task LogOutAsync()
        {
            await SessionStorage.ClearAsync();
            Nav.NavigateTo("/login");
        }
    }
}