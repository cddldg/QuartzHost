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
        protected override async Task OnInitializedAsync()
        {
            await UserCheckAsync();
            Results = await Load();
        }

        public async Task<Result<List<JobNodesEntity>>> Load()
        {
            var model = await Http.GetHttpJsonAsync<Result<List<JobNodesEntity>>>("job/node/all", false);
            CacheHelper.Set("job_node_all", model.Data);
            return model;
        }
    }
}