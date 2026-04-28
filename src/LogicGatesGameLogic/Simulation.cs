using BinarySerializer;
using LogicGatesGameLogic.DataStructures;

namespace LogicGatesGameLogic;

[BinarySerializable]
public partial class Simulation
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<Guid> LogicPinsIDs { get; set; } = [];
    public List<Guid> LogicComponentIDs { get; set; } = [];
    public List<Guid> LogicGateIDs { get; set; } = [];

    public Dictionary<Guid, LogicPin> LogicPins = [];
    public Dictionary<Guid, LogicComponent> LogicComponents = [];
    public Dictionary<Guid, LogicGate> LogicGates = [];

    public HashSet<LogicPin> DirtyLogicPins = [];
    public HashSet<LogicComponent> DirtyLogicComponents = [];
    public HashSet<LogicGate> DirtyLogicGates = [];

    private int DirtyComponentsCount => DirtyLogicComponents.Count + DirtyLogicGates.Count + DirtyLogicPins.Count;

    /// <summary>
    /// Evaluates all dirty components once. This simulates a single step
    /// To simulate an entire frame, use 
    /// </summary>
    /// <param name="updateUI">Whether the ui will get updated. Set this to 'true' only if calling this method directly.</param>
    /// <returns>'false' if any errors did occur or the cancellationToken was triggered. 'true' otherwise.</returns>
    public bool SimulateStep(bool updateUI, CancellationToken cancellationToken)
    {
        if (DirtyComponentsCount == 0)
        {
            return true;
        }

        LogicPin[] logicPins = [.. DirtyLogicPins];
        LogicComponent[] logicComponents = [.. DirtyLogicComponents];
        LogicGate[] logicGates = [.. DirtyLogicGates];

        DirtyLogicPins.Clear();
        DirtyLogicComponents.Clear();
        DirtyLogicGates.Clear();

        try
        {
            foreach (LogicPin logicPin in logicPins)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                logicPin.Evaluate(this);
            }

            foreach (LogicComponent logicComponent in logicComponents)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                logicComponent.Evaluate(this);
            }

            foreach (LogicGate logicGate in logicGates)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                logicGate.Evaluate(this);
            }

            if (updateUI)
            {
                UpdateUI();
            }
        }
        catch
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Evaluates all dirty components once. This simulates a single step
    /// To simulate an entire frame, use 
    /// </summary>
    /// <returns>'false' if any errors did occur or the cancellationToken was triggered. 'true' otherwise.</returns>
    public bool SimulateFrame(CancellationToken cancellationToken)
    {
        while (DirtyComponentsCount > 0)
        {
            if (!SimulateStep(false, cancellationToken))
            {
                return false;
            }
        }

        UpdateUI();
        return true;
    }

    /// <summary>
    /// Updates all UI elements to reflect signal changes
    /// </summary>
    public void UpdateUI()
    {

    }
}
