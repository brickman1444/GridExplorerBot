using System.Collections.Generic;

namespace GridExplorerBot
{
    public class InventoryEntry
    {
        public Objects.ID mType = Objects.ID.Unknown;

        public virtual bool IsStackable() { return false; }

        public string Render()
        {
            string outText = Emoji.GetEmoji(mType, 0);

            return outText;
        }

        public void Save(BitStreams.BitStream stream)
        {
            stream.Write(mType);
        }

        public void Load(BitStreams.BitStream stream)
        {
            stream.Read(out mType);
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
            InventoryEntry existingEntry = GetEntry(dynamicObject.GetTypeID());

            if (existingEntry != null && existingEntry.IsStackable())
            {
                //existingEntry.mQuantity++;
            }
            else
            {
                InventoryEntry entry = new InventoryEntry()
                {
                    mType = dynamicObject.GetTypeID(),
                };

                mEntries.Add(entry);
            }
        }

        public void RemoveItem(Objects.ID type)
        {
            InventoryEntry entry = GetEntry(type);

            if (entry.IsStackable())
            {
                //entry.mQuantity--;
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

        public bool Contains(Objects.ID type)
        {
            InventoryEntry entry = GetEntry(type);

            return (entry != null);
        }
    }
}