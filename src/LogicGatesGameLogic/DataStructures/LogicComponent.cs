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



    public List<LogicPin> InputPins { get { _inputs ??= [.. InputPinIDs.Select(id => Simulation.Instance.LogicPins[id])]; return _inputs; } }
    public List<LogicPin> OutputPins { get { _outputs ??= [.. OutputPinIDs.Select(id => Simulation.Instance.LogicPins[id])]; return _outputs; } }



    private List<LogicPin>? _inputs = null;
    private List<LogicPin>? _outputs = null;

    public virtual void Evaluate() { }

    public LogicPin AddInput()
    {
        LogicPin logicPin = new(ID);
        InputPinIDs.Add(logicPin.ID);
        InputPins.Add(logicPin);
        return logicPin;
    }

    public LogicPin AddOutput()
    {
        LogicPin logicPin = new(ID);
        OutputPinIDs.Add(logicPin.ID);
        OutputPins.Add(logicPin);
        return logicPin;
    }

    public void RemovePin(LogicPin pin)
    {
        InputPins.Remove(pin);
        InputPinIDs.Remove(pin.ID);
        OutputPins.Remove(pin);
        OutputPinIDs.Remove(pin.ID);
        Simulation.Instance.LogicPins.Remove(pin.ID);
        Simulation.Instance.DirtyLogicPins.Remove(pin);
    }
}
