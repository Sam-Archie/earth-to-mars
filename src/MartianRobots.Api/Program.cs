using MartianRobots.Api;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapPost("/missions", MissionsEndpoint.RunMission);
app.Run();
