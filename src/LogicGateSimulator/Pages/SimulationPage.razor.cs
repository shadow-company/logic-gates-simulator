using LogicGatesGameLogic;
using LogicGateSimulator.GameLogic.Management;
using LogicGateSimulator.Interfaces;
using Microsoft.AspNetCore.Components;

namespace LogicGateSimulator.Pages;

public sealed record SimulationPageDataModel
{
    public Simulations Simulations = new();
    public Simulation Simulation = new();
}

public partial class SimulationPage
{
    [Inject] public required IPersistorService Persistor { get; set; }

    private readonly SimulationPageDataModel _simulationPageData = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            return;
        }

        await InitializeSimulation.RunAsync(Persistor, _simulationPageData);
    }
}
