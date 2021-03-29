using Elasticsearch.Net;
using Nest;
using System;
using TR.OGT.QueryExtract.Infrastructure.Elastic;

namespace TR.OGT.QueryExtract.Data
{
    public static class ElasticClientFactory
    {
        /// <summary>
        /// Creates instance of IElasticClient
        /// </summary>
        /// <returns>Instance of <see cref="IElasticClient"/></returns>
        public static IElasticClient Create(ElasticConfig elasticConfig)
        {
            if (elasticConfig == null)
            {
                throw new ArgumentNullException(nameof(elasticConfig));
            }

            var connectionPool = new SingleNodeConnectionPool(new Uri(elasticConfig.NodeUrl));
            var settings = new ConnectionSettings(connectionPool)
                .DefaultIndex(elasticConfig.HsDetailsIndexName);

            var client = new ElasticClient(settings);

            if (!client.Indices.Exists(elasticConfig.HsDetailsIndexName).Exists)
            {
                client.Indices.Create(elasticConfig.HsDetailsIndexName, c => c.Map<HsDetailsElasticDto>(m => m.AutoMap()));
            }
            else
            {
                client.Indices.PutMapping<HsDetailsElasticDto>(md => md.AutoMap());
            }

            return client;
        }
    }
}
