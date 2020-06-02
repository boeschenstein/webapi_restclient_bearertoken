using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAPIBase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get(int rangeStart, int rangeEnd)
        {
            // curl -X GET "https://localhost:5001/TestArguments?rangeStart=1&rangeEnd=4" -H "accept: text/plain"

            if (rangeStart == 0) rangeStart = 1;
            if (rangeEnd == 0) rangeEnd = 5;

            var rng = new Random();
            var ret = Enumerable.Range(rangeStart, rangeEnd).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = "get_" + Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
            return ret;
        }

        // [Authorize] // roles not checked: access
        [Authorize(Roles = "TestRole1")] // role found in token: access
        // [Authorize(Roles = "Test!Role1")] // role not in token: no access
        [HttpPost]
        public IEnumerable<WeatherForecast> Post(int rangeStart, int rangeEnd)
        {
            // curl -X POST "https://localhost:5001/TestArguments?rangeStart=3&rangeEnd=6" -H "accept: text/plain"

            if (rangeStart == 0) rangeStart = 1;
            if (rangeEnd == 0) rangeEnd = 5;

            var rng = new Random();
            var ret = Enumerable.Range(rangeStart, rangeEnd).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = "post_" + Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
            return ret;
        }

        //[HttpPut]
        //public IEnumerable<WeatherForecast> Put(int rangeStart, int rangeEnd)
        //{
        //    if (rangeStart == 0) rangeStart = 1;
        //    if (rangeEnd == 0) rangeEnd = 5;

        //    var rng = new Random();
        //    var ret = Enumerable.Range(rangeStart, rangeEnd).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = "put_" + Summaries[rng.Next(Summaries.Length)]
        //    })
        //   .ToArray();
        //    return ret;
        //}

        [HttpPut]
        public IEnumerable<WeatherForecast> PutWithArgumentsClass(PutArguments arguments)
        {
            if (arguments.RangeStart == 0) arguments.RangeStart = 1;
            if (arguments.RangeEnd == 0) arguments.RangeEnd = 5;

            var rng = new Random();
            var ret = Enumerable.Range(arguments.RangeStart, arguments.RangeEnd).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = "put_" + Summaries[rng.Next(Summaries.Length)]
            })
           .ToArray();
            return ret;
        }
    }
}