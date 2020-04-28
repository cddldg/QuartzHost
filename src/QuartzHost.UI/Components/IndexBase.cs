using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.UI.Components
{
    public class IndexBase : PageBase<string>
    {
        protected override async Task OnInitializedAsync()
        {
            await UserCheckAsync();
        }
    }
}