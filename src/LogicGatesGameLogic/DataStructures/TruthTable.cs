namespace LogicGatesGameLogic.DataStructures;

public class TruthTable
{
    public int Length => Table.Length;
    
    public readonly uint[] Table;
    public readonly bool[] Output;

    public TruthTable(int inputCount, int outputCount)
    {
        Table = new uint[(ulong)1 << inputCount];
        Output = new bool[outputCount];
    }

    public TruthTable(uint[] table, bool[] output)
    {
        Table = table;
        Output = output;
    }

    public TruthTable(TruthTable truthTable)
    {
        Table = new uint[truthTable.Length];
        Output = new bool[truthTable.Length];
    }

    public void PopulateAt(uint address, uint outputs)
    {
        Table[address] = outputs;
    }

    public void PopulateAt(uint address, bool[] outputs)
    {
        PopulateAt(address, outputs.ConvertToUInt());
    }

    public void EvaluateAt(uint inputs)
    {
        uint output = Table[inputs];
        
        for (int i = 0; i < Output.Length; i++)
        {
            Output[i] = (output & (1 << i)) > 0;
        }
    }

    public void EvaluateAt(bool[] inputs)
    {
        EvaluateAt(inputs.ConvertToUInt());
    }

    public void DeleteContents()
    {
        for (int i = 0; i < Length; i++)
        {
            Table[i] = 0;
        }
    }

    public void Populate(TruthTable source, int index, int length)
    {
        Array.Copy(source.Table, index, Table, index, length);
    }
}
