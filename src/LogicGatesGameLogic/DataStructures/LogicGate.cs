using BinarySerializer;

namespace LogicGatesGameLogic.DataStructures;

[BinarySerializable]
public partial class LogicGate : LogicComponent
{
    public Guid InputAPinID { get; set; } = Guid.Empty;
    public Guid InputBPinID { get; set; } = Guid.Empty;
    public Guid OutputPinID { get; set; } = Guid.Empty;

    protected LogicPin? _inputAPin = null;
    protected LogicPin? _inputBPin = null;
    protected LogicPin? _outputPin = null;

    public LogicPin GetInputAPin(Simulation simulation)
    {
        _inputAPin ??= simulation.LogicPins[InputAPinID];
        return _inputAPin;
    }

    public LogicPin GetInputBPin(Simulation simulation)
    {
        _inputBPin ??= simulation.LogicPins[InputBPinID];
        return _inputBPin;
    }

    public LogicPin GetOutputPin(Simulation simulation)
    {
        _outputPin ??= simulation.LogicPins[OutputPinID];
        return _outputPin;
    }
}
