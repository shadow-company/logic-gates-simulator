using LogicGatesGameLogic.Enums;
using Microsoft.AspNetCore.Components;

namespace LogicGateSimulator.Components;

public abstract class LogicGateBaseComponent : ComponentBase
{
    [Parameter, EditorRequired] public required SignalModes InputASignalMode { get; set; }
    [Parameter, EditorRequired] public required SignalModes InputBSignalMode { get; set; }
    [Parameter, EditorRequired] public required SignalModes OutputSignalMode { get; set; }
    [Parameter, EditorRequired] public required int X { get; set; }
    [Parameter, EditorRequired] public required int Y { get; set; }

    protected string GetCssStyleAttribute()
    {
        return $"position: absolute; top: {Y}px; left: {X}px;";
    }
}
