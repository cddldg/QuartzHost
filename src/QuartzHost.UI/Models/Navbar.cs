using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.UI.Models
{
    /// <summary>
    /// 导航栏
    /// </summary>
    public class Navbar
    {
        public BarItem BarItem { get; set; }
    }

    /// <summary>
    /// 侧边栏
    /// </summary>
    public class Sidebar
    {
        public List<SideBarItem> BarItems { get; set; }
    }

    public class BarItem
    {
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 样式名称
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; }
    }

    public class SideBarItem : BarItem
    {
    }
}