
using ORG.BasicInfo.API.Extensions;
using ORG.BasicInfo.API.Shared;
using ORG.BasicInfo.EXTService.DTO.AuthServiceDto;
using ORG.BasicInfo.EXTService.RequestGrpc.EditService;
using ORG.BasicInfo.EXTService.RequestGrpc.UpdateService;
using ORG.ServiceManagment.EXTService.RequestGrpc;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
namespace ORG.ServiceManagment.API.Extensions;
[ExcludeFromCodeCoverage]

public static class ServiceCommunicationExtensions
{
    private static ulong idCodePrivateService;
    private static string pemContent;
    private static string serviceManagementUrl;
    public static async Task AddServiceAuth(this IServiceCollection services, IConfiguration configuration)
    {
        //added your services for comminucation in system
        idCodePrivateService = configuration
       .GetRequiredSection("IdCodePrivateService")
       .Get<ulong>(binderOptions => binderOptions.BindNonPublicProperties = true);
        
        pemContent = configuration
       .GetRequiredSection("ServiceManagmentPubKey")
       .Get<string>(binderOptions => binderOptions.BindNonPublicProperties = true);

        serviceManagementUrl = Environment.GetEnvironmentVariable("ServiceDiscovery__servicemanagement_grpc") ?? "https://localhost:5001";
        AuthRestService authRestService = new AuthRestService(serviceManagementUrl, pemContent, idCodePrivateService);
        AuthenticateServiceRestRequestServiceDto authenticateRequestDto = new AuthenticateServiceRestRequestServiceDto { };
        var result = await authRestService.HandleAction(authenticateRequestDto);
        if (result.StatusCode != 200)
            new Exception("can't get jwtCode from ManagmentService!");

        configuration["Jwt"] = result.JwtCode;
        AddServiceUpdateInfo();

    }
    private static void AddServiceUpdateInfo()
    {

        ReadServiceInfo readServiceInfo = new ReadServiceInfo(idCodePrivateService);

        string result=readServiceInfo.ReadServiceInfoToDto();
        if (result != "")
            throw new Exception(result);
        //string pemContent = File.ReadAllText("./publicKeyServiceManagment.pem");
        var serviceManagementUrl = Environment.GetEnvironmentVariable("ServiceDiscovery__servicemanagement_grpc") ?? "https://localhost:5001";
        UpdateService updateService = new UpdateService(serviceManagementUrl, pemContent);
        updateService.HandleAction(readServiceInfo.UpdateServiceGrpc);
     
    }
    public static void AddServiceEditService(this IServiceCollection services)
    {
        //string pemContent = File.ReadAllText("./publicKeyServiceManagment.pem");
        var serviceManagementUrl = Environment.GetEnvironmentVariable("ServiceDiscovery__servicemanagement_grpc") ?? "https://localhost:5001";
        EditService editService = new EditService(serviceManagementUrl, pemContent, idCodePrivateService);
        services.AddSingleton(editService);
    }
    

}
