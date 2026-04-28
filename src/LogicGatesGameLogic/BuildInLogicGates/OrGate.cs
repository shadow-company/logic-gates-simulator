using LogicGatesGameLogic.DataStructures;

namespace LogicGatesGameLogic.BuildInLogicGates;

public class OrGate : LogicGate
{
    public OrGate()
    {
        _inputAPin = new LogicPin(ID);
        InputAPinID = _inputAPin.ID;
        _inputBPin = new LogicPin(ID);
        InputBPinID = _inputBPin.ID;
        _outputPin = new LogicPin(ID);
        OutputPinID = _outputPin.ID;
    }

    public override void Evaluate(Simulation simulation)
    {
        _outputPin!.SetState(_inputAPin!.OutputState | _inputBPin!.OutputState, simulation);
    }
}
