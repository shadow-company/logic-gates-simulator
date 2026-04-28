using LogicGateSimulator.Interfaces;
using LogicGateSimulator.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace LogicGateSimulator;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebAssemblyHostBuilder webAssemblyHostBuilder = WebAssemblyHostBuilder.CreateDefault(args);
        webAssemblyHostBuilder.RootComponents.Add<App>("#app");
        webAssemblyHostBuilder.RootComponents.Add<HeadOutlet>("head::after");
        webAssemblyHostBuilder.Services.AddHttpClient();
        webAssemblyHostBuilder.Services.AddSingleton<IPersistorService, IndexedDbPersistor>();

        WebAssemblyHost webAssemblyHost = webAssemblyHostBuilder.Build();
        await webAssemblyHost.RunAsync();
    }
}
