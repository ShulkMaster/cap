using System.Globalization;
using System.Text;
using Cap.Data;
using Microsoft.AspNetCore.Mvc;
using Cap.Models;
using GenericParsing;
using Microsoft.EntityFrameworkCore;

namespace Cap.Controllers;

[ApiController]
[Route("[controller]")]
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
    public async Task<List<Quake>> Index([FromQuery] int limit)
    {
        int fixedLimit = Math.Min(100, Math.Abs(limit));
        var res =  await _db.Quakes.Take(fixedLimit).ToListAsync();
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
                quakes.Add(quake);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
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