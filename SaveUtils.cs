using System.Diagnostics;

namespace GridExplorerBot
{
    public static class SaveUtils
    {
        public static int GetNumBitsToStoreValue(int value)
        {
            Debug.Assert(value >= 0);
            return (int)System.MathF.Log(value, 2) + 1; 
        }

        public static int GetNumBits(InitialRooms.ID id)
        {
            return GetNumBitsToStoreValue(63);
        }

        public static int GetNumBits(Objects.ID id)
        {
            return GetNumBitsToStoreValue(127);
        }
    }
}