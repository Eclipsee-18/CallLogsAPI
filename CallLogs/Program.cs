using CallLogs.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.AddAutoMapper(typeof(Program), typeof(MappingProfiles));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
	{
		Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
		In = ParameterLocation.Header,
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey

	});
	options.OperationFilter<SecurityRequirementsOperationFilter>();
});


builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
	builder.Configuration.GetConnectionString("DefaultConnection")
	));
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
		options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
				ValidateIssuer = false,
				ValidateAudience = false,
			};
		}
	);

var myCors = "appCors";
builder.Services.AddCors(options =>
{
	options.AddPolicy(myCors, policy =>
	{
		//policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod();
		policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
	});
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
