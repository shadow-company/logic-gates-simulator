using BinarySerializer;

namespace LogicGatesGameLogic.DataStructures;

[BinarySerializable]
public partial class LogicGate : LogicComponent
{
    public Guid InputAPinID { get; set; } = Guid.Empty;
    public Guid InputBPinID { get; set; } = Guid.Empty;
    public Guid OutputPinID { get; set; } = Guid.Empty;

    public LogicPin InputAPin { get { _inputAPin ??= Simulation.Instance.LogicPins[InputAPinID]; return _inputAPin; } }
    public LogicPin InputBPin { get { _inputBPin ??= Simulation.Instance.LogicPins[InputBPinID]; return _inputBPin; } }
    public LogicPin OutputPin { get { _outputPin ??= Simulation.Instance.LogicPins[OutputPinID]; return _outputPin; } }

    protected LogicPin? _inputAPin = null;
    protected LogicPin? _inputBPin = null;
    protected LogicPin? _outputPin = null;
}
