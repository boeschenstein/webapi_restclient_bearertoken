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
        private static readonly HttpClient clientGet = new HttpClient();
        private static readonly HttpClient clientPost = clientGet;
        private static readonly HttpClient clientPut = clientGet;
        //private static readonly HttpClient clientPost = new HttpClient();
        //private static readonly HttpClient clientPut = new HttpClient();

        public async Task GetDataAsync(string token)
        {
            //const string uri = "http://localhost:40274/WeatherForecast?rangeStart=1&rangeEnd=4";

            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

            //client.DefaultRequestHeaders.Add("User-Agent", "My Test Client ");

            clientGet.DefaultRequestHeaders.Add("Authentication", $"Bearer {token}");

            // GET

            UriBuilder builderGet = new UriBuilder("http://localhost:5000/WeatherForecast");
            builderGet.Query = "rangeStart=1&rangeEnd=3";
            var responseGet = await clientGet.GetAsync(builderGet.Uri);
            responseGet.EnsureSuccessStatusCode();
            //string responseBodyGet = await responseGet.Content.ReadAsStringAsync();
            using var responseStreamGet = await responseGet.Content.ReadAsStreamAsync();
            var responseBodyGet = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(responseStreamGet);
            Log("responseBodyGet", responseBodyGet);

            // POST

            UriBuilder builderPost = new UriBuilder("http://localhost:5000/WeatherForecast");
            builderPost.Query = "rangeStart=1&rangeEnd=4";
            var responsePost = await clientPost.PostAsync(builderPost.Uri, null);
            responsePost.EnsureSuccessStatusCode();
            //string responseBodyPost = await responsePost.Content.ReadAsStringAsync();
            using var responseStreamPost = await responsePost.Content.ReadAsStreamAsync();
            var responseBodyPost = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(responseStreamPost);
            Log("responseBodyPost", responseBodyPost);

            // PUT

            UriBuilder builderPut = new UriBuilder("http://localhost:5000/WeatherForecast");
            //builderPut.Query = "rangeStart=1&rangeEnd=6";
            //var responsePut = await clientPut.PutAsync(builderPut.Uri, null);
            ////responsePut.EnsureSuccessStatusCode();
            ////string responseBodyPut = await responsePut.Content.ReadAsStringAsync();
            //using var responseStreamPut = await responsePut.Content.ReadAsStreamAsync();
            //var responseBodyPut = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(responseStreamPut);
            //Log("responseBodyPut", responseBodyPut);

            // PUT withArgumentsClass

            UriBuilder builderPutWithArgumentsClass = new UriBuilder("http://localhost:5000/WeatherForecast");

            var args = new WebAPIBase.Controllers.PutArguments { RangeEnd = 2, RangeStart = 3 };
            var argsJson = JsonSerializer.Serialize(args);
            var content = new StringContent(argsJson.ToString(), Encoding.UTF8, "application/json");
            var responsePutWithArgumentsClass = await clientPut.PutAsync(builderPut.Uri, content);
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