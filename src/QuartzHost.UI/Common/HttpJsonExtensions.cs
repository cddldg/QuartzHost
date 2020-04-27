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
        public const string TOKEN = "7e07d2b6c1444d6bb94c87547916b18d";
        public const string ApiHost = "http://localhost:60000/";

        public static async Task<T> GetHttpJsonAsync<T>(this HttpClient Http, string requestUri, bool isAuth = true)
        {
            //using (httpClient){}
            if (isAuth) Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN);
            var content = await Http.GetStringAsync($"{ApiHost}{requestUri}");
            return content.ToObj<T>();
        }

        public static async Task<T> PostHttpAsync<T>(this HttpClient Http, string requestUri, object content = null, bool isAuth = true)
        {
            if (isAuth) Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN);
            return await Http.PostJsonAsync<T>($"{ApiHost}{requestUri}", content);
        }
    }
}