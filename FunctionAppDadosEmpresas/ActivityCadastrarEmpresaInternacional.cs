using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Bogus;
using FunctionAppDadosEmpresas.Models;

namespace FunctionAppDadosEmpresas;

public static class ActivityCadastrarEmpresaInternacional
{
    [Function(nameof(ActivityCadastrarEmpresaInternacional))]
    public static int CadastrarEmpresaInternacional([ActivityTrigger] DateTime inicioProcesso,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ActivityCadastrarEmpresaInternacional));
        logger.LogInformation(
            $"{nameof(ActivityCadastrarEmpresaInternacional)} - Iniciando a execucao...");
        var fakeEmpresas = new Faker<Empresa>().StrictMode(false)
            .RuleFor(p => p.Nome, f => f.Company.CompanyName())
            .RuleFor(p => p.Cidade, f => f.Address.City())
            .RuleFor(p => p.InicioProcessamento, _ => inicioProcesso)
            .Generate(40);
        foreach (var empresa in fakeEmpresas)
        {
            logger.LogInformation(
                $"{nameof(ActivityCadastrarEmpresaInternacional)} - Dados gerados: " +
                JsonSerializer.Serialize(empresa));
            Thread.Sleep(1200);
        }
        return fakeEmpresas.Count;
    }
}
