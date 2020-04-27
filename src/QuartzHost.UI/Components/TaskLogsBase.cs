using Microsoft.AspNetCore.Components;
using QuartzHost.Contract.Common;
using QuartzHost.Contract.Models;
using QuartzHost.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.UI.Components
{
    public class TaskLogsBase : PageBase<List<JobTraceEntity>>
    {
        [Parameter]
        public string Info { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Load();
            await JSRuntime.DoVoidAsync("initDataTable", new[] { "#logs" });
        }

        public async Task Load(int pageIndex = 1, int pageSize = 10)
        {
            Results = await Pager(pageIndex, pageSize);
        }

        public async Task<PageResult<List<JobTraceEntity>>> Pager(int pageIndex = 1, int pageSize = 10)
        {
            Id = Info.Split('|')[0];
            Name = Info.Split('|')[1];
            var pager = new PageInput
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                Extens = new Dictionary<string, string> { { "TaskId", Id } }
            };

            return await Http.PostHttpAsync<PageResult<List<JobTraceEntity>>>($"job/task/trace", pager);
        }
    }
}