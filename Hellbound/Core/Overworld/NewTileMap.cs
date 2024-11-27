using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Overworld
{
    public class NewTileMap
    {
        public const int TILE_SIZE = 16;

        public int width;
        public int height;
        public int[,] tiles;
        public TileConnectionType[,] displayedTiles;
        public bool needsUpdate;

        public RenderTarget2D texture;

        public NewTileMap(int width = 3, int height = 3)
        {
            this.width = width;
            this.height = height;
            tiles = new int[width, height];
            displayedTiles = new TileConnectionType[width, height];
            needsUpdate = true;
        }

        public int this[int x, int y]
        {
            get => tiles[x, y];
            set
            {
                needsUpdate = true;
                tiles[x, y] = value;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (!needsUpdate)
                return;

            needsUpdate = false;

            var gd = Main.instance.GraphicsDevice;
            if (texture == null)
            {
                texture = new RenderTarget2D(gd, width * TILE_SIZE, height * TILE_SIZE);
            }

            Renderer.StartSpriteBatch(sb, true);
            gd.SetRenderTarget(texture);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < Neighbors.Length; k++)
                    {
                        int ox = j - Neighbors[k].x;
                        int oy = i - Neighbors[k].y;
                        if(ox >= 0 && oy >= 0)
                        CheckSurroundingTiles(ox, oy);
                    }
                    Draw(sb, j, i);
                }
            }

            sb.End();
            gd.SetRenderTarget(null);
        }

        private void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            Vector2 position = new Vector2(x, y);

            spriteBatch.Draw(Assets.GetTexture("frame"), position * TILE_SIZE, null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            if (tiles[x, y] == 0)
            {
                spriteBatch.Draw(Assets.GetTexture("DirtTile"), position * TILE_SIZE, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                return;
            }

            Texture2D tex = Assets.GetTexture("Template2");

            for (int i = 0; i < Neighbors.Length; i++)
            {
                int ox = Math.Clamp(x + Neighbors[i].x, 0, width - 1);
                int oy = Math.Clamp(y + Neighbors[i].y, 0, height - 1);
                int nx = Math.Clamp(x - Neighbors[i].x, 0, width - 1);
                int ny = Math.Clamp(y - Neighbors[i].y, 0, height - 1);

                int dx = (int)OffsetMapping[displayedTiles[ox, oy]].X;
                int dy = (int)OffsetMapping[displayedTiles[ox, oy]].Y;

                Vector2 off = new Vector2(ox, oy);

                var tl = displayedTiles[ox, oy];

                Rectangle src = new Rectangle(dx * TILE_SIZE + 1 * dx, dy * TILE_SIZE + 1 * dy, TILE_SIZE, TILE_SIZE);

                spriteBatch.Draw(tex, new Vector2(ox, oy) * TILE_SIZE - new Vector2(TILE_SIZE) * 0.5f, src, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
        }

        public void CheckSurroundingTiles(int x, int y)
        {
            int myId = tiles[x, y];

            bool[] sameAsMe = new bool[Neighbors.Length];

            for(int i = 0; i < Neighbors.Length; i++)
            {
                int otherX = x - Neighbors[i].x;
                int otherY = y - Neighbors[i].y;

                int otherTile = myId;

                if (otherX >= 0 && otherY >= 0)
                {
                    otherTile = tiles[otherX, otherY];
                }

                if (otherTile == myId)
                {
                    sameAsMe[i] = true;
                }
            }

            var key = CheckConnectionType.Keys.First(
                x 
                => x[0] != sameAsMe[3]
                && x[1] != sameAsMe[2]
                && x[2] != sameAsMe[1]
                && x[3] != sameAsMe[0]
            );

            CheckConnectionType.TryGetValue(key, out var conType);

            displayedTiles[x, y] = conType;
        }

        public struct IntVec : IEquatable<IntVec>
        {
            public int x; public int y;

            public IntVec(int x, int y)
            {
                this.x = x; this.y = y;
            }

            public bool Equals(IntVec other)
            {
                return x == other.x && y == other.y;
            }

            public override string ToString()
            {
                return $"{x}, {y}";
            }
        }

        public static IntVec[] Neighbors =
        {
            new(1, 1), // top left         [1 1]
            new(1, 0), // bottom left    [1 0]
            new(0, 1), // top right      [0 1]
            new(0, 0) // bottom right     [0 0]
        };

        public Dictionary<bool[], TileConnectionType> CheckConnectionType = new Dictionary<bool[], TileConnectionType>
        {
            {[false, false, false, false], TileConnectionType.None },
            {[true, true, true, true], TileConnectionType.All },
            {[true, true, false, false], TileConnectionType.EdgeTop },
            {[false, false, true, true], TileConnectionType.EdgeBottom },
            {[true, false, true, false], TileConnectionType.EdgeLeft },
            {[false, true, false, true], TileConnectionType.EdgeRight },
            {[true, true, true, false], TileConnectionType.InnerCornerBottomRight },
            {[true, true, false, true], TileConnectionType.InnerCornerBottomLeft },
            {[true, false, true, true], TileConnectionType.InnerCornerTopRight },
            {[false, true, true, true], TileConnectionType.InnerCornerTopLeft },
            {[false, true, false, false], TileConnectionType.OuterCornerTopRight },
            {[true, false, false, false], TileConnectionType.OuterCornerTopLeft },
            {[false, false, false, true], TileConnectionType.OuterCornerBottomRight },
            {[false, false, true, false], TileConnectionType.OuterCornerBottomLeft },
            {[true, false, false, true], TileConnectionType.JunctionBottomRightTopLeft },
            {[false, true, true, false], TileConnectionType.JunctionBottomLeftTopRight },

        };

        public static Dictionary<TileConnectionType, Vector2> OffsetMapping = new Dictionary<TileConnectionType, Vector2>
            {
            { TileConnectionType.None, Vector2.Zero},
            { TileConnectionType.All, new Vector2(0, 1)},
            { TileConnectionType.EdgeTop, new Vector2(1, 1)},
            { TileConnectionType.EdgeBottom, new Vector2(1, 0)},
            { TileConnectionType.EdgeLeft, new Vector2(3, 0)},
            { TileConnectionType.EdgeRight, new Vector2(2, 0)},
            { TileConnectionType.InnerCornerTopRight, new Vector2(0, 4)},
            { TileConnectionType.InnerCornerTopLeft, new Vector2(1, 4)},
            { TileConnectionType.InnerCornerBottomRight, new Vector2(0, 2)},
            { TileConnectionType.InnerCornerBottomLeft, new Vector2(1, 2)},
            { TileConnectionType.OuterCornerTopRight, new Vector2(3, 2)},
            { TileConnectionType.OuterCornerTopLeft, new Vector2(3, 3)},
            { TileConnectionType.OuterCornerBottomRight, new Vector2(2, 2)},
            { TileConnectionType.OuterCornerBottomLeft, new Vector2(2, 3)},
            { TileConnectionType.JunctionBottomLeftTopRight, new Vector2(2, 1)},
            { TileConnectionType.JunctionBottomRightTopLeft, new Vector2(3, 1)},
        };
    }

    public enum TileConnectionType
    {
        None,
        All,
        EdgeTop,
        EdgeBottom,
        EdgeLeft,
        EdgeRight,
        InnerCornerBottomRight,
        InnerCornerBottomLeft,
        InnerCornerTopRight,
        InnerCornerTopLeft,
        OuterCornerBottomRight,
        OuterCornerBottomLeft,
        OuterCornerTopRight,
        OuterCornerTopLeft,
        JunctionBottomLeftTopRight,
        JunctionBottomRightTopLeft,
    }
}
