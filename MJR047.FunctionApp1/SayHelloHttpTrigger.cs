using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace MJR047.FunctionApp1
{
    public static class SayHelloHttpTrigger
    {
        [FunctionName(nameof(SayHelloHttpTrigger))]
        public static async Task<HttpResponseMessage> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {
            log.LogInformation($"{nameof(SayHelloHttpTrigger)} trigger function processed a request.");

            var data = await req.Content.ReadAsAsync<SayHelloRequest>();

            var instanceId =
                await starter.StartNewAsync(nameof(SayHelloOrchestrationTrigger), data);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
