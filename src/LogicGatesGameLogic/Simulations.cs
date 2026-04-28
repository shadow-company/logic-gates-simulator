using BinarySerializer;

namespace LogicGatesGameLogic;

[BinarySerializable]
public partial class Simulations
{
    public Guid CurrentSimulationID { get; set; }
    public List<Guid> SimulationIDs { get; set; }

    public Simulations()
    {
        CurrentSimulationID = Guid.NewGuid();
        SimulationIDs = [CurrentSimulationID];
    }
}
