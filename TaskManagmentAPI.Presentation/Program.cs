using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;
using TaskManagement.Core.Interfaces.IRepositories;
using TaskManagement.Core.Interfaces.IServices;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.Services;
using TaskManagement.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration()
		.ReadFrom.Configuration(builder.Configuration, "Serilog")
		.Enrich.FromLogContext()
		.MinimumLevel.Information()
		.WriteTo.Console()
		.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
		//.WriteTo.Seq(builder.Configuration["SEQ_URL"] ?? "http://localhost:5341")  // Use SEQ_URL env var in Heroku
		.CreateLogger();

try
{
	Log.Information("Starting up");

	builder.Services.Configure<MongoDBSettings>(options =>
	{
		options.ConnectionString = builder.Configuration["MongoDBSettings:ConnectionString"]!;
		options.DatabaseName = builder.Configuration["MongoDBSettings:DatabaseName"]!;
	});

	builder.Services.Configure<RedisSettings>(options =>
	{
		options.ConnectionString = builder.Configuration["RedisSettings:ConnectionString"]!;
		options.SlidingExpiration = TimeSpan.Parse(builder.Configuration["RedisSettings:SlidingExpiration"]!);
		options.AbsoluteExpiration = TimeSpan.Parse(builder.Configuration["RedisSettings:AbsoluteExpiration"]!);
	});

	builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
	{
		var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
		return new MongoClient(settings.ConnectionString);
	});

	builder.Services.AddSingleton(sp =>
	{
		var client = sp.GetRequiredService<IMongoClient>();
		var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
		return new MongoDbContext(client, settings.DatabaseName);
	});

	builder.Services.AddCors(options =>
	{
		options.AddPolicy("AllowReactApp",
				policy => policy
						.WithOrigins("http://localhost:3000")
						.AllowAnyMethod()
						.AllowAnyHeader());
	});

	builder.Services.AddScoped<IUserRepository, UserRepository>();
	builder.Services.AddScoped<IListRepository, ListRepository>();
	builder.Services.AddScoped<IGroupRepository, GroupRepository>();
	builder.Services.AddScoped<ITaskRepository, TaskRepository>();

	builder.Services.AddScoped<IUserService, UserService>();
	builder.Services.AddScoped<IListService, ListService>();
	builder.Services.AddScoped<IGroupService, GroupService>();
	builder.Services.AddScoped<ITaskService, TaskService>();

	builder.Services.AddStackExchangeRedisCache(options =>
	{
		var redisSettings = builder.Configuration.GetSection("RedisSettings").Get<RedisSettings>();
		if (redisSettings != null && !string.IsNullOrEmpty(redisSettings.ConnectionString))
		{
			options.Configuration = redisSettings.ConnectionString;
			options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
			{
				AbortOnConnectFail = true,
				EndPoints = { redisSettings.ConnectionString }
			};
		}
	});

	builder.Services.AddControllers();
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	builder.Services.AddScoped<DatabaseSeeder>();

	var app = builder.Build();

	using (var scope = app.Services.CreateScope())
	{
		var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
		await seeder.SeedAsync();
	}

	if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseCors("AllowReactApp");
	app.UseHttpsRedirection();
	app.UseAuthorization();
	app.MapControllers();

	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application start-up failed");
}
finally
{
	Log.CloseAndFlush();
}
