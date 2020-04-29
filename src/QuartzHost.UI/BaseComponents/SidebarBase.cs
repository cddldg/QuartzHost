using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuartzHost.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuartzHost.UI.Common;
using QuartzHost.Contract.Models;

namespace QuartzHost.UI.Components
{
    public class SidebarBase : ComponentBase
    {
        /// <summary>
        /// 获得 IJSRuntime 实例
        /// </summary>
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// 获得 SessionStorage 实例
        /// </summary>
        [Inject]
        protected ISessionStorage SessionStorage { get; set; }

        /// <summary>
        /// 侧边栏
        /// </summary>
        [Parameter]
        public List<Sidebar> Sidebars { get; set; }

        /// <summary>
        /// 获得 网站标题
        /// </summary>
        [Parameter]
        public string WebName { get; set; } = "QuartzHost";

        public string Logo { get; set; } = "img/logo.png";

        /// <summary>
        /// 用户信息
        /// </summary>
        public JobUserEntity UserInfo { get; set; } //= new JobUserEntity { Name = "抵拢倒拐", Img = "img/lbxx.jpg" };

        protected override async Task OnInitializedAsync()
        {
            var user = await SessionStorage.GetItemAsync<JobUserEntity>(SessionStorage.GetId());
            if (user != null)
            {
                user.Img = "img/lbxx.jpg";
                UserInfo = user;
            }
            else
            {
                UserInfo = new JobUserEntity { RealName = "小秘密", Img = "img/lbxx.jpg" };
            }
        }

        public void MenuClick()
        {
            JSRuntime.DoVoid("onnav");
        }
    }
}