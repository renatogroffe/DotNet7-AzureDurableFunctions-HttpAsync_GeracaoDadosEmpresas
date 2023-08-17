using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using FunctionAppDadosEmpresas.Models;

namespace FunctionAppDadosEmpresas
{
    public static class Orquestracao
    {
        [Function(nameof(Orquestracao))]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(Orquestracao));

            var inicioProcessamento = DateTime.Now;
            logger.LogInformation(
                $"Iniciando processamento - {inicioProcessamento:HH:mm:ss}...");

            var qtd1aCarga = await context.CallActivityAsync<int>(
                nameof(ActivityCadastrarEmpresa), inicioProcessamento);
            context.SetCustomStatus(
                $"1a carga concluida com {JsonSerializer.Serialize(qtd1aCarga)} registro(s)");

            var qtd2aCarga = await context.CallActivityAsync<int>(
                nameof(ActivityCadastrarEmpresaInternacional), inicioProcessamento);
            context.SetCustomStatus(
                $"2a carga concluida com {JsonSerializer.Serialize(qtd2aCarga)} registro(s)");
        }

        [Function(nameof(DadosEmpresaHttpStart))]
        public static async Task<HttpResponseData> DadosEmpresaHttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger(nameof(DadosEmpresaHttpStart));

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(Orquestracao));

            logger.LogInformation("Iniciada orquestracao com ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}