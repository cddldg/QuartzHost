using Microsoft.AspNetCore.Components;
using QuartzHost.Contract.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuartzHost.UI.Common
{
    public static class HttpJsonExtensions
    {
        public static async Task<T> GetHttpJsonAsync<T>(this HttpClient Http, string requestUri, bool isAuth = true)
        {
            //using (httpClient){}
            if (isAuth) Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "d99bd1fb81754107ad7c070e3c0ab3a1");
            var content = await Http.GetStringAsync(requestUri);
            return content.ToObj<T>();
        }

        public static async Task<T> PostHttpAsync<T>(this HttpClient Http, string requestUri, object content = null, bool isAuth = true)
        {
            if (isAuth) Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "d99bd1fb81754107ad7c070e3c0ab3a1");
            return await Http.PostJsonAsync<T>(requestUri, content);
        }
    }
}