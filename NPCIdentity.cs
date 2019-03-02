using System.Diagnostics;

namespace GridExplorerBot
{
    public class NPCIdentityData
    {
        private int mSkinToneIndex = 0;
        private int mGenderIndex = 0;

        public NPCIdentityData()
        {
            mSkinToneIndex = Game.random.Next() % StringUtils.GetNumSkinToneVariations();
            mGenderIndex = Game.random.Next() % StringUtils.GetNumGenderVariations();
        }

        public void Stream(SaveStream stream)
        {
            stream.Stream(ref mSkinToneIndex, SaveUtils.GetNumBitsToStoreValue(StringUtils.GetNumSkinToneVariations() - 1));
            stream.Stream(ref mGenderIndex, SaveUtils.GetNumBitsToStoreValue(StringUtils.GetNumGenderVariations() - 1));
        }

        public int GetSkinToneIndex()
        {
            return mSkinToneIndex;
        }

        public int GetGenderIndex()
        {
            return mGenderIndex;
        }
    }

    public class NPCIdentifier
    {
        public const int maxNumNPCs = 10;
        public static int currentMaxID = 0;
        
        private int mId = 0;

        public NPCIdentifier()
        {
            mId = currentMaxID++;

            Debug.Assert(currentMaxID < maxNumNPCs); // Ensure there's enough space in the save data for all the IDs

            InitialRooms.identityData[mId] = new NPCIdentityData();
        }

        public string GetEmojiVariant(string baseEmoji)
        {
            return StringUtils.GetVariantEmoji(baseEmoji, Game.mNPCIdentities[mId]);
        }

        public void Stream(SaveStream stream)
        {
            stream.Stream(ref mId, SaveUtils.GetNumBitsToStoreValue(maxNumNPCs));
        }
    }
}