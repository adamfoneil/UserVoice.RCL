using GitHubApiClient;
using GitHubApiClient.Models;
using Radzen;
using UserVoice.RCL.Service;
using UserVoice.RCL.Service.Interfaces;
using UserVoice.RCL.Service.Models;
using UserVoice.Service;
using UserVoice.Service.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("github.json", optional: true);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddScoped<UserVoiceDataContext>();
builder.Services.Configure<GitHubIssueImportOptions>(builder.Configuration.GetSection("GitHubIssueImporter"));
builder.Services.Configure<Settings>(builder.Configuration.GetSection("GitHubApi"));
builder.Services.AddScoped<GitHubClient>();
builder.Services.AddScoped<IIssueImporter, GitHubIssueImporter>();
builder.Services.AddScoped<DialogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
