using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MJR047.FunctionApp1
{
    public static class SayHelloQueueTrigger
    {
        [FunctionName(nameof(SayHelloQueueTrigger))]
        public static async Task Run(
            [QueueTrigger("sayhello")]string queueItem,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation($"{nameof(SayHelloQueueTrigger)} trigger function processed a request.");

            var data = JsonConvert.DeserializeObject<SayHelloRequest>(queueItem);

            await starter.StartNewAsync(nameof(SayHelloOrchestrationTrigger), data);
        }
    }
}
