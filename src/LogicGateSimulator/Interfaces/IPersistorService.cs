using BinarySerializer;

namespace LogicGateSimulator.Interfaces;

public interface IPersistorService
{
    Task<T?> ReadAsync<T>(Guid id) where T : IBinarySerializable<T>, new();
    Task UpsertAsync<T>(Guid id, T value) where T : IBinarySerializable<T>;
    Task DeleteAsync(Guid id);
}
