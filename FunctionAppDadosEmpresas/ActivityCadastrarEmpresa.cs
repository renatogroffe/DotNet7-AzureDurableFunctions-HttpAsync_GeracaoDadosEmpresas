using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Bogus;
using Bogus.Extensions.Brazil;
using FunctionAppDadosEmpresas.Models;

namespace FunctionAppDadosEmpresas;

public static class ActivityCadastrarEmpresa
{
    [Function(nameof(ActivityCadastrarEmpresa))]
    public static int CadastrarEmpresa([ActivityTrigger] DateTime inicioProcesso,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ActivityCadastrarEmpresa));
        logger.LogInformation(
            $"{nameof(ActivityCadastrarEmpresa)} - Iniciando a execucao...");
        var fakeEmpresas = new Faker<Empresa>("pt_BR").StrictMode(false)
            .RuleFor(p => p.Nome, f => f.Company.CompanyName())
            .RuleFor(p => p.CNPJ, f => f.Company.Cnpj(true))
            .RuleFor(p => p.Cidade, f => f.Address.City())
            .RuleFor(p => p.InicioProcessamento, _ => inicioProcesso)
            .Generate(30);
        foreach (var empresa in fakeEmpresas)
        {
            logger.LogInformation(
                $"{nameof(ActivityCadastrarEmpresa)} - Dados gerados: " +
                JsonSerializer.Serialize(empresa));
            Thread.Sleep(1000);
        }
        return fakeEmpresas.Count;
    }
}
