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
    }
}