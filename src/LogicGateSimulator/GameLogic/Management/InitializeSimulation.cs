using LogicGatesGameLogic;
using LogicGateSimulator.Interfaces;
using LogicGateSimulator.Pages;

namespace LogicGateSimulator.GameLogic.Management;

internal static class InitializeSimulation
{
    internal static async Task RunAsync(IPersistorService persistor, SimulationPageDataModel simulationPageData)
    {
        try
        {
            Simulations? simulations = await persistor.ReadAsync<Simulations>(Guid.Empty);
            if (simulations is not null)
            {
                simulationPageData.Simulations = simulations;
                return;
            }
        }
        catch { }

        Guid currentSimulationID = Guid.NewGuid();
        simulationPageData.Simulations = new() { CurrentSimulationID = currentSimulationID, SimulationIDs = [currentSimulationID] };
        simulationPageData.Simulation = new() { ID = currentSimulationID };

        await persistor.UpsertAsync(Guid.Empty, simulationPageData.Simulations);
        await persistor.UpsertAsync(currentSimulationID, simulationPageData.Simulation);
    }
}
