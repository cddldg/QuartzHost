using Microsoft.AspNetCore.Components;
using QuartzHost.Contract.Models;
using QuartzHost.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.UI.Components
{
    public class IndexBase : PageBase<List<JobNodesEntity>>
    {
        public string Name { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await UserCheckAsync();
            Results = await Load();
        }

        public async Task<Result<List<JobNodesEntity>>> Load()
        {
            var model = await Http.GetHttpJsonAsync<Result<List<JobNodesEntity>>>("job/node/all", false);
            CacheHelper.Set("job_node_all", model.Data);
            Name = model.Data?.FirstOrDefault(p => p.Status == NodeStatus.Run)?.NodeName ?? "";
            await SessionStorage.SetItemAsync("nodename", Name);
            return model;
        }

        public async Task Startd(string name)
        {
            await SessionStorage.SetItemAsync("nodename", name);
            await JSRuntime.DoVoidAsync("toastrs", new[] { "success", "节点选择", $"已选节点：{name}" });
        }
    }
}