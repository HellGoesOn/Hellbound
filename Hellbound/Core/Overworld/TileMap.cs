using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Overworld
{
    [Obsolete("Class is obsolete, use NewerTileMap")]
    public class TileMap
    {
        public const int TILE_SIZE = 16;

        public static Dictionary<string, Tile> Tiles = [];
        public static Dictionary<int, Tile> TilesById = [];

        public static void AddTile(string name, string texture, int id, int weight)
        {
            Tile tile = new Tile(texture, id, weight);
            Tiles.Add(name, tile);
            TilesById.Add(id, tile);
        }

        public static void Initialize()
        {
            AddTile("Grass", "GrassTile", 0, 0);
            AddTile("Stone", "StoneTiles", 1, 20);
            AddTile("Dirt", "DirtTiles", 2, 10);
            AddTile("Pathing", "PathTile", 3, 5);
        }

        public static void Unload()
        {
            Tiles.Clear();
            Tiles = null;
        }

        public int width;
        public int height;
        public Tile[,] tiles;
        public bool bakedTexture;
        public bool needsUpdate;

        public RenderTarget2D texture;

        public Tile this[int x, int y]
        {
            get => tiles[x, y];
            set => tiles[x, y] = value;
        }

        public TileMap(int width, int height)
        {
            this.width = width;
            this.height = height;
            tiles = new Tile[this.width, this.height];
            needsUpdate = true;
            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    tiles[i, j] = Tiles["Dirt"];
                }
            }
        }

        public void ChangeTile(Tile newTile, int x, int y)
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
            var gd = Main.instance.GraphicsDevice;
            if (texture == null)
            {
                texture = new RenderTarget2D(gd, width * TILE_SIZE, height * TILE_SIZE);
            }

            var spriteBatch = Main.instance.spriteBatch;

            Renderer.StartSpriteBatch(spriteBatch, true, state: SamplerState.PointWrap);
            gd.SetRenderTarget(texture);
            gd.Clear(Color.Transparent);
            //spriteBatch.Draw(Assets.GetTexture("DirtTyile"), Vector2.Zero, new Rectangle(0, 0, TILE_SIZE * width, TILE_SIZE * height), Color.White); 
            //spriteBatch.Draw(Assets.GetTexture("GrassTile"), Vector2.Zero, new Rectangle(34, 17, TILE_SIZE, TILE_SIZE), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            Renderer.StartSpriteBatch(spriteBatch, true, sortMode: SpriteSortMode.Deferred);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Tile tileType = tiles[j, i];
                    spriteBatch.Draw(Assets.GetTexture("Tiles8"), new Vector2(j, i) * TILE_SIZE, new Rectangle((TILE_SIZE * tileType.id + 1 * tileType.id), 0, TILE_SIZE, TILE_SIZE), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    List<int> offset = [];

                }
            }

            List<Tile> tileList = Tiles.Values.OrderBy(x => x.weight).ToList();

            if(Input.HeldKey(Keys.LeftShift))
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        SetDisplayTile(tiles[i, j], i, j, spriteBatch);
                    }
                }   
            
        
            spriteBatch.End();
            gd.SetRenderTarget(null);
            bakedTexture = true;
        }

        public void SetDisplayTile(Tile tileType, int i, int j, SpriteBatch spriteBatch)
        {
            for (int k = 0; k < Neighbours.Length; k++)
            {
                var tileTexture = Assets.GetTexture(tileType.name);
                Vector2 off = GetOffsetFromNeighbors(0, i + (int)Neighbours[k].X, j + (int)Neighbours[k].Y);
                Vector2 halfSize = new Vector2(TILE_SIZE) * 0.5f;
                Vector2 position = new Vector2(i + (int)Neighbours[k].X, j + (int)Neighbours[k].Y) * TILE_SIZE - halfSize;
                int x = (int)off.X;
                int y = (int)off.Y;

                spriteBatch.Draw(tileTexture, position, new Rectangle(TILE_SIZE * x + 1 * x, TILE_SIZE * y + 1 * y, TILE_SIZE, TILE_SIZE), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
        }

        public Vector2 GetOffsetFromNeighbors(int myId, int x, int y)
        {
            Tile[] neighbours =
                [
                    new("Null", myId, -1),
                    new("Null", myId, -1),
                    new("Null", myId, -1),
                    new("Null", myId, -1),
                ];

            for (int i = neighbours.Length-1; i >=0; i--)
            {
                int otherX = x - (int)Neighbours[i].X;
                int otherY = y - (int)Neighbours[i].Y;
                if (otherY >= 0 && otherX >= 0 && otherX < width && otherY < height)
                {
                    neighbours[i] = tiles[otherX, otherY];
                }
            }

            TileCheckerSequence sequence =
                new TileCheckerSequence(
                    [
                    neighbours[0].id != myId,
                    neighbours[1].id != myId,
                    neighbours[2].id != myId,
                    neighbours[3].id != myId,
                ]
                    );
            

            var key = TileLookUpTable.Keys.First(x =>
            x.ids[0] == sequence.ids[3]
            && x.ids[1] == sequence.ids[2]
            && x.ids[2] == sequence.ids[1]
            && x.ids[3] == sequence.ids[0]);

            return TileLookUpTable[key];
        }

        public static Vector2[] Neighbours = new Vector2[]
        {
            new(0, 0),
            new(1, 0),
            new(0, 1),
            new(1, 1)
        };

        /// <summary>
        /// general idea & values taken from here:
        /// https://www.youtube.com/watch?v=jEWFSv3ivTg
        /// </summary>
        public static Dictionary<TileCheckerSequence, Vector2> TileLookUpTable = new Dictionary<TileCheckerSequence, Vector2>()
{
    { new TileCheckerSequence([false, false, false, false]), new Vector2(2, 1) }, // all corners
    { new TileCheckerSequence([true, true, true, false]), new Vector2(1, 3) }, // outer bottom right corner
    { new TileCheckerSequence([true, true, false, true]), new Vector2(0, 0) }, // outer bottom left corner
    { new TileCheckerSequence([true, false, true, true]), new Vector2(0, 2) }, // outer top right
    { new TileCheckerSequence([false, true, true, true]), new Vector2(3, 3) }, // outer top left
    { new TileCheckerSequence([true, false, true, false]), new Vector2(1, 0) }, // right edge
    { new TileCheckerSequence([false, true, false, true]), new Vector2(3, 2) }, // left edge
    { new TileCheckerSequence([true, true, false, false]), new Vector2(3, 0) }, // bottom edge
    { new TileCheckerSequence([false, false, true, true]), new Vector2(1, 2) }, // top edge
    { new TileCheckerSequence([true, false, false, false]), new Vector2(1, 1) }, // inner bottom right
    { new TileCheckerSequence([false, true, false, false]), new Vector2(2, 0) }, // inner bottom left
    { new TileCheckerSequence([false, false, true, false]), new Vector2(2, 2) }, // inner top right
    { new TileCheckerSequence([false, false, false, true]), new Vector2(3, 1) }, // inner top left
    { new TileCheckerSequence([true, false, false, true]), new Vector2(2, 3) }, // bottom left to top right
    { new TileCheckerSequence([false, true, true, false]), new Vector2(0, 1) }, // top left to down right 
    { new TileCheckerSequence([true, true, true, true]), new Vector2(0, 3) }, // no corners
};

        public struct TileCheckerSequence
        {
            public bool[] ids;

            public TileCheckerSequence(bool[] ids)
            {
                this.ids = ids;
            }
        }

        public struct Tile
        {
            public readonly string name;
            public int id;
            public int weight;

            public Tile(string name, int id, int weight)
            {
                this.name = name;
                this.id = id;
                this.weight = weight;
            }
        }
    }
}
