using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;
using System.IO;

public class MainGameManager : MonoBehaviour {

    public SaveObjectManager SaveObject;
    public GameObject ObjectToSpawn;
    public Sprite MouseOverTexture;
    public MacroTileCategory TileCategory;
    public Sprite[] BaseTileSprites;
    private Dictionary<Vec2Ser, MacroTile> tileDict = new Dictionary<Vec2Ser, MacroTile>();
    private float distance
    {
        get
        {
            Sprite tileSprite = ObjectToSpawn.GetComponent<SpriteRenderer>().sprite;
            return tileSprite.texture.width / tileSprite.pixelsPerUnit;
        }
    }
    void Start () {
        DontDestroyOnLoad(SaveObject);
        
        LoadGame(SaveObject.Map);
	}
    
    void LoadGame(Map map)
    {
        foreach (KeyValuePair<Vec2Ser, MacroTileData> values in map.MapData)
        {
            float posX = values.Key.X * distance;
            float posY = values.Key.Y * distance;
            int chunkX = (int)(values.Key.X / 10);
            int chunkY = (int)(values.Key.Y / 10);
            ObjectToSpawn.GetComponent<MacroTile>().Height = values.Value.Height;
            GameObject tempObj = Instantiate(ObjectToSpawn, new Vector3(posX, posY, 0), new Quaternion()) as GameObject;

            tempObj.GetComponent<MacroTile>().MacroTileClicked += MainGameManager_MacroTileClicked;
            tempObj.GetComponent<MacroTile>().MacroTileMouseOver += MainGameManager_MacroTileMouseOver;
            tempObj.GetComponent<MacroTile>().MacroTileMouseLeaved += MainGameManager_MacroTileMouseLeaved;

            tempObj.GetComponent<MacroTile>().Coordinates = new Vec2Ser(values.Key.X, values.Key.Y);
            tempObj.GetComponent<MacroTile>().ChunkCoordinates = new Vec2Ser(chunkX, chunkY);

            tempObj.GetComponent<MacroTile>().TileCategory = values.Value.TileCategory;
            tempObj.GetComponent<MacroTile>().SetTileBaseSprite(BaseTileSprites);

            tileDict.Add(values.Key, tempObj.GetComponent<MacroTile>());
        }
    }

    void MainGameManager_MacroTileMouseLeaved(MacroTile tileSender)
    {
        tileSender.SetColor(tileSender.HighColor);
        tileSender.SetSpriteRaw(tileSender.BaseSprite);
    }
    void MainGameManager_MacroTileMouseOver(MacroTile tileSender)
    {
        tileSender.SetColor(Color.white);
        tileSender.SetSpriteRaw(MouseOverTexture);
    }

    void MainGameManager_MacroTileClicked(MacroTile tileSender)
    {
        if (SaveObject.Chunks.ContainsKey(tileSender.ChunkCoordinates) == false)
        {
            if (File.Exists(PlayerPrefs.GetString(PlayerPrefEnum.MainFilePath.ToString()) + tileSender.ChunkCoordinates.ToString() + ".chunk") == false)
            {
                Chunk newChunk = new Chunk(6, tileSender, tileDict);
                SaveObject.SaveChunk(tileSender.ChunkCoordinates, newChunk);
                SaveObject.Chunks.Add(tileSender.ChunkCoordinates, newChunk);
            }
            else
            {
                SaveObject.Chunks.Add(tileSender.ChunkCoordinates, SaveObject.LoadChunk(tileSender.ChunkCoordinates));
            }
        }
        PlayerPrefs.SetString(PlayerPrefEnum.ChunkCoords.ToString(), tileSender.ChunkCoordinates.ToString());
        PlayerPrefs.SetString(PlayerPrefEnum.ParentCoords.ToString(), tileSender.Coordinates.ToString());
        Application.LoadLevel("MicroScene");
    }
}