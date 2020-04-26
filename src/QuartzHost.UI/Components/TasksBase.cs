using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuartzHost.Contract.Models;
using QuartzHost.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace QuartzHost.UI.Components
{
    public class TasksBase : PageBase<List<JobTasksEntity>>
    {
        public const string ApiHost = "http://localhost:60000";

        protected override async Task OnInitializedAsync()
        {
            Results = await Http.PostHttpAsync<Result<List<JobTasksEntity>>>($"{ApiHost}/job/task/all");
            await JSRuntime.DoVoidAsync("initDataTable", new[] { "#task" });
        }

        public async Task<PageResult<List<JobTasksEntity>>> Pager()
        {
            var pager = new PageInput
            {
                PageSize = 2,
                PageIndex = 1,
                Extens = new Dictionary<string, string> { { "Title", "b" } },
                OrderBy = "TotalRunCount DESC"
            };
            return await Http.PostHttpAsync<PageResult<List<JobTasksEntity>>>($"{ApiHost}/job/task/pager", pager);
        }

        public async Task<Result<JobTaskStatus>> Start(long sid)
        {
            return await Http.PostHttpAsync<Result<JobTaskStatus>>($"{ApiHost}/job/task/start/{sid}");
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<JobTaskStatus>> Stop(long id)
        {
            return await Http.PostHttpAsync<Result<JobTaskStatus>>($"{ApiHost}/job/task/stop/{id}");
        }
    }
}