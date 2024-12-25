using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using SampleElasticApi.Configurations;
using SampleElasticApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var elasticSettings = builder.Configuration.GetSection(nameof(ElasticSettings)).Get<ElasticSettings>();
var connectionSettings = new ElasticsearchClientSettings(new Uri(elasticSettings.Url))
    /*.Authentication(new BasicAuthentication(elasticSettings.UserName, elasticSettings.Password))*/;

builder.Services.AddSingleton(new ElasticsearchClient(connectionSettings));
 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
