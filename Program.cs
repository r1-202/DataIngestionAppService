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

app.MapGet("/getweather/{start_date}/{end_date}", async (string start_date, string end_date) => 
{
    List<WeatherData>? weather_data_array = await WeatherData.requestData(start_date, end_date);

    if(weather_data_array == null)
    {
        return Results.NotFound(new {cod = 404, error = "not found"});
    }
    else
    {
        return Results.Ok(new {units = WeatherData.units, data = weather_data_array});
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();