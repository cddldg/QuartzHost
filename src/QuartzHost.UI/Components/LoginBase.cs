using Microsoft.AspNetCore.Components;
using QuartzHost.Contract.Models;
using QuartzHost.UI.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuartzHost.UI.Components
{
    public class LoginBase : PageBase<JobUserEntity>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }

        public async Task UserLoginAsync()
        {
            Console.WriteLine($"{UserName} {Password} {Remember} {Contract.Common.Secret.GetMd5(Password)} ");
            var input = new Input
            {
                Extens = new Dictionary<string, string> { { "UserName", UserName }, { "Password", Password } }
            };
            var re = await Http.PostHttpAsync<Result<JobUserEntity>>($"job/login", input);
            if (re.Success)
            {
                await JSRuntime.DoVoidAsync("toastrs", new[] { "success", "登录", re.Message });
                re.Data.Password = "";
                await SessionStorage.SetItemAsync(SessionStorage.GetId(), re.Data);
                Nav.NavigateTo("/");
            }
            else
                await JSRuntime.DoVoidAsync("toastrs", new[] { "error", "登录", re.Message });
        }
    }
}