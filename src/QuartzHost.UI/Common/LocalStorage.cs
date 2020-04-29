using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using QuartzHost.Contract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzHost.UI.Common
{
    public interface ILocalStorage
    {
        Task SetItemAsync(string key, object data);

        Task<T> GetItemAsync<T>(string key);

        Task ClearAsync();

        Task RemoveItemAsync(string key);
    }

    public class LocalStorage : ILocalStorage
    {
        private readonly IJSRuntime _jSRuntime;

        public LocalStorage(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async Task SetItemAsync(string key, object data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            await _jSRuntime.InvokeAsync<object>("localStorage.setItem", key, data.ToJson());
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var serialisedData = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", key);

            if (serialisedData == null)
                return default(T);

            return serialisedData.ToObj<T>();
        }

        public async Task RemoveItemAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            await _jSRuntime.InvokeAsync<object>("localStorage.removeItem", key);
        }

        public async Task ClearAsync() => await _jSRuntime.InvokeAsync<object>("localStorage.clear");
    }

    public static class LocalStorageServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalStorage(this IServiceCollection services)
        {
            return services.AddScoped<ILocalStorage, LocalStorage>();
        }
    }
}