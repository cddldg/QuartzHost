using Microsoft.AspNetCore.Components;
using QuartzHost.Contract.Models;
using QuartzHost.UI.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuartzHost.UI.Components
{
    public class TasksBase : PageBase<List<JobTasksEntity>>
    {
        protected override async Task OnInitializedAsync()
        {
            await UserCheckAsync();
            await Load();
            await JSRuntime.DoVoidAsync("initDataTable", new[] { "#task" });
        }

        public async Task Load(int pageIndex = 1, int pageSize = 10)
        {
            Results = await Pager(pageIndex, pageSize);
        }

        public async Task<PageResult<List<JobTasksEntity>>> Pager(int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PageInput
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                OrderBy = "TotalRunCount DESC"
            };

            return await Http.PostHttpAsync<PageResult<List<JobTasksEntity>>, PageInput>($"job/task/pager", pager, nodeName: await SessionStorage.GetItemAsync<string>("nodename"));
        }

        public async Task<Result<JobTaskStatus>> SingleSetting(SingleType type, string name, long sid)
        {
            var re = await Http.PostHttpAsync<Result<JobTaskStatus>>($"job/task/{type}/{sid}", nodeName: await SessionStorage.GetItemAsync<string>("nodename"));
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