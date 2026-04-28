using BinarySerializer;

namespace LogicGatesGameLogic.DataStructures;

[BinarySerializable]
public partial class LogicComponent
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public List<Guid> InputPinIDs { get; set; } = [];
    public List<Guid> OutputPinIDs { get; set; } = [];
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;


    private List<LogicPin>? _inputs = null;
    private List<LogicPin>? _outputs = null;

    public virtual void Evaluate(Simulation simulation) { }

    public List<LogicPin> GetInputPins(Simulation simulation)
    {
        _inputs ??= [.. InputPinIDs.Select(id => simulation.LogicPins[id])];
        return _inputs;
    }

    public List<LogicPin> GetOutputPins(Simulation simulation)
    {
        _outputs ??= [.. OutputPinIDs.Select(id => simulation.LogicPins[id])];
        return _outputs;
    }

    public LogicPin AddInput(Simulation simulation)
    {
        LogicPin logicPin = new(ID);
        InputPinIDs.Add(logicPin.ID);
        GetInputPins(simulation).Add(logicPin);
        return logicPin;
    }

    public LogicPin AddOutput(Simulation simulation)
    {
        LogicPin logicPin = new(ID);
        OutputPinIDs.Add(logicPin.ID);
        GetOutputPins(simulation).Add(logicPin);
        return logicPin;
    }

    public void RemovePin(LogicPin pin, Simulation simulation)
    {
        GetInputPins(simulation).Remove(pin);
        InputPinIDs.Remove(pin.ID);
        GetOutputPins(simulation).Remove(pin);
        OutputPinIDs.Remove(pin.ID);
        simulation.LogicPins.Remove(pin.ID);
        simulation.DirtyLogicPins.Remove(pin);
    }
}
