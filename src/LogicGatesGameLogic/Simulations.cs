using BinarySerializer;

namespace LogicGatesGameLogic;

[BinarySerializable]
public partial class Simulations
{
    public List<Guid> PageIDs { get; set; } = [];
}
