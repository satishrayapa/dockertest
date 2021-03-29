using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using TR.OGT.QueryExtract.Data;
using TR.OGT.QueryExtract.Queue;
using TR.OGT.QueryExtract.Domain;
using TR.OGT.QueryExtract.Infrastructure;
using TR.OGT.QueryExtract.Infrastructure.Sql;
using Amazon.SQS;
using TR.OGT.Common.ServiceLogging;
using TR.OGT.Common.Configuration;
using Steeltoe.Extensions.Configuration.ConfigServer;

namespace TR.OGT.QueryExtract.Console
{
    public static class Startup
    {
        public static IConfigurationRoot ConfigurationRoot { get; }
        public static IServiceProvider ServiceProvider { get; }

        static Startup()
        {
            var applicationEnvironment = new ApplicationEnvironment();

            ConfigurationRoot = applicationEnvironment.IsCloud
                ? CloudConfiguration(applicationEnvironment)
                : LocalConfiguration();

            ServiceProvider = SetupServiceCollection(new ServiceCollection()).BuildServiceProvider();
        }

        static IConfigurationRoot CloudConfiguration(ApplicationEnvironment applicationEnvironment)
        {
            const string ConfigsToGet = "dbconfig, logconfig, appconfig";

            var configServerClientSettings = new ConfigServerClientSettings
            {
                Environment = $"{applicationEnvironment.Name},{ConfigsToGet}",
                Name = applicationEnvironment.ApplicationName,
                Uri = applicationEnvironment.ConfigServerLocation
            };

            return new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false)
                    .AddConfigServer(configServerClientSettings)
                    .Build();
        }

        static IConfigurationRoot LocalConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .Build();
        }

        static IServiceCollection SetupServiceCollection(IServiceCollection services)
        {
            services.AddDefaultAWSOptions(ConfigurationRoot.GetAWSOptions("sqs"));
            services.AddAWSService<IAmazonSQS>();

            services
                .AddTRLogging()
                .AddSingleton<IConfiguration>(ConfigurationRoot)
                .AddSingleton(typeof(ISQSClient), typeof(SQSClient))
                .AddSingleton(s => ElasticClientFactory.Create(s.GetService<ElasticConfig>()))
                .AddSingleton(typeof(ITempDataDao), typeof(TempDataDao))
                .AddSingleton(typeof(IHSGenericDao<HSDetailsSqlDto>), typeof(ProductClassificationDao))
                .AddSingleton(typeof(IHSGenericDao<HSDescriptionSqlDto>), typeof(ProductClassificationDescriptionDao))
                .AddSingleton(typeof(IHSGenericDao<RelatedHSSqlDto>), typeof(RelatedControlsDao))
                .AddSingleton(typeof(IHSGenericDao<ChargeQuotaSqlDto>), typeof(ChargeQuotaDao))
                .AddSingleton(typeof(IHSGenericDao<PcNoteSqlDto>), typeof(PcNoteDao))
                .AddSingleton(typeof(IBindingRullingsDao), typeof(BindingRullingsDao))
                .AddSingleton(typeof(IHsDetailsElasticRepository), typeof(HsDetailsElasticRepository))
                .AddSingleton(typeof(IHsDetailsSqlRepository), typeof(HsDetailsSqlRepository))
                .AddSingleton(typeof(IExtractService), typeof(ExtractService));

            services.AddSingleton(s => s.GetService<IConfiguration>().GetSection("sqs").Get<SQSConfig>());
            services.AddSingleton(s => s.GetService<IConfiguration>().GetSection("elastic").Get<ElasticConfig>());

            return services;
        }
    }
}
