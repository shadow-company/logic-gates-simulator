using LogicGatesGameLogic.Enums;
using BinarySerializer;

namespace LogicGatesGameLogic.DataStructures;

[BinarySerializable]
public partial class LogicBusTemplate
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public List<Guid> Inputs { get; set; } = [];
    public List<Guid> Outputs { get; set; } = [];
    public string Name { get; set; } = "Bus";
    public BusDirections BusDirection { get; set; } = BusDirections.Horizontal;
    public int CoordinateOffset { get; set; } = 0;
}
