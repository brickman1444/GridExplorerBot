using System.Collections.Generic;

namespace GridExplorerBot
{
    public class InventoryEntry
    {
        public Objects.ID mType = Objects.ID.Unknown;
        public int mDisplayEmojiIndex = -1;
        public int mQuantity = 1;

        public string Render()
        {
            string outText = Emoji.GetEmoji(mType, mDisplayEmojiIndex);

            if (mQuantity > 1)
            {
                outText += "x" + mQuantity;
            }

            return outText;
        }

        public string Save()
        {
            byte type = (byte)mType;
            byte displayIndex = (byte)mDisplayEmojiIndex;
            byte quantity = (byte)mQuantity;
            byte[] bytes = { type, displayIndex, quantity };
            return StringUtils.SaveDataEncode(bytes);
        }

        public void Load(string saveData)
        {
            byte[] bytes = StringUtils.SaveDataDecode(saveData);
            mType = (Objects.ID)bytes[0];
            mDisplayEmojiIndex = bytes[1];
            mQuantity = bytes[2];
        }
    }

    public class Inventory
    {
        List<InventoryEntry> mEntries = new List<InventoryEntry>();

        public string Render()
        {
            List<string> renderedEntries = new List<string>();

            foreach (InventoryEntry entry in mEntries)
            {
                renderedEntries.Add(entry.Render());
            }

            return string.Join(' ', renderedEntries);
        }

        public string Save()
        {
            string outSaveData = "";

            List<string> entryTokens = new List<string>();

            foreach (InventoryEntry entry in mEntries)
            {
                entryTokens.Add(entry.Save());
            }

            outSaveData += string.Join(' ', entryTokens);

            return outSaveData;
        }

        public void Load(string saveData)
        {
            string[] tokens = saveData.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                InventoryEntry entry = new InventoryEntry();
                entry.Load(token);
                mEntries.Add(entry);
            }
        }

        public void AddItem(DynamicObject dynamicObject)
        {
            InventoryEntry existingEntry = GetEntry(dynamicObject.mType);

            if (existingEntry != null)
            {
                existingEntry.mQuantity++;
            }
            else
            {
                InventoryEntry entry = new InventoryEntry()
                {
                    mDisplayEmojiIndex = dynamicObject.mDisplayEmojiIndex,
                    mQuantity = 1,
                    mType = dynamicObject.mType
                };

                mEntries.Add(entry);
            }
        }

        public void RemoveItem(Objects.ID type)
        {
            InventoryEntry entry = GetEntry(type);

            if (entry.mQuantity > 1)
            {
                entry.mQuantity--;
            }
            else
            {
                mEntries.Remove(entry);
            }
        }

        public InventoryEntry GetEntry(Objects.ID type)
        {
            foreach (InventoryEntry entry in mEntries)
            {
                if (entry.mType == type)
                {
                    return entry;
                }
            }

            return null;
        }

        public int GetBalance(Objects.ID type)
        {
            InventoryEntry entry = GetEntry(type);

            return (entry != null ? entry.mQuantity : 0);
        }
    }
}