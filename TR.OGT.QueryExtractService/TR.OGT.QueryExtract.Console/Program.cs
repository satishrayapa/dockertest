using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TR.OGT.QueryExtract.Domain;
using TR.OGT.QueryExtract.Queue;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System;
using TR.OGT.QueryExtract.Infrastructure;
using TR.OGT.QueryExtract.Common;

namespace TR.OGT.QueryExtract.Console
{
    class Program
    {
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(Environment.ProcessorCount);

        static async Task Main(string[] args)
        {
            var serviceProvider = Startup.ServiceProvider;
            var sqsClient = serviceProvider.GetService<ISQSClient>();
            var manager = serviceProvider.GetService<IExtractService>();
            var logger = serviceProvider.GetService<ILogger<Program>>();

            while (true)
            {
                var messages = await sqsClient.GetMessages();

                if (!messages.Any())
                {
                    logger.LogInformation("No new messages;");
                    continue;
                }

                logger.LogInformation("Received {Count} messages.", messages.Count);

                await _semaphoreSlim.WaitAsync();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    var eventGroups = messages
                        .Select(m => (Message: m, Body: JsonConvert.DeserializeObject<SQSHsMessageBody>(m.Body)))
                        .GroupBy(g => g.Body.EventType);

                    foreach (var group in eventGroups)
                    {
                        Result result;
                        var ids = group.Select(i => i.Body.GuidChanged);

                        switch (group.Key)
                        {
                            case EventType.Insert:
                                result = await manager.HandleCreateEvent(ids);
                                continue;
                            case EventType.Update:
                                result = await manager.HandleUpdateEvent(ids);
                                break;
                            case EventType.Delete:
                                result = await manager.HandleDeleteEvent(ids);
                                break;
                        }

                        var messagesToPostProcess = group.Select(g => g.Message);
                        if (result.IsOk)
                        {
                            await sqsClient.AckMessages(messagesToPostProcess);
                        }
                        else
                        {
                            await sqsClient.ReleaseMessages(messagesToPostProcess);
                        }
                    }


                }).ContinueWith(async t =>
                {
                    if (t.IsFaulted)
                    {
                        logger.LogError(t.Exception.InnerException, "Main Thread.");
                        await sqsClient.ReleaseMessages(messages);
                    }

                    _semaphoreSlim.Release();
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }
    }
}
