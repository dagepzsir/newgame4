using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using UnityEngine;
using Assets.Scripts;
namespace Assets.Scripts
{
    public enum PlayerPrefEnum
    {
        ParentCoords,
        ChunkCoords,
        MainFilePath
    }

    [ProtoContract]
    public class Chunk
    {
        [ProtoMember(1)]
        public Dictionary<Vec2Ser, Dictionary<Vec2Ser, MiniTileData>> ChunkData = new Dictionary<Vec2Ser, Dictionary<Vec2Ser, MiniTileData>>();
        public Chunk() { }
        private Dictionary<Vec2Ser, MiniTileData> CreateAMicroMap(int size, MacroTile parent)
        {
            Dictionary<Vec2Ser, MiniTileData> tempDic = new Dictionary<Vec2Ser, MiniTileData>();
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    MiniTileData temp = new MiniTileData(parent.Height, parent.TileCategory);
                    tempDic.Add(new Vec2Ser(x, y), temp);
                }
            }
            return tempDic;
        }
        public Chunk(int subsize, MacroTile parent, Dictionary<Vec2Ser, MacroTile> tiledict)
        {
            foreach (KeyValuePair<Vec2Ser, MacroTile> value in tiledict)
            {
                if (parent.ChunkCoordinates == value.Value.ChunkCoordinates)
                {
                    ChunkData.Add(value.Value.Coordinates, CreateAMicroMap(subsize, value.Value));
                }
            }
        }
        public void UpdateChunk(Vec2Ser parentcoord, Dictionary<Vec2Ser, MiniTile> tiledict)
        {
            Dictionary<Vec2Ser, MiniTileData> mappedData = new Dictionary<Vec2Ser, MiniTileData>();
            foreach(KeyValuePair<Vec2Ser, MiniTile> pairs in tiledict)
            {
                mappedData.Add(pairs.Key, new MiniTileData(pairs.Value));
            }

            ChunkData[parentcoord] = mappedData;
        }

    }
    [ProtoContract]
    public class Map
    {
        public Map() { }
        public Map(int size)
        {
            CreateMap(size);
        }

        [ProtoMember(1)]
        public Dictionary<Vec2Ser, MacroTileData> MapData = new Dictionary<Vec2Ser, MacroTileData>();
        private void CreateMap(int size, float seed = 0)
        {
            if (seed == 0)
                seed = UnityEngine.Random.Range(0.0f, 114.0f);
            for(int y = 0; y < size; y++)
            {
                for(int x = 0; x < size; x++)
                {
                    MacroTileData temp = new MacroTileData(Mathf.PerlinNoise(0.1f * x + seed, 0.1f * y + seed));
                    MapData.Add(new Vec2Ser(x, y), temp);
                }
            }
        }
    }

    [ProtoContract]
    public struct Vec2Ser
    {
        [ProtoMember(1)]
        public float X;
        [ProtoMember(2)]
        public float Y;
        public Vec2Ser(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Vec2Ser(string vectorstring)
        {
            string[] splitted = vectorstring.Split('_');
            X = float.Parse(splitted[0]);
            Y = float.Parse(splitted[1]);
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("{0}_{1}", X, Y);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public static bool operator ==(Vec2Ser x, Vec2Ser y)
        {
            return x.X == y.X && x.Y == y.Y;
        }
        public static bool operator !=(Vec2Ser x, Vec2Ser y)
        {
            return !(x == y);
        }

        public static bool operator ==(Vec2Ser x, Vector2 y)
        {
            return x.X == y.x && x.Y == y.y;
        }
        public static bool operator !=(Vec2Ser x, Vector2 y)
        {
            return !(x == y);
        }
    }

    [ProtoContract]
    public class MacroTileData
    {
        [ProtoMember(1)]
        public float Height { get; set; }

        [ProtoMember(2)]
        public MacroTileCategory TileCategory;

        public MacroTileData(float height)
        {
            Height = height;


            //Set TileCategory
            if (height <= 0.25)
                TileCategory = MacroTileCategory.Water;
            else if (height <= 0.5 && height > 0.25)
                TileCategory = MacroTileCategory.Grass;
            else if (height <= 0.70 && height > 0.5)
                TileCategory = MacroTileCategory.Forest;
            else if (height > 0.70)
                TileCategory = MacroTileCategory.Mountain;
        }
        public MacroTileData() { }
    }
    [ProtoContract]
    public class MiniTileData
    {
        [ProtoMember(1)]
        public float Height { get; set; }
        [ProtoMember(2)]
        public MacroTileCategory TileCategory;
        [ProtoMember(3)]
        public MiniTileCategory MiniCategory;
        [ProtoMember(4)]
        public int Level;
        public MiniTileData(float height, MacroTileCategory categ)
        {
            Height = height;
            TileCategory = categ;
            MiniCategory = MiniTileCategory.None;
            Level = -1;
        }
        public MiniTileData(MiniTile miniTile)
        {
            Height = miniTile.Height;
            TileCategory = miniTile.TileCategory;
            MiniCategory = miniTile.MiniCategory;
            Level = miniTile.Level;
        }

        public MiniTileData() { }
    }
}