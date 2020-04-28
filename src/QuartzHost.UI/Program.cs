using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using QuartzHost.UI.Common;

namespace QuartzHost.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddSessionStorage();
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}