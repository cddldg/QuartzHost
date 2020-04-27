using Microsoft.AspNetCore.Components;
using QuartzHost.Contract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.UI.Components
{
    public class PagerInfoBase : ComponentBase
    {
        [Parameter]
        public int Colspan { get; set; }

        [Parameter]
        public int Total { get; set; }

        [Parameter]
        public int PageIndex { get; set; }

        [Parameter]
        public int PageSize { get; set; }

        public Previou Previous { get; set; } = new Previou();

        public Next Nexts { get; set; } = new Next();

        public int[] PageSplit { get; set; } = new int[] { };

        protected override async Task OnInitializedAsync()
        {
            await CreatePager();
        }

        public Task CreatePager()
        {
            Console.WriteLine($"{Total},{PageSize},{PageIndex},{10}");
            if (Total <= PageSize || PageIndex == 1 || Total == 0)
                Previous.Disabled = "disabled";

            if (PageIndex * PageSize >= Total || Total == 0)
                Nexts.Disabled = "disabled";

            PageSplit = CreatePageSplit(Total, PageSize, PageIndex, 10);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 生成分页页码
        /// </summary>
        /// <param name="total">总数</param>
        /// <param name="pagesize">页大小</param>
        /// <param name="index">当前页</param>
        /// <param name="length">返回页码个数</param>
        /// <returns></returns>
        public int[] CreatePageSplit(int total, int pagesize, int index, int length)
        {
            //总页数
            var pagetotal = total / pagesize;
            if (total % pagesize > 0) { pagetotal++; }
            //当前页校正
            if (index < 1) { index = 1; }
            if (index > pagetotal) { index = pagetotal; }

            var listSplit = new List<int>();

            var len = length;
            if (len % 2 == 0) { len++; }
            var half = len / 2;

            var start = index - half;
            var end = index + half;

            if (start < 1)
            {
                var p = 1 - start;
                start += p;
                end += p;
            }
            if (end > pagetotal)
            {
                var p = end - pagetotal;
                end = pagetotal;
                if (start - p >= 1) { start -= p; }
                else { start = 1; }
            }
            for (int i = start, j = 0; i <= end && j < length; i++, j++)
            {
                listSplit.Add(i);
            }
            return listSplit.ToArray();
        }
    }

    public class Previou
    {
        public string Disabled { get; set; } = "";
    }

    public class Next
    {
        public string Disabled { get; set; } = "";
    }
}