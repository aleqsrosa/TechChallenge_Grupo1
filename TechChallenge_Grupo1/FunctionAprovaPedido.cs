using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TechChallenge_Grupo1
{
    public static class FunctionAprovaPedido
    {
        [FunctionName("DurableFunctionsTechChallenge")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            outputs.Add(await context.CallActivityAsync<string>(nameof(ReceberPedido), 1));
            outputs.Add(await context.CallActivityAsync<string>(nameof(VerificarEstoque), 1));
            outputs.Add(await context.CallActivityAsync<string>(nameof(VerificarPagamento), 1)); 
            outputs.Add(await context.CallActivityAsync<string>(nameof(DespacharProduto), 1));

            return outputs;
        }

        [FunctionName(nameof(ReceberPedido))]
        public static string ReceberPedido([ActivityTrigger] int pedido, ILogger log)
        {
            log.LogInformation("recebendo pedido {pedido}.", pedido);
            return $"Pedido {pedido} recebido com sucesso!";
        }

        [FunctionName(nameof(VerificarEstoque))]
        public static string VerificarEstoque([ActivityTrigger] int pedido, ILogger log)
        {
            log.LogInformation("Verificando o estoque do item {pedido}.", pedido);
            return $"Item {pedido} em estoque";
        }

        [FunctionName(nameof(VerificarPagamento))]
        public static string VerificarPagamento([ActivityTrigger] int pedido, ILogger log)
        {
            log.LogInformation("Verificando o pagamento do pedido {pedido}.", pedido);
            return $"Pagamento do pedido {pedido} realizado com sucesso!";
        }

        [FunctionName(nameof(DespacharProduto))]
        public static string DespacharProduto([ActivityTrigger] int pedido, ILogger log)
        {
            log.LogInformation("Despachando pedido {pedido}.", pedido);
            return $"Produto despachado com sucesso!";
        }

        [FunctionName("DurableFunctionsTechChallenge_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("DurableFunctionsTechChallenge", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}