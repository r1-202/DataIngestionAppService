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
    string time{get;set;}
    string temperature{get;set;}
    string pressure{get;set;}
    string humidity{get;set;}
    string clouds{get;set;}
    string wind_speed{get;set;}
    string wind_direction{get;set;}
}

public class WeatherData
{
    //city is fixed to 33.8485° N, 35.8981° E (Zahle)
    public WeatherData(string time, float temperature, float pressure,
                        float humidity, float clouds, float wind_speed,
                        float wind_direction, string season)
    {
        this.time = time;
        this.temperature = temperature;
        this.pressure = pressure;
        this.humidity = humidity;
        this.clouds = clouds;
        this.wind_speed = wind_speed;
        this.wind_direction = wind_direction;
    }
    public string time{get;set;}
    public float temperature{get;set;}
    public float pressure{get;set;}
    public float humidity{get;set;}
    public float clouds{get;set;}
    public float wind_speed{get;set;}
    public float wind_direction{get;set;}
    public string season{get;set;}
    public Units units;


    public static async Task<List<WeatherData>?> requestData(string start_d, string end_d)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://archive-api.open-meteo.com");
        var response = await client.GetAsync(
            $"/v1/archive?latitude=33.8485&longitude=35.8981"+
            $"&start_date={start_d}&end_date={end_d}&"+
            "hourly=temperature_2m,relative_humidity_2m,surface_pressure,"+
            "cloud_cover,wind_speed_10m,wind_direction_10m");
        string json_response = await response.Content.ReadAsStringAsync();

        if(json_response==null) return null;

        JsonNode weather_node = JsonNode.Parse(json_response)!;
        
        if(weather_node["weather"]==null) return null;

        city_name = weather_node["name"]!.GetValue<string>();
        country_ID = weather_node["sys"]!["country"]!.GetValue<string>();
        weather_description = weather_node["weather"]!.AsArray()[0]!
        ["description"]!.GetValue<string>();
        temperature = weather_node["main"]!["temp"]!.GetValue<float>();

        return new WeatherData(city_name, country_ID, weather_description, temperature);
    }
}