using Microsoft.Xna.Framework;

namespace Combat_Craft
{
    static class Raycast
    {
        public static Vector3 Raycast_destroyBlockOrigin(Player player, int interactions = 20)
        {
            // Initialize variables
            Vector3 destroyBlockOrigin = new Vector3(0, -1, 0);
            Vector3 directionRay = Raycast.Ray_Direction(player);

            // Checking for interaction with blocks and direction vector
            Vector3 calculatedOrigin;
            for (int i = 0; i < interactions; i++)
            {
                calculatedOrigin = Chunk.getBlockOffset(player.camera.Position + directionRay * i);
                if (GameDictionaries.blocksDictionary.ContainsKey(calculatedOrigin))
                {
                    if (GameDictionaries.blocksDictionary[calculatedOrigin] != BlockType.Water)
                    { destroyBlockOrigin = calculatedOrigin; break; }
                }
            }

            return destroyBlockOrigin;
        }

        public static Vector3 Raycast_addBlockOrigin(Player player, int interactions = 20)
        {
            // Initialize variables
            Vector3 destroyBlockOrigin = new Vector3(0, -1, 0);
            Vector3 directionRay = Raycast.Ray_Direction(player);

            // Checking for interaction with blocks and direction vector
            int i; Vector3 calculatedOrigin;
            for (i = 0; i < interactions; i++)
            {
                calculatedOrigin = Chunk.getBlockOffset(player.camera.Position + directionRay * i);
                if (GameDictionaries.blocksDictionary.ContainsKey(calculatedOrigin))
                {
                    if (GameDictionaries.blocksDictionary[calculatedOrigin] != BlockType.Water)
                    { destroyBlockOrigin = calculatedOrigin; break; }
                }
            }

            if (destroyBlockOrigin != new Vector3(0, -1, 0) && i != 0)
            { return Chunk.getBlockOffset(player.camera.Position + directionRay * (i - 1)); }
            else
            { return destroyBlockOrigin; }
        }

        private static Vector3 Ray_Direction(Player player)
        {
            // Calculate the NEAR ray point of view
            Vector3 nearPoint = Globals.graphicsDevice.Viewport.Unproject(new Vector3(Globals.graphicsDevice.Viewport.Width / 2, Globals.graphicsDevice.Viewport.Height / 2, 0f),
            player.camera.Projection, player.camera.View, Matrix.Identity);

            // Calculate the FAR ray point of view
            Vector3 farPoint = Globals.graphicsDevice.Viewport.Unproject(new Vector3(Globals.graphicsDevice.Viewport.Width / 2, Globals.graphicsDevice.Viewport.Height / 2, 1f),
            player.camera.Projection, player.camera.View, Matrix.Identity);

            // Calculate the direction ray
            Vector3 direction = farPoint - nearPoint;
            if (direction != Vector3.Zero)
            {
                direction.Normalize();
                direction /= 6;
            }

            return direction;
        }
    }
}