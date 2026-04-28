using BinarySerializer;
using LogicGatesGameLogic.Enums;

namespace LogicGatesGameLogic.DataStructures;

[BinarySerializable]
public partial class LogicSignal
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public SignalModes SignalMode { get; set; } = SignalModes.Normal;
    public SignalDisplayModes SignalDisplayMode { get; set; } = SignalDisplayModes.Connections;
    public Dictionary<Guid, SignalModes> ConnectionIDs { get; set; } = [];
}
