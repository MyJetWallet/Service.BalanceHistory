using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using MyJetWallet.Sdk.GrpcMetrics;
using MyJetWallet.Sdk.GrpcSchema;
using MyJetWallet.Sdk.Postgres;
using OpenTelemetry.Trace;
using Prometheus;
using ProtoBuf.Grpc.Server;
using Service.BalanceHistory.Grpc;
using Service.BalanceHistory.Modules;
using Service.BalanceHistory.Postgres;
using Service.BalanceHistory.Services;
using SimpleTrading.BaseMetrics;
using SimpleTrading.ServiceStatusReporterConnector;

namespace Service.BalanceHistory
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCodeFirstGrpc(options =>
            {
                options.Interceptors.Add<PrometheusMetricsInterceptor>();
                options.BindMetricsInterceptors();
            });

            services.AddHostedService<ApplicationLifetimeManager>();

            services.AddDatabaseWithoutMigrations<DatabaseContext>(DatabaseContext.Schema, Program.Settings.PostgresConnectionString);

            if (!string.IsNullOrEmpty(Program.Settings.ZipkinUrl))
            {
                services.AddOpenTelemetryTracing((builder) => builder
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = context =>
                        {
                            if (context?.Request?.Path.Value?.Contains("metrics") == true) return false;
                            if (context?.Request?.Path.Value?.Contains("isalive") == true) return false;
                            if (context?.Request?.Path.Value?.Contains("metrics") == true) return false;

                            return true;
                        };
                    })
                    .AddHttpClientInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddGrpcCoreInstrumentation()
                    .AddZipkinExporter(options => { options.Endpoint = new Uri(Program.Settings.ZipkinUrl); })
                );
                Console.WriteLine($"+++ ZIPKIN is connected +++, {Program.Settings.ZipkinUrl}");
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMetricServer();

            app.BindServicesTree(Assembly.GetExecutingAssembly());

            app.BindIsAlive();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcSchema<WalletBalanceUpdateService, IWalletBalanceUpdateService>();

                endpoints.MapGrpcSchemaRegistry();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });

            
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();
        }
    }
}
