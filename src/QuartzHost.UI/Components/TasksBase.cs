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

        public async Task Load()
        {
            Results = await Http.PostHttpAsync<Result<List<JobTasksEntity>>>($"{ApiHost}/job/task/all");
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

        public async Task<Result<JobTaskStatus>> SingleSetting(SingleType type, string name, long sid)
        {
            var re = await Http.PostHttpAsync<Result<JobTaskStatus>>($"{ApiHost}/job/task/{type}/{sid}");
            if (re.Success)
            {
                await JSRuntime.DoVoidAsync("toastrs", new[] { "success", name, re.Message });
                await Load();
            }
            else
                await JSRuntime.DoVoidAsync("toastrs", new[] { "error", name, re.Message });

            return re;
        }

        public enum SingleType
        {
            start,
            pause,
            runonce,
            resume,
            stop,
            delete
        }
    }
}