using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MJR047.FunctionApp1
{
    public static class SayHelloOrchestrationTrigger
    {
        [FunctionName(nameof(SayHelloOrchestrationTrigger))]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log,
            [Queue("sayhello-poison")] ICollector<string> errors)
        {
            log.LogInformation($"{nameof(SayHelloOrchestrationTrigger)} trigger function processed a request.");

            var data = context.GetInput<SayHelloRequest>();

            var outputs = new List<string>();

            foreach (var name in data.Names)
            {
                try
                {
                    // Retries with Azure Durable Functions https://www.serverlessnotes.com/docs/retries-with-azure-durable-functions
                    // outputs.Add(await context.CallActivityAsync<string>("Function1_Hello", name));

                    var retryOptions =
                        new RetryOptions(TimeSpan.FromSeconds(5), 3);

                    outputs.Add(
                        await context.CallActivityWithRetryAsync<string>(
                            "Function1_Hello",
                            retryOptions,
                            name));
                }
                catch
                {
                    errors.Add(JsonConvert.SerializeObject(new SayHelloRequest { Names = new[] { name } }));
                }
            }

            return outputs;
        }

        [FunctionName("Function1_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Function1_Hello trigger function processed a request.");

            if (name == "Matt")
            {
                throw new ArgumentException(nameof(name));
            }

            log.LogInformation($"Saying hello to {name}.");

            return $"Hello {name}!";
        }
    }
}