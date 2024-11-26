using System.Text.Json.Nodes;

public class WeatherData
{
    public WeatherData(string city_name, string country_ID, 
    string weather_description, float temperature)
    {
        this.city_name=city_name;
        this.country_ID=country_ID;
        this.weather_description=weather_description;
        this.temperature=temperature;
    }
    public string city_name{get;set;}
    public string country_ID{get;set;}
    public string weather_description{get;set;}
    public float temperature{get;set;}

    public static async Task<WeatherData?> requestData(string city_name, string API_KEY)
    {
        string country_ID="";
        string weather_description="";
        float temperature=0;

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("http://api.openweathermap.org");
        var response = await client.GetAsync(
            $"/data/2.5/weather?q={city_name}&appid={API_KEY}&units=metric");
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