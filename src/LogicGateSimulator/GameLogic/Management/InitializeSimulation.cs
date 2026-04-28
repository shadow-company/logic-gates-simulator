using LogicGatesGameLogic;
using LogicGateSimulator.Interfaces;
using LogicGateSimulator.Pages;

namespace LogicGateSimulator.GameLogic.Management;

internal static class InitializeSimulation
{
    internal static async Task RunAsync(IPersistorService persistor, HomeDataModel homeData)
    {
        try
        {
            Simulations? simulations = await persistor.ReadAsync<Simulations>(Guid.Empty);
            if (simulations is not null)
            {
                homeData.Simulations = simulations;
                return;
            }
        }
        catch { }

        Guid currentSimulationID = Guid.NewGuid();
        homeData.Simulations = new() { CurrentSimulationID = currentSimulationID, SimulationIDs = [currentSimulationID] };
        homeData.Simulation = new() { ID = currentSimulationID };

        await persistor.UpsertAsync(Guid.Empty, homeData.Simulations);
        await persistor.UpsertAsync(currentSimulationID, homeData.Simulation);
    }
}
