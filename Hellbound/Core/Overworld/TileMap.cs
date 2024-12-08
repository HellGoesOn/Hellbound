using HellTrail.Core.ECS;
using HellTrail.Core.ECS.Components;
using HellTrail.Core.UI;
using HellTrail.Extensions;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Overworld
{
    public class TileMap
    {
        public readonly static Dictionary<string, TileDefinition> TileDefinitions = [];

        public static TileDefinition GetById(int id)
        {
            return TileDefinitions.Values.First(x => x.id == id);
        }

        public static void Init()
        {
            TileDefinitions.Add("Stone", new(1, 100, "Stone", Vector2.Zero));
            TileDefinitions.Add("Path", new(2, 6, "GrassBase", "Path2", Vector2.Zero));
            TileDefinitions.Add("Grass", new(3, 5, "GrassBase", "GrassTile", Vector2.Zero));
            TileDefinitions.Add("DarkGrass", new(4, 1, "GrassBase", "DarkGrass", Vector2.Zero));
            TileDefinitions.Add("Cliff", new(5, 0, "GrassBase", "CliffTest", new Vector2(0, -16))
            {
                elevation = 1
            }); 
            TileDefinitions.Add("StoneWall", new(6, 2, "Stone", "StoneWall", new Vector2(0, -16))
            {
                elevation = 1
            });
        }

        public int width;
        public int height;

        public bool didBakeTexture;
        public RenderTarget2D bakedTexture;

        private int[,] _physicalTiles;
        private int[,] _elevationMap;
        private List<DisplayTileLayer> _displayTiles;

        public TileMap(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.didBakeTexture = false;

            _physicalTiles = new int[width, height];
            _elevationMap = new int[width, height];
            _displayTiles = [];

            foreach (var tile in TileDefinitions.Values.OrderBy(x => x.weight))
            {
                if (tile.sheetName != tile.textureName)
                {
                    _displayTiles.Add(new DisplayTileLayer(tile, width, height)
                    {
                        baseTexture = Assets.GetTexture(tile.textureName),
                        tileSheet = Assets.GetTexture(tile.sheetName)
                    });
                }
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    SetTile(TileDefinitions["Stone"], j, i);
                }
            }
        }

        public int GetTile(int x, int y) => _physicalTiles[x, y];
        public int GetTileElevation(int x, int y) => _elevationMap[x, y];

        public void SetTile(TileDefinition tile, int x, int y, int elevation = 0)
        {
            int cx = Math.Clamp(x, 0, width - 1);
            int cy = Math.Clamp(y, 0, height - 1);
            _physicalTiles[cx, cy] = tile.id;
            _elevationMap[cx, cy] = tile.elevation;
            foreach (var layer in _displayTiles)
            {
                layer.SetTile(tile, cx, cy, true);
            }
            didBakeTexture = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Texture2D tex = Assets.GetTexture(GetById(_physicalTiles[j, i]).textureName);
                    Renderer.Draw(tex, new Vector2(j * DisplayTileLayer.TILE_SIZE, i * DisplayTileLayer.TILE_SIZE).ToInt(), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, _elevationMap[j, i]);
                    //Renderer.Draw(Assets.GetTexture("Frame2"), new Vector2(j * DisplayTileLayer.TILE_SIZE, i * DisplayTileLayer.TILE_SIZE), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
                }
            }

            foreach (var displayLayer in _displayTiles)
            {
                displayLayer.Draw(spriteBatch);
            }

        }

    }

    public struct TileDefinition
    {
        public int id;
        public int weight;
        public readonly string textureName;
        public readonly string sheetName;
        public int elevation;
        public Vector2 drawOffset;

        public TileDefinition(int id, int weight, string textureName, Vector2 drawOffset)
        {
            this.id = id;
            this.weight = weight;
            this.sheetName = this.textureName = textureName;
            this.elevation = 0;
            this.drawOffset = drawOffset;
        }

        public TileDefinition(int id, int weight, string textureName, string sheetName, Vector2 drawOffset)
        {
            this.id = id;
            this.weight = weight;
            this.sheetName = sheetName;
            this.textureName = textureName;
            this.elevation = 0;
            this.drawOffset = drawOffset;
        }
    }

    public class DisplayTileLayer
    {
        public const int TILE_SIZE = 32;

        public TileDefinition myTile;
        public int width;
        public int height;
        public int weight;

        public Texture2D baseTexture;
        public Texture2D tileSheet;

        private int[,] _tiles;
        private Vector2[,] _displayedTiles;
        public int[,] elevationMap;

        public DisplayTileLayer(TileDefinition tileId, int width, int height)
        {
            myTile = tileId;
            baseTexture = Assets.GetTexture("Pixel");
            weight = 0;
            this.width = width;
            this.height = height;
            _tiles = new int[this.width, this.height];
            elevationMap = new int[this.width+1, this.height+1];
            _displayedTiles = new Vector2[this.width+1, this.height+1];
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < height+1; i++)
            {
                for (int j = 0; j < width + 1; j++)
                {
                    int x = (int)_displayedTiles[j, i].X;
                    int y = (int)_displayedTiles[j, i].Y;
                    int ex = Math.Clamp(j, 0, width - 1);
                    int ey = Math.Clamp(i, 0, height - 1);
                    float depth = i * TILE_SIZE + (elevationMap[j, i] * TILE_SIZE + 12 * elevationMap[j, i]) + myTile.weight * 0.0001f + myTile.drawOffset.Y;
                    Rectangle rect = new(x * TILE_SIZE + 1 * x, y * TILE_SIZE + 1 * y, TILE_SIZE, TILE_SIZE);
                    Vector2 pos = new Vector2(j, i).ToInt() * TILE_SIZE - (new Vector2(TILE_SIZE) * 0.5f).ToInt() + myTile.drawOffset.ToInt();
                    Renderer.Draw(tileSheet, pos, rect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, depth);

                }
            }
        }

        public void SetTile(TileDefinition tile, int x, int y, bool setDisplayTile = false)
        {
            _tiles[x, y] = tile.id == myTile.id ? 1 : 0;
                    
            SetDisplayTile(x, y, tile.id == myTile.id ? tile.elevation : -1);
        }

        public void SetDisplayTile(int x, int y, int elevation = 0)
        {
            for (int i = 0; i < Neighbors.Length; i++)
            {
                int xx = x + (int)Neighbors[i].X;
                int yy = y + (int)Neighbors[i].Y;
                int cx = Math.Clamp(xx, 0, width);
                int cy = Math.Clamp(yy, 0, height);

                if(elevation != -1)
                    elevationMap[cx, cy] = elevation;
                _displayedTiles[cx, cy] = GetDisplayTile(xx, yy);
            }
        }

        public int GetTile(int x, int y)
        {
            int cx = Math.Clamp(x, 0, this.width - 1);
            int cy = Math.Clamp(y, 0, this.height - 1);

            return _tiles[cx, cy];
        }

        public int GetTile(Vector2 tilePos)
        {
            return GetTile((int)tilePos.X, (int)tilePos.Y);
        }

        public Vector2 GetDisplayTile(int x, int y)
        {
            int topLeft = GetTile(new Vector2(x, y) - Neighbors[0]);
            int topRight = GetTile(new Vector2(x, y) - Neighbors[1]);
            int bottomLeft = GetTile(new Vector2(x, y) - Neighbors[2]);
            int bottomRight = GetTile(new Vector2(x, y) - Neighbors[3]);

            return PatternMapping[new(topLeft, topRight, bottomLeft, bottomRight)];
        }

        public static Vector2[] Neighbors =
        [
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
        ];

        public static Dictionary<TilePattern, Vector2> PatternMapping = new Dictionary<TilePattern, Vector2>()
            {
                {new(1, 1,
                    1, 1), new Vector2(2, 1) }, // all corners
                {new(0, 0,
                    0, 0), new Vector2(0, 3) }, // no corners
                {new(0, 0,
                    0, 1), new Vector2(1, 3) }, // outer bottom right
                {new(0, 0,
                    1, 0), new Vector2(0, 0) }, // outer bottom left
                {new(1, 0,
                    0, 0), new Vector2(3, 3) }, // outer top left
                {new(0, 1,
                    0, 0), new Vector2(0, 2) }, // outer top right
                {new(0, 1,
                    1, 1), new Vector2(1, 1) }, // inner top left
                {new(1, 0,
                    1, 1), new Vector2(2, 0) }, // inner top right
                {new(1, 1,
                    0, 1), new Vector2(2, 2) }, // inner bottom left
                {new(1, 1,
                    1, 0), new Vector2(3, 1) }, // inner bottom right
                {new(1, 0,
                    1, 0), new Vector2(3, 2) }, // edge left
                {new(0, 1,
                    0, 1), new Vector2(1, 0) }, // edge right
                {new(1, 1,
                    0, 0), new Vector2(1, 2) }, // edge top
                {new(0, 0,
                    1, 1), new Vector2(3, 0) }, // edge bottom  
                {new(1, 0,
                    0, 1), new Vector2(0, 1) }, // top-left to bottom-right
                {new(0, 1,
                    1, 0), new Vector2(2, 3)  } // top-right to bottom-left
            };
    }

    public struct TilePattern : IEquatable<TilePattern>
    {
        public int[] pattern;
        public TilePattern(int topLeft, int topRight, int bottomLeft, int bottomRight)
        {
            pattern = [topLeft, topRight, bottomLeft, bottomRight];
        }

        public bool Equals(TilePattern other)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                if (other.pattern[i] != pattern[i])
                    return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is TilePattern && Equals((TilePattern)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(pattern[0], pattern[1], pattern[2], pattern[3]);
        }

        public static bool operator ==(TilePattern left, TilePattern right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TilePattern left, TilePattern right)
        {
            return !(left == right);
        }
    }
}
