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



    public LogicComponent Parent { get { _parent ??= Simulation.Instance.LogicComponents[ParentComponentID]; return _parent; } }
    public LogicPin IncomingConnection { get { _incomingConnection ??= Simulation.Instance.LogicPins[IncomingConnectionPinID]; return _incomingConnection; } }
    public List<LogicPin> OutgoingConnections { get { _outgoingConnections ??= [.. OutgoingConnectionIDs.Select(id => Simulation.Instance.LogicPins[id])]; return _outgoingConnections; } }



    private LogicComponent? _parent = null;
    private LogicPin? _incomingConnection = null;
    private List<LogicPin>? _outgoingConnections = null;

    public LogicPin(Guid parentComponentID)
    {
        ParentComponentID = parentComponentID;
    }

    public void Evaluate()
    {
        bool output = State ^ IsInverted;

        if (output == OutputState)
        {
            return;
        }

        OutputState = output;
        Propagate();
    }

    public void SetState(bool state)
    {
        if (State == state)
        {
            return;
        }

        State = state;
        Simulation.Instance.DirtyLogicPins.Add(this);
    }

    private void Propagate()
    {
        foreach (Guid outgoingConnectionID in OutgoingConnectionIDs)
        {
            Simulation.Instance.LogicPins[outgoingConnectionID].SetState(OutputState);
        }

        if (PinMode is PinModes.Input)
        {
            Simulation.Instance.DirtyLogicComponents.Add(Parent);
        }
    }
}
