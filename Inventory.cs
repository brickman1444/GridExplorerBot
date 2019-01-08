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

        public void Save(BitStreams.BitStream stream)
        {
            stream.WriteByte((byte)mType, 7); // 127 7 bits
            stream.WriteByte((byte)mDisplayEmojiIndex, 3); // 7 3 bits
            stream.WriteByte((byte)mQuantity, 7); // 7 3 bits
        }

        public void Load(BitStreams.BitStream stream)
        {
            mType = (Objects.ID)stream.ReadByte(7);
            mDisplayEmojiIndex = stream.ReadByte(3);
            mQuantity = stream.ReadByte(7);
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

        public void Save(BitStreams.BitStream stream)
        {
            stream.WriteByte((byte)mEntries.Count, 4); // 10 4

            foreach (InventoryEntry entry in mEntries)
            {
                entry.Save(stream);
            }
        }

        public void Load(BitStreams.BitStream stream)
        {
            byte numEntries = stream.ReadByte(4);

            for (int entryIndex = 0; entryIndex < numEntries; entryIndex++)
            {
                InventoryEntry entry = new InventoryEntry();
                entry.Load(stream);
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
                    mDisplayEmojiIndex = Emoji.GetEmojiIndex(dynamicObject.mType, dynamicObject.Render()),
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