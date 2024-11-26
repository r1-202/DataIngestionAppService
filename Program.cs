var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });;
});;
var app = builder.Build();

StreamReader sr = new StreamReader("API_KEY.txt");
string API_KEY = sr.ReadLine()!;

app.MapGet("/getweather/{city_name}", async (string city_name) => 
{
    WeatherData? weather_data = await WeatherData.requestData(city_name, API_KEY);

    if(weather_data == null)
    {
        return Results.NotFound(new {cod = 404, error = "city not found"});
    }
    else
    {
        return Results.Ok(weather_data);
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();