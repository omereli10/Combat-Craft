using Microsoft.Xna.Framework;

namespace Combat_Craft
{
    static class Spawner
    {
        #region Methods

        public static void plantTree(Vector3 position)
        {
            Vector3 pos, chunkHeight_DictionaryLocation;
            int i, woodHeight = 5 + (((int)position.X + (int)position.Z) % 2);

            #region Wood Trunks

            for (i = 0; i < woodHeight; i++)
            {
                pos = new Vector3(position.X, position.Y + i, position.Z);
                if (!GameDictionaries.blocksDestroyedDictionary.ContainsKey(pos))
                { GameDictionaries.blocksDictionary[pos] = BlockType.Wood_Trunk; }
            }

            #endregion Wood Trunks

            #region Leafs

            for (int x = (int)position.X - 2; x < (int)position.X + 3; x++)
            {
                for (int z = (int)position.Z - 2; z < (int)position.Z + 3; z++)
                {
                    chunkHeight_DictionaryLocation = RayBlock.getChunkOrigin_OffSet(new Vector3(x, 0, z), 0, 0, 0);
                    if ((GameDictionaries.chunksHeightDictionary.TryGetValue(chunkHeight_DictionaryLocation, out int heightValue) || true ) && heightValue < ((int)position.Y + i + 1))
                    { GameDictionaries.chunksHeightDictionary[chunkHeight_DictionaryLocation] = (int)position.Y + i + 1; }
                    for (int y = (int)position.Y + i - 2; y < (int)position.Y + i + 2; y++)
                    {
                        if (x != position.X || z != position.Z || y > (int)position.Y + i - 1)
                        {
                            pos = new Vector3(x, y, z);
                            if (!GameDictionaries.blocksDestroyedDictionary.ContainsKey(pos))
                            { GameDictionaries.blocksDictionary[pos] = BlockType.Leaf; }
                        }
                    }
                }
            }
            #endregion Leafs
        }

        #endregion Methods
    }
}