using System.Text.Json.Nodes;

public class Units
{
    public Units(string time, string temperature, string pressure,
    string humidity, string clouds, string wind_speed, string wind_direction)
    {
        this.time = time;
        this.temperature = temperature;
        this.pressure = pressure;
        this.humidity = humidity;
        this.clouds = clouds;
        this.wind_speed = wind_speed;
        this.wind_direction = wind_direction;
    }
    public string time { get; set; }
    public string temperature { get; set; }
    public string pressure { get; set; }
    public string humidity { get; set; }
    public string clouds { get; set; }
    public string wind_speed { get; set; }
    public string wind_direction { get; set; }
}

public class WeatherData
{
    //city is fixed to 33.8485° N, 35.8981° E (Zahle)
    public WeatherData(string time, float temperature, float pressure,
                        float humidity, float clouds, float wind_speed,
                        float wind_direction)
    {
        this.time = time;
        this.temperature = temperature;
        this.pressure = pressure;
        this.humidity = humidity;
        this.clouds = clouds;
        this.wind_speed = wind_speed;
        this.wind_direction = wind_direction;
        this.season = getSeason(time);
    }
    public string time { get; set; }
    public float temperature { get; set; }
    public float pressure { get; set; }
    public float humidity { get; set; }
    public float clouds { get; set; }
    public float wind_speed { get; set; }
    public float wind_direction { get; set; }
    public string season { get; set; }
    public static Units? units;

    public static string getSeason(string dt)
    {
        DateTime date = DateTime.Parse(dt);
        int year = date.Year;

        // Define the start dates of seasons
        DateTime springStart = new DateTime(year, 3, 21);
        DateTime summerStart = new DateTime(year, 6, 21);
        DateTime autumnStart = new DateTime(year, 9, 23);
        DateTime winterStart = new DateTime(year, 12, 21);

        if (date >= springStart && date < summerStart)
            return "Spring";
        else if (date >= summerStart && date < autumnStart)
            return "Summer";
        else if (date >= autumnStart && date < winterStart)
            return "Autumn";
        else
            return "Winter";
    }
    public static async Task<List<WeatherData>?> requestData(string start_d, string end_d)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://archive-api.open-meteo.com");
        var response = await client.GetAsync(
            $"/v1/archive?latitude=33.8485&longitude=35.8981" +
            $"&start_date={start_d}&end_date={end_d}&" +
            "hourly=temperature_2m,relative_humidity_2m,surface_pressure," +
            "cloud_cover,wind_speed_10m,wind_direction_10m");
        string json_response = await response.Content.ReadAsStringAsync();

        if (json_response == null) return null;

        JsonNode weather_node = JsonNode.Parse(json_response)!;

        if (weather_node["hourly"] == null) return null;

        JsonNode hourly_units = weather_node["hourly_units"]!;

        units = new Units(hourly_units["time"]!.GetValue<string>(),
        hourly_units["temperature_2m"]!.GetValue<string>(),
        hourly_units["surface_pressure"]!.GetValue<string>(),
         hourly_units["relative_humidity_2m"]!.GetValue<string>(),
        hourly_units["cloud_cover"]!.GetValue<string>(),
        hourly_units["wind_speed_10m"]!.GetValue<string>(),
        hourly_units["wind_direction_10m"]!.GetValue<string>());

        

        var time_array = weather_node["hourly"]!["time"]!.AsArray();
        var temperature_array = weather_node["hourly"]!["temperature_2m"]!.AsArray();
        var pressure_array = weather_node["hourly"]!["surface_pressure"]!.AsArray();
        var humidity_array = weather_node["hourly"]!["relative_humidity_2m"]!.AsArray();
        var clouds_array = weather_node["hourly"]!["cloud_cover"]!.AsArray();
        var wind_speed_array = weather_node["hourly"]!["wind_speed_10m"]!.AsArray();
        var wind_direction_array = weather_node["hourly"]!["wind_direction_10m"]!.AsArray();

        List<WeatherData> list = new List<WeatherData>();

        for (int i = 0; i < time_array.Count(); ++i)
        {
            list.Add(new WeatherData(
                time_array[i]!.GetValue<string>(),
                temperature_array[i]!.GetValue<float>(),
                pressure_array[i]!.GetValue<float>(),
                humidity_array[i]!.GetValue<float>(),
                clouds_array[i]!.GetValue<float>(),
                wind_speed_array[i]!.GetValue<float>(),
                wind_direction_array[i]!.GetValue<float>()
            ));
        }

        return list;
    }
}