using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts;
using ProtoBuf;
public class SaveObjectManager : MonoBehaviour {

    public Dictionary<Vec2Ser, Chunk> Chunks = new Dictionary<Vec2Ser,Chunk>();
    public Map Map;


    public void SaveChunk(Vec2Ser chunkcoord, Chunk chunk)
    {
        string filePath = PlayerPrefs.GetString(PlayerPrefEnum.MainFilePath.ToString());
        using (FileStream file = File.Create(filePath + chunkcoord.ToString() + ".chunk"))
        {
            Serializer.Serialize(file, chunk);
        }
    }
    public void SaveAllChunks()
    {
        foreach(KeyValuePair<Vec2Ser, Chunk> pair in Chunks)
        {
            SaveChunk(pair.Key, pair.Value);
        }
    }
    public Chunk LoadChunk(Vec2Ser chunkcoord)
    {
        string filePath = PlayerPrefs.GetString(PlayerPrefEnum.MainFilePath.ToString());
        using (FileStream file = File.OpenRead(filePath + chunkcoord.ToString() + ".chunk"))
        {
            return Serializer.Deserialize<Chunk>(file);
        }
    }

    void Awake () {
        Debug.Log("SaveObj Awake");
        string filePath = PlayerPrefs.GetString(PlayerPrefEnum.MainFilePath.ToString());
        if (File.Exists(filePath + "main.bin"))
        {
            if (Map == null)
            {
                using (FileStream file = File.OpenRead(filePath + "main.bin"))
                    Map = Serializer.Deserialize<Map>(file);
                foreach(string path in Directory.GetFiles(filePath, "*.chunk"))
                {
                    string[] filename = Path.GetFileNameWithoutExtension(path).Split('_');
                    Vec2Ser coord = new Vec2Ser(int.Parse(filename[0]), int.Parse(filename[1]));
                    using(FileStream file = File.OpenRead(path))
                    {
                        Chunks.Add(coord, Serializer.Deserialize<Chunk>(file));
                    }
                }
            }
        }
        else
        {
            using (FileStream file = File.Create(filePath + "main.bin"))
            {
                Map = new Map(100);
                Serializer.Serialize(file, Map);
            }
        }
	}
}