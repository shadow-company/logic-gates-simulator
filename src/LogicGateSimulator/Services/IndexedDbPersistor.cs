using BinarySerializer;
using LogicGateSimulator.Interfaces;
using Microsoft.JSInterop;

namespace LogicGateSimulator.Services;

public class IndexedDbPersistor : IPersistorService
{
    private readonly IJSRuntime _jsRuntime;

    public IndexedDbPersistor(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _jsRuntime.InvokeVoidAsync("DeleteAsync", id.ToString());
    }

    public async Task<T?> ReadAsync<T>(Guid id) where T : IBinarySerializable<T>, new()
    {
        IJSStreamReference? jsStreamReference = await _jsRuntime.InvokeAsync<IJSStreamReference?>("ReadAsync", id.ToString());
        if (jsStreamReference is null)
        {
            return default;
        }

        T result = new();
        await using Stream stream = await jsStreamReference.OpenReadStreamAsync();
        using MemoryStream memoryStream = new();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        result.Deserialize(memoryStream);
        return result;
    }

    public async Task UpsertAsync<T>(Guid id, T value) where T : IBinarySerializable<T>
    {
        using MemoryStream memoryStream = value.Serialize();
        memoryStream.Position = 0;
        using DotNetStreamReference dotNetStreamReference = new(memoryStream);
        await _jsRuntime.InvokeVoidAsync("UpsertAsync", id.ToString(), dotNetStreamReference);
    }
}
