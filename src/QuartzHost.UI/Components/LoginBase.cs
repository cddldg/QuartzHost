using Microsoft.AspNetCore.Components;
using QuartzHost.Contract.Common;
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

        protected override async Task OnInitializedAsync()
        {
            var user = await LocalStorage.GetItemAsync<JobUserEntity>($"__User");
            Console.WriteLine("__User:" + user.ToJson());
            if (user != null && user.Id > 0)
            {
                UserName = user.UserName;
                Password = Secret.DesDecrypt(user.Password);
                Remember = true;
                await UserLoginAsync();
            };
        }

        public async Task UserLoginAsync()
        {
            var input = new Input
            {
                Extens = new Dictionary<string, string> { { "UserName", UserName }, { "Password", Secret.DesEncrypt(Password) } }
            };

            var re = await Http.PostHttpAsync<Result<JobUserEntity>, Input>($"job/login", input);
            if (re.Success)
            {
                await JSRuntime.DoVoidAsync("toastrs", new[] { "success", "登录", re.Message });

                re.Data.Password = "******";
                await SessionStorage.SetItemAsync(SessionStorage.GetId(), re.Data);
                if (Remember)
                {
                    re.Data.Password = Secret.DesEncrypt(Password);
                    await LocalStorage.SetItemAsync($"__User", re.Data);
                }

                Nav.NavigateTo("/");
            }
            else
                await JSRuntime.DoVoidAsync("toastrs", new[] { "error", "登录", re.Message });
        }
    }
}