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
    public class TileMap
    {
        public const int TILE_SIZE = 32;

        public static Dictionary<string, int> Tiles = [];

        public static void Initialize()
        {
            Tiles.Add("Grass", 0);
            Tiles.Add("Stone", 1);
            Tiles.Add("Dirt", 2);
        }

        public static void Unload()
        {
            Tiles.Clear();
            Tiles = null;
        }

        public int width;
        public int height;
        public int[,] tiles;
        public bool bakedTexture;
        public bool needsUpdate;

        public RenderTarget2D texture;

        public int this[int x, int y]
        {
            get => tiles[x, y];
            set => tiles[x, y] = value;
        }

        public TileMap(int width, int height)
        {
            this.width = width;
            this.height = height;
            tiles = new int[this.width, this.height];
            needsUpdate = true;
            for(int i = 0; i < this.width; i++)
            {
                for(int j = 0; j < this.height; j++)
                {
                    tiles[i, j] = 0;
                }
            }
        }

        public void ChangeTile(int newTile, int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                this.tiles[x, y] = newTile;
                needsUpdate = true;
            }
        }

        public void BakeMap()
        {
            needsUpdate = false;
            var tileTexture = Assets.Textures["Tileset"];
            var gd = Main.instance.GraphicsDevice;
            if (texture == null)
            {
                texture = new RenderTarget2D(gd, width * TILE_SIZE, height * TILE_SIZE);
            }

            var spriteBatch = Main.instance.spriteBatch;

            Renderer.StartSpriteBatch(spriteBatch, true);
            gd.SetRenderTarget(texture);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int tileType = tiles[i, j];
                    spriteBatch.Draw(tileTexture, new Vector2(i, j) * TILE_SIZE, new Rectangle((TILE_SIZE * tileType + 1 * tileType), 0, TILE_SIZE, TILE_SIZE), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                    if (tileType != 1)
                    {
                        List<int> offset = [];
                        if (i - 1 >= 0 && tiles[i - 1, j] == 1)
                        {
                            offset.Add(2);
                        }

                        if (j - 1 >= 0 && tiles[i, j - 1] == 1)
                        {
                            offset.Add(4);
                        }

                        if (i + 1 < width && tiles[i + 1, j] == 1)
                        {
                            offset.Add(1);
                        }

                        if (j + 1 < height && tiles[i, j + 1] == 1)
                        {
                            offset.Add(3);
                        }

                        foreach (int off in offset)
                            spriteBatch.Draw(tileTexture, new Vector2(i, j) * TILE_SIZE, new Rectangle((TILE_SIZE + 1), TILE_SIZE * off + off, TILE_SIZE, TILE_SIZE), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    }
                }
            }
            spriteBatch.End();
            gd.SetRenderTarget(null);
            bakedTexture = true;
        }
    }
}
