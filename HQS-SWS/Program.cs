using HQS_SWS;
using Microsoft.EntityFrameworkCore;

// var builder = Host.CreateApplicationBuilder(args);
// builder.Services.AddHostedService<Worker>();

// var host = builder.Build();
// host.Run();



IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(
                            context.Configuration.GetConnectionString("dc")
                        ));
                    services.AddHostedService<Worker>();
                })
                .Build();

await host.RunAsync();