using LogicGatesGameLogic;
using LogicGateSimulator.Interfaces;
using Microsoft.AspNetCore.Components;

namespace LogicGateSimulator.Pages;

public partial class Home
{
    [Inject] public required IPersistorService Persistor { get; set; }

    private Simulations _simulations = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            return;
        }

        await InitializeDatabaseAsync();
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            Simulations? simulations = await Persistor.ReadAsync<Simulations>(Guid.Empty);
            if (simulations is not null)
            {
                _simulations = simulations;
                return;
            }
        }
        catch { }

        await Persistor.UpsertAsync(Guid.Empty, _simulations);
    }
}
