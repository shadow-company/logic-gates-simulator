using LogicGatesGameLogic;
using LogicGateSimulator.GameLogic.Management;
using LogicGateSimulator.Interfaces;
using Microsoft.AspNetCore.Components;

namespace LogicGateSimulator.Pages;

public sealed record HomeDataModel
{
    public Simulations Simulations = new();
    public Simulation Simulation = new();
}

public partial class Home
{
    [Inject] public required IPersistorService Persistor { get; set; }

    private readonly HomeDataModel _homeData = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            return;
        }

        await InitializeSimulation.RunAsync(Persistor, _homeData);
    }
}
