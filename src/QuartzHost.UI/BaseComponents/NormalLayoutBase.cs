using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuartzHost.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.UI.Components
{
    public class NormalLayoutBase : LayoutComponentBase
    {
        /// <summary>
        /// 获得 IJSRuntime 实例
        /// </summary>
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await JSRuntime.DoVoidAsync("init");
        }
    }
}