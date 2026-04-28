namespace LogicGatesGameLogic;

public static partial class ExtensionMethods
{
    public static uint ConvertToUInt(this bool[] bools)
    {
        uint output = 0;
        for (int i = 0; i < bools.Length; i++)
        {
            if (bools[i])
            {
                output |= (uint)(1 << i);
            }
        }

        return output;
    }
}
