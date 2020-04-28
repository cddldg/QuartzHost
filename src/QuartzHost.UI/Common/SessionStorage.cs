using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using QuartzHost.Contract.Common;
using QuartzHost.Contract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.UI.Common
{
    public interface ISessionStorage
    {
        string GetId();

        Task SetItemAsync(string key, object data);

        Task<T> GetItemAsync<T>(string key);

        Task ClearAsync();

        Task RemoveItemAsync(string key);
    }

    public class SessionStorage : ISessionStorage
    {
        private readonly IJSRuntime _jSRuntime;
        private readonly IJSInProcessRuntime _jSInProcessRuntime;

        public static readonly string SessionId = Guid.NewGuid().ToString("n");

        public SessionStorage(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
            _jSInProcessRuntime = jSRuntime as IJSInProcessRuntime;
        }

        public string GetId() => SessionId;

        public async Task SetItemAsync(string key, object data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            await _jSRuntime.InvokeAsync<object>("sessionStorage.setItem", key, data.ToJson());
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var serialisedData = await _jSRuntime.InvokeAsync<string>("sessionStorage.getItem", key);

            if (serialisedData == null)
                return default(T);

            return serialisedData.ToObj<T>();
        }

        public async Task RemoveItemAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            await _jSRuntime.InvokeAsync<object>("sessionStorage.removeItem", key);
        }

        public async Task ClearAsync() => await _jSRuntime.InvokeAsync<object>("sessionStorage.clear");
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSessionStorage(this IServiceCollection services)
        {
            return services.AddScoped<ISessionStorage, SessionStorage>();
        }
    }
}