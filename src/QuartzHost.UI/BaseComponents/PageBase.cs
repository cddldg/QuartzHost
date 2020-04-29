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
        /// 获得 SessionStorage 实例
        /// </summary>
        [Inject]
        public ISessionStorage SessionStorage { get; set; }

        /// <summary>
        /// 获得 LocalStorage 实例
        /// </summary>
        [Inject]
        protected ILocalStorage LocalStorage { get; set; }

        protected Result<T> Results { get; set; }

        protected async Task UserCheckAsync()
        {
            Console.WriteLine("UserCheck:" + SessionStorage.GetId());
            var user = await SessionStorage.GetItemAsync<JobUserEntity>(SessionStorage.GetId());

            if (user == null || user.Id <= 0)
            {
                user = await LocalStorage.GetItemAsync<JobUserEntity>($"__User");
                if (user != null && user.Id > 0)
                {
                    var re = await UserLoginAsync(user.UserName, Secret.DesDecrypt(user.Password));
                    if (re.Success)
                        await SessionStorage.SetItemAsync(SessionStorage.GetId(), re.Data);
                    else
                        Nav.NavigateTo("/login");
                }
                else
                {
                    await SessionStorage.ClearAsync();
                    Nav.NavigateTo("/login");
                }
            }
        }

        public async Task<Result<JobUserEntity>> UserLoginAsync(string UserName, string Password)
        {
            var input = new Input
            {
                Extens = new Dictionary<string, string> { { "UserName", UserName }, { "Password", Secret.DesEncrypt(Password) } }
            };
            return await Http.PostHttpAsync<Result<JobUserEntity>, Input>($"job/login", input);
        }
    }
}