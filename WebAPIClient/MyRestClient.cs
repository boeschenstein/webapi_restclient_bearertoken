using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPIBase;

namespace WebAPIClient
{
    public class MyRestClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task GetDataAsync(string token)
        {
            //const string uri = "http://localhost:40274/WeatherForecast?rangeStart=1&rangeEnd=4";

            httpClient.DefaultRequestHeaders.Accept.Clear();
            // clientGet.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // GET

            UriBuilder builderGet = new UriBuilder("https://localhost:5001/WeatherForecast");
            builderGet.Query = "rangeStart=1&rangeEnd=3";
            var responseGet = await httpClient.GetAsync(builderGet.Uri);
            responseGet.EnsureSuccessStatusCode();
            //string responseBodyGet = await responseGet.Content.ReadAsStringAsync();
            using var responseStreamGet = await responseGet.Content.ReadAsStreamAsync();
            var responseBodyGet = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(responseStreamGet);
            Log("responseBodyGet", responseBodyGet);

            // POST

            UriBuilder builderPost = new UriBuilder("https://localhost:5001/WeatherForecast");
            builderPost.Query = "rangeStart=1&rangeEnd=4";

            var responsePost = await httpClient.PostAsync(builderPost.Uri, null);
            responsePost.EnsureSuccessStatusCode();
            //string responseBodyPost = await responsePost.Content.ReadAsStringAsync();
            using var responseStreamPost = await responsePost.Content.ReadAsStreamAsync();
            var responseBodyPost = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(responseStreamPost);
            Log("responseBodyPost", responseBodyPost);

            // PUT

            UriBuilder builderPut = new UriBuilder("https://localhost:5001/WeatherForecast");
            //builderPut.Query = "rangeStart=1&rangeEnd=6";
            //var responsePut = await clientPut.PutAsync(builderPut.Uri, null);
            ////responsePut.EnsureSuccessStatusCode();
            ////string responseBodyPut = await responsePut.Content.ReadAsStringAsync();
            //using var responseStreamPut = await responsePut.Content.ReadAsStreamAsync();
            //var responseBodyPut = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(responseStreamPut);
            //Log("responseBodyPut", responseBodyPut);

            // PUT withArgumentsClass

            UriBuilder builderPutWithArgumentsClass = new UriBuilder("https://localhost:5001/WeatherForecast");

            var args = new WebAPIBase.Controllers.PutArguments { RangeEnd = 2, RangeStart = 3 };
            var argsJson = JsonSerializer.Serialize(args);
            var content = new StringContent(argsJson.ToString(), Encoding.UTF8, "application/json");
            var responsePutWithArgumentsClass = await httpClient.PutAsync(builderPut.Uri, content);
            responsePutWithArgumentsClass.EnsureSuccessStatusCode();
            // string responseBodyPut = await responsePut.Content.ReadAsStringAsync();
            using var responseStreamPutWithArgumentsClass = await responsePutWithArgumentsClass.Content.ReadAsStreamAsync();
            var responseBodyPutWithArgumentsClass = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(responseStreamPutWithArgumentsClass);
            Log("responseBodyPutWithArgumentsClass", responseBodyPutWithArgumentsClass);
        }

        private void Log(string title, IEnumerable<WeatherForecast> items)
        {
            Console.WriteLine(title);
            foreach (var item in items)
            {
                Console.WriteLine($"    {item.Date}\t{item.TemperatureC}\t{item.TemperatureF}\t{item.Summary}\t");
            }
        }
    }
}