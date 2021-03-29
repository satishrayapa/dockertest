using Amazon.DynamoDBv2;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Steeltoe.Extensions.Configuration.ConfigServer;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

using TR.OGT.ChangeLedger.Common;
using TR.OGT.ChangeLedger.Data.Dao;
using TR.OGT.ChangeLedger.Data.Dao.Abstractions;
using TR.OGT.ChangeLedger.Data.Interfaces;
using TR.OGT.ChangeLedger.Data.Repositories;
using TR.OGT.ChangeLedger.Domain.Options;
using TR.OGT.ChangeLedger.Infrastructure.Interfaces;
using TR.OGT.ChangeLedger.Infrastructure.Services;
using TR.OGT.Common.Configuration;

namespace TR.OGT.ChangeLedger.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            var manager = host.Services.GetRequiredService<IChangeManager>();
            await manager.StartChangeLedger(CancellationToken.None);
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    var applicationEnvironment = new ApplicationEnvironment();

                    config
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: false);

                    if (!applicationEnvironment.IsCloud)
                        return;

                    const string ConfigsToGet = "dbconfig, logconfig, appconfig";

                    var configServerClientSettings = new ConfigServerClientSettings
                    {
                        Environment = $"{applicationEnvironment.Name},{ConfigsToGet}",
                        Name = applicationEnvironment.ApplicationName,
                        Uri = applicationEnvironment.ConfigServerLocation
                    };

                    config.AddConfigServer(configServerClientSettings);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDefaultAWSOptions(context.Configuration.GetAWSOptions(nameof(AWSConfig)));
                    services.AddAWSService<IAmazonSQS>();
                    services.AddAWSService<IAmazonDynamoDB>();

                    services
                        .AddSingleton<IChangeManager, ChangeManager>()
                        .AddScoped<IQueueProducer, AmazonSQSQueueProducer>()
                        .AddScoped<IVersionRepository, VersionRepository>()
                        .AddScoped<IContentUpdateRepository, ContentUpdateRepository>()
                        .AddSingleton<ParentEntityDao, DetailDao>()
                        .AddSingleton<IReadOnlyCollection<IChildEntityDao>>(sp => sp.GetServices<IChildEntityDao>().ToImmutableArray())
                        .AddSingleton<IChildEntityDao, DescriptionDao>()
                        .AddSingleton<IChildEntityDao, RelatedControlsDao>()
                        .AddSingleton<IChildEntityDao, ChargeQuotaDao>()
                        .AddSingleton<IChildEntityDao, ChargeQuotaMapDao>()
                        .AddSingleton<IChildEntityDao, NotesDao>()
                        .AddSingleton<IControlsRepository, ControlsRepository>()
                        .AddSingleton<IControlEntityDao, DetailControlDao>()
                        .AddSingleton<IControlEntityDao, DetailControlMapDao>()
                        .AddSingleton<IControlEntityDao, DocumentControlMap>()
                        .AddSingleton<IControlAgenciesRepository, ControlAgenciesRepository>()
                        .AddSingleton<IControlAgencyEntityDao, AgencyDao>()
                        .AddSingleton<IControlAgencyEntityDao, AgencyDescription>()
                        .AddSingleton<IControlAgencyEntityDao, AgencyMapDao>()
                        .AddSingleton<IRulingsRepository, RulingsRepository>()
                        .AddSingleton<IRulingEntityDao, RulingsDao>()
                        .AddSingleton<IRulingEntityDao, RulingsDescriptionDao>()
                        .AddSingleton<IRulingEntityDao, RulingsTypeDao>()
                        .AddSingleton<IRulingEntityDao, RulingsMapDao>()
                        .AddSingleton<IVersionsDynamoDbDao, VersionsDynamoDbDao>()
                        .AddSingleton<IVersionsSqlDao, VersionsSqlDao>();

                    services.AddOptions<AWSConfig>().BindConfiguration(nameof(AWSConfig));
                    services.AddOptions<ContentDbConfig>().BindConfiguration(nameof(ContentDbConfig));
                })
                .ConfigureLogging((context, logging) => {
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                        logging.AddEventSourceLogger();
                    }

                    logging.Services.AddSingleton(typeof(ILogger<>), typeof(TRLogger<>));
                    logging.Services.AddOptions<LogConfig>().BindConfiguration(nameof(LogConfig));
                });
                    
    }
}
