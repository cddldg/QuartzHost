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
    public class TasksBase : PageBase<List<JobTasksEntity>>
    {
        protected override async Task OnInitializedAsync()
        {
            Results = await Http.PostHttpAsync<Result<List<JobTasksEntity>>>($"job/task/all");
            await JSRuntime.DoVoidAsync("initDataTable", new[] { "#task" });
        }

        public async Task Load()
        {
            Results = await Http.PostHttpAsync<Result<List<JobTasksEntity>>>($"job/task/all");
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
            return await Http.PostHttpAsync<PageResult<List<JobTasksEntity>>>($"job/task/pager", pager);
        }

        public async Task<PageResult<List<JobTasksEntity>>> LogPager(long id, int pageIndex = 1, int pageSize = 20)
        {
            var pager = new PageInput
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                Extens = new Dictionary<string, string> { { "TaskId", id.ToString() } }
            };
            return await Http.PostHttpAsync<PageResult<List<JobTasksEntity>>>($"job/task/trace", pager);
        }

        public async Task ShowLog(long id, int pageIndex = 1, int pageSize = 20)
        {
            var pager = new PageInput
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                Extens = new Dictionary<string, string> { { "TaskId", id.ToString() } }
            };
            var logs = await Http.PostHttpAsync<PageResult<List<JobTasksEntity>>>($"job/task/trace", pager);
            Console.WriteLine(logs.ToJson());
        }

        public async Task<Result<JobTaskStatus>> SingleSetting(SingleType type, string name, long sid)
        {
            var re = await Http.PostHttpAsync<Result<JobTaskStatus>>($"job/task/{type}/{sid}");
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