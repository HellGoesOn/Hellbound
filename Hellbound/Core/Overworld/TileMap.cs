using Casull.Extensions;
using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.Overworld
{
    public class TileMap
    {
        public bool showGrid;

        public readonly static Dictionary<string, TileDefinition> TileDefinitions = [];

        public static TileDefinition GetById(int id)
        {
            return TileDefinitions.Values.First(x => x.id == id);
        }

        public static void Init()
        {
            DefineTile("Stone", 100, "Stone", offset: Vector2.Zero);
            DefineTile("Grass", 5, "VoidBase", "GrassTile", Vector2.Zero);
            DefineTile("Path", 6, "VoidBase", "Path2", Vector2.Zero);
            DefineTile("Road", 2, "VoidBase", "RoadTile", Vector2.Zero);
            DefineTile("DarkGrass", 1, "VoidBase", "DarkGrass", Vector2.Zero);
            DefineTile("Cliff", 0, "VoidBase", "CliffTest", new Vector2(0, -16), 1);
            DefineTile("StoneWall", 2, "VoidBase", "StoneWall", new Vector2(0, -16), 1);
            DefineTile("Void", 100, "VoidBase");
            DefineTile("Pond", 0, "VoidBase", "PondTile", param: ShaderParam.Water);
        }

        public static TileDefinition DefineTile(string name, int weight, string baseTexture, string tileTexture = "NULLA TERRA", Vector2 offset = default, int elevation = 0, ShaderParam param = ShaderParam.None)
        {
            if (tileTexture != "NULLA TERRA") {
                TileDefinition newTile = new(TileDefinitions.Count, weight, baseTexture, tileTexture, offset, param: param) {
                    elevation = elevation
                };
                TileDefinitions.Add(name, newTile);
                return newTile;
            }
            else {
                TileDefinition newTile = new(TileDefinitions.Count, weight, baseTexture, offset, param: param) {
                    elevation = elevation
                };
                TileDefinitions.Add(name, newTile);
                return newTile;
            }

        }

        public int width;
        public int height;

        public bool didBakeTexture;
        public RenderTarget2D bakedTexture;

        private int[,] _physicalTiles;
        private int[,] _elevationMap;
        public int[,] ElevationMap => _elevationMap;
        private List<DisplayTileLayer> _displayTiles;

        public TileMap(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.didBakeTexture = false;

            _physicalTiles = new int[width, height];
            _elevationMap = new int[width, height];
            _displayTiles = [];

            foreach (var tile in TileDefinitions.Values.OrderBy(x => x.weight)) {
                if (tile.sheetName != tile.textureName) {
                    _displayTiles.Add(new DisplayTileLayer(tile, width, height) {
                        baseTexture = Assets.GetTexture(tile.textureName),
                        tileSheet = Assets.GetTexture(tile.sheetName)
                    });
                }
            }

            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    SetTile(TileDefinitions["Stone"], j, i);
                }
            }
        }

        public int GetTile(int x, int y) => _physicalTiles[x, y];
        public int GetTileElevation(int x, int y) => _elevationMap[x, y];

        public void SetTile(TileDefinition tile, int x, int y)
        {
            int cx = Math.Clamp(x, 0, width - 1);
            int cy = Math.Clamp(y, 0, height - 1);
            _physicalTiles[cx, cy] = tile.id;
            foreach (var layer in _displayTiles) {
                layer.SetTile(tile, cx, cy);
            }
            didBakeTexture = false;
        }

        public void SetTileElevation(int elevation, int x, int y)
        {
            _elevationMap[x, y] = elevation;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    Texture2D tex = Assets.GetTexture(GetById(_physicalTiles[j, i]).textureName);
                    Renderer.Draw(tex, new Vector2(j * DisplayTileLayer.TILE_SIZE, i * DisplayTileLayer.TILE_SIZE).ToInt(), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, _elevationMap[j, i]);

                    if (showGrid) {
                        Renderer.Draw(Assets.GetTexture("Frame2"), new Vector2(j * DisplayTileLayer.TILE_SIZE, i * DisplayTileLayer.TILE_SIZE), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1000f);
                        Renderer.Draw(Assets.GetTexture("TileElevation"), new Vector2(j * DisplayTileLayer.TILE_SIZE, i * DisplayTileLayer.TILE_SIZE) + new Vector2(8), new Rectangle(0, 16 * GetTileElevation(j, i), 16, 16), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1000f);
                    }
                }
            }

            foreach (var displayLayer in _displayTiles) {
                displayLayer.Draw();
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
        public ShaderParam param = ShaderParam.None;

        public TileDefinition(int id, int weight, string textureName, Vector2 drawOffset, ShaderParam param = ShaderParam.None)
        {
            this.id = id;
            this.weight = weight;
            this.sheetName = this.textureName = textureName;
            this.elevation = 0;
            this.drawOffset = drawOffset;
            this.param = param;
        }

        public TileDefinition(int id, int weight, string textureName, string sheetName, Vector2 drawOffset, ShaderParam param = ShaderParam.None)
        {
            this.id = id;
            this.weight = weight;
            this.sheetName = sheetName;
            this.textureName = textureName;
            this.elevation = 0;
            this.drawOffset = drawOffset;
            this.param = param;
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

        public Color clr = Color.White;
        public Action<DisplayTileLayer> onDraw;
        public ShaderParam shaderParam = ShaderParam.None;


        public DisplayTileLayer(TileDefinition tileId, int width, int height)
        {
            myTile = tileId;
            baseTexture = Assets.GetTexture("Pixel");
            weight = 0;
            this.width = width;
            this.height = height;
            _tiles = new int[this.width, this.height];
            elevationMap = new int[this.width + 1, this.height + 1];
            _displayedTiles = new Vector2[this.width + 1, this.height + 1];
        }

        public void Draw()
        {
            onDraw?.Invoke(this);

            for (int i = 0; i < height + 1; i++) {
                for (int j = 0; j < width + 1; j++) {
                    int x = (int)MathF.Floor(_displayedTiles[j, i].X);
                    int y = (int)MathF.Floor(_displayedTiles[j, i].Y);
                    float depth = i * TILE_SIZE + (elevationMap[j, i] * TILE_SIZE + 12 * elevationMap[j, i]) + myTile.weight * 0.0001f + myTile.drawOffset.Y;
                    Rectangle rect = new(x * TILE_SIZE + 1 * x, y * TILE_SIZE + 1 * y, TILE_SIZE, TILE_SIZE);
                    Vector2 pos = (new Vector2(j, i) * TILE_SIZE - (new Vector2(TILE_SIZE) * 0.5f) + myTile.drawOffset).ToInt();
                    Renderer.Draw(tileSheet, pos, rect, clr, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, depth, myTile.param);
                }
            }
        }

        public void SetTile(TileDefinition tile, int x, int y)
        {
            _tiles[x, y] = tile.id == myTile.id ? 1 : 0;

            SetDisplayTile(x, y, tile.id == myTile.id ? tile.elevation : -1);
        }

        public void SetDisplayTile(int x, int y, int elevation = 0)
        {
            for (int i = 0; i < Neighbors.Length; i++) {
                int xx = x + (int)Neighbors[i].X;
                int yy = y + (int)Neighbors[i].Y;
                int cx = Math.Clamp(xx, 0, width);
                int cy = Math.Clamp(yy, 0, height);

                if (elevation != -1)
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

        public readonly static Dictionary<TilePattern, Vector2> PatternMapping = new()
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

        public readonly bool Equals(TilePattern other)
        {
            for (int i = 0; i < pattern.Length; i++) {
                if (other.pattern[i] != pattern[i])
                    return false;
            }
            return true;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is TilePattern tile && Equals(tile);
        }

        public override readonly int GetHashCode()
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
