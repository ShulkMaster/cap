using System.Globalization;
using System.Text;
using System.Text.Json;
using Cap.Contract.Request;
using Cap.Data;
using Cap.Filter;
using Microsoft.AspNetCore.Mvc;
using Cap.Models;
using Cap.Utils;
using GenericParsing;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Cap.Controllers;

[ApiController]
[Route("[controller]")]
[ServiceFilter(typeof(ApiKeyAuthFilter))]
public class QuakeController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly QuakeContext _db;

    public QuakeController(ILogger<QuakeController> logger, QuakeContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public async Task<List<Quake>> Index([FromQuery] QuakeFilter filter)
    {
        IQueryable<Quake> quakes = _db.Quakes;
        var filterer = new QuakeFilterer(filter);
        quakes = filterer.Apply(quakes);

        quakes = filterer.Sort(quakes);
        if (filter.Size != 0 && filter.Page != 0)
        {
            quakes = quakes.Take(filter.Size)
                .Skip(filter.Size * (filter.Page - 1));
        }

        var res = await quakes.ToListAsync();
        return res;
    }

    [HttpPost("/update")]
    public async Task<int> Update()
    {
        StreamReader sRead = System.IO.File.OpenText("data.csv");
        var parser = new GenericParser(sRead);
        var quakes = new List<Quake>();
        parser.FirstRowHasHeader = true;
        
        while (parser.Read())
        {
            if (parser.ColumnCount < 5)
            {
                _logger.LogWarning("Skipping line");
                continue;
            }

            var quake = new Quake();
            string date = parser[0].Replace(",", "").ToLower();
            string fixedDay = date[..3] + "." + date[3..];
            string timeFix = parser[1].Replace("AM", "a. m.")
                .Replace("PM", "p. m.")
                .ToLower();
            TimeZoneInfo elSalvadorTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
            try
            {
                string fDate = $"{fixedDay} {timeFix}".Trim();
                DateTime dd = DateTime.ParseExact(
                    fDate,
                    "MMM dd yyyy h:mm:ss tt",
                    new CultureInfo("es-SV")
                );
                DateTime elSalvadorDateTime = TimeZoneInfo.ConvertTime(dd, elSalvadorTimeZone);
                quake.Date = elSalvadorDateTime.ToUniversalTime();
                quake.Latitude = double.Parse(parser[2]);
                quake.Longitude = double.Parse(parser[3]);
                byte[] bytes = Encoding.Latin1.GetBytes(parser[4]);
                quake.Location = Encoding.UTF8.GetString(bytes);
                quake.Depth = double.Parse(parser[5]);
                quake.Magnitude = double.Parse(parser[6]);
                quake.Intensity = CountIsWithAverage(parser[7]);
                bytes = Encoding.Latin1.GetBytes(parser[7]);
                quake.IntensityDescription = Encoding.UTF8.GetString(bytes);
                var point = new Point(quake.Longitude, quake.Latitude)
                {
                    SRID = 4326
                };
                quake.Geom = point;
                quakes.Add(quake);
            }
            catch (Exception e)
            {
                _logger.LogError("{message}", e.Message);
            }
        }

        _db.Quakes.AddRange(quakes);
        return await _db.SaveChangesAsync();
    }

    private static float CountIsWithAverage(string input)
    {
        int count = 0;
        int total = 0;
        int groupCount = 0;

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == 'I')
            {
                count++;
            }
            else if (i > 0 && input[i - 1] == 'I' && input[i] == '-')
            {
                groupCount++;
                total += count;
                count = 0;
            }
        }

        if (count > 0)
        {
            total += count;
            groupCount++;
        }

        return groupCount == 0 ? 0 : total / (float)groupCount;
    }
}