using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddHttpClient("LMStudio", client => {
    client.BaseAddress = new Uri("http://localhost:1234"); // LM Studio API URL
});


builder.Services.Configure<FormOptions>(o => {
    o.MultipartBodyLengthLimit = 50L * 1024 * 1024; // 50 MB
});

var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();
app.MapDefaultControllerRoute();
app.Run();
