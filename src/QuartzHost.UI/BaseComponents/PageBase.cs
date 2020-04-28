using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuartzHost.Contract.Common;
using QuartzHost.Contract.Models;
using QuartzHost.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace QuartzHost.UI.Components
{
    public class PageBase<T> : ComponentBase
    {
        /// <summary>
        /// 获得 IJSRuntime 实例
        /// </summary>
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected HttpClient Http { get; set; }

        /// <summary>
        /// 获得 IJSRuntime 实例
        /// </summary>
        [Inject]
        protected NavigationManager Nav { get; set; }

        /// <summary>
        /// 获得 IJSRuntime 实例
        /// </summary>
        [Inject]
        protected ISessionStorage SessionStorage { get; set; }

        protected Result<T> Results { get; set; }

        protected async Task UserCheckAsync()
        {
            Console.WriteLine("ccId:" + SessionStorage.GetId());
            var user = await SessionStorage.GetItemAsync<JobUserEntity>(SessionStorage.GetId());

            if (user == null || user.Id <= 0)
            {
                try
                {
                    await SessionStorage.RemoveItemAsync(SessionStorage.GetId());
                }
                catch
                {
                    await SessionStorage.ClearAsync();
                }
                Nav.NavigateTo("/login");
            }
        }
    }
}