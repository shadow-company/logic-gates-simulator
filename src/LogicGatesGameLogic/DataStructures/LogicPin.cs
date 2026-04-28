using BinarySerializer;
using LogicGatesGameLogic.Enums;
using System.Data;

namespace LogicGatesGameLogic.DataStructures;

[BinarySerializable]
public partial class LogicPin
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public Guid ParentComponentID { get; set; } = Guid.Empty;
    public Guid IncomingConnectionPinID { get; set; } = Guid.Empty;
    public List<Guid> OutgoingConnectionIDs { get; set; } = [];
    public bool IsInverted { get; set; } = false;
    public bool OutputState { get; set; } = false;
    public bool State { get; set; } = false;
    public PinModes PinMode { get; set; } = PinModes.Input;
    public Directions PinDirection { get; set; } = Directions.North;
    public int OffsetX { get; set; } = 0;
    public int OffsetY { get; set; } = 0;


    private LogicComponent? _parent = null;
    private LogicPin? _incomingConnection = null;
    private List<LogicPin>? _outgoingConnections = null;

    public LogicPin(Guid parentComponentID)
    {
        ParentComponentID = parentComponentID;
    }

    public LogicComponent GetParent(Simulation simulation)
    {
        _parent ??= simulation.LogicComponents[ParentComponentID];
        return _parent;
    }

    public LogicPin GetIncomingConnection(Simulation simulation)
    {
        _incomingConnection ??= simulation.LogicPins[IncomingConnectionPinID];
        return _incomingConnection;
    }

    public List<LogicPin> GetOutgoingConnections(Simulation simulation)
    {
        _outgoingConnections ??= [.. OutgoingConnectionIDs.Select(id => simulation.LogicPins[id])];
        return _outgoingConnections;
    }

    public void Evaluate(Simulation simulation)
    {
        bool output = State ^ IsInverted;

        if (output == OutputState)
        {
            return;
        }

        OutputState = output;
        Propagate(simulation);
    }

    public void SetState(bool state, Simulation simulation)
    {
        if (State == state)
        {
            return;
        }

        State = state;
        simulation.DirtyLogicPins.Add(this);
    }

    private void Propagate(Simulation simulation)
    {
        foreach (Guid outgoingConnectionID in OutgoingConnectionIDs)
        {
            simulation.LogicPins[outgoingConnectionID].SetState(OutputState, simulation);
        }

        if (PinMode is PinModes.Input)
        {
            simulation.DirtyLogicComponents.Add(GetParent(simulation));
        }
    }
}
