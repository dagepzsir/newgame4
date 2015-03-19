using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts;



public class MicroGameManager : MonoBehaviour {

    public GameObject ObjectToSpawn;
    public Camera Camera;

    public Sprite[] BaseTileSprites;
    public Sprite[] CategoryBaseTiles;
    public Sprite[] ColonialSprites;
    public Sprite[] IndustrialSprites;
    public Sprite[] MilitarySprites;

    private SaveObjectManager saveObject;
    private Dictionary<Vec2Ser, MiniTileData> tileDatainChunk = new Dictionary<Vec2Ser, MiniTileData>();
    private Dictionary<Vec2Ser, MiniTile> tileDict = new Dictionary<Vec2Ser, MiniTile>();
    private MiniTileCategory currentCategory;
    private Chunk currentChunk;
    private Sprite currentObject;
    private int MAX_LEVEL 
    { 
        get 
        { 
            return MilitarySprites.Length - 1; 
        } 
    }
    private float distance
    {
        get
        {
            Sprite tileSprite = ObjectToSpawn.GetComponent<SpriteRenderer>().sprite;
            return tileSprite.texture.width / tileSprite.pixelsPerUnit;
        }
    }
	void Start () {
        GameObject tempObject = GameObject.FindGameObjectWithTag("SaveObject");
        saveObject = tempObject.GetComponent<SaveObjectManager>();

        currentChunk = saveObject.Chunks[new Vec2Ser(PlayerPrefs.GetString(PlayerPrefEnum.ChunkCoords.ToString()))];
        tileDatainChunk = currentChunk.ChunkData[new Vec2Ser(PlayerPrefs.GetString(PlayerPrefEnum.ParentCoords.ToString()))];

        foreach (KeyValuePair<Vec2Ser, MiniTileData> values in tileDatainChunk)
        {
            float posX = values.Key.X * distance;
            float posY = values.Key.Y * distance;
            
            GameObject tempObj = Instantiate(ObjectToSpawn, new Vector3(posX, posY, 0), new Quaternion()) as GameObject;
            tempObj.GetComponent<MiniTile>().Coordinates = new Vec2Ser(values.Key.X, values.Key.Y);

            tempObj.GetComponent<MiniTile>().Height = values.Value.Height;
            tempObj.GetComponent<MiniTile>().TileCategory = values.Value.TileCategory;
            tempObj.GetComponent<MiniTile>().MiniCategory = values.Value.MiniCategory;
            tempObj.GetComponent<MiniTile>().Level = values.Value.Level;
            tempObj.GetComponent<MiniTile>().SetTileBaseSprite(BaseTileSprites);
            tempObj.GetComponent<MiniTile>().SetMiniTypeSprite(CategoryBaseTiles);
            SetMiniTileObject(tempObj.GetComponent<MiniTile>());
            tempObj.GetComponent<MiniTile>().MiniTileClicked += MicroGameManager_MiniTileClicked;
            tempObj.GetComponent<MiniTile>().MiniTileMouseOver += MicroGameManager_MiniTileMouseOver;
            tempObj.GetComponent<MiniTile>().MiniTileMouseLeave += MicroGameManager_MiniTileMouseLeave;
            tileDict.Add(values.Key, tempObj.GetComponent<MiniTile>());
        }

        //Center the Camera
        Vec2Ser midleCoords = new Vec2Ser((Mathf.Sqrt(tileDict.Count) / 2) - 1, (Mathf.Sqrt(tileDict.Count) / 2) - 1);
        Vector2 midlePosition = tileDict[midleCoords].transform.position;
        Camera.transform.position = new Vector3(midlePosition.x + distance / 2, midlePosition.y + distance / 2, -10);

        //Sets first MiniTileCategory and NextObject
        GetNextKind();
	}

    private void MicroGameManager_MiniTileMouseLeave(MiniTile tileSender)
    {
        if (tileSender.MiniCategory != MiniTileCategory.None)
        {
            tileSender.SetMiniTypeSprite(CategoryBaseTiles);
            tileSender.SetColorAlpha(1.0f);
        }
        else
        {
            tileSender.SetTileBaseSprite(BaseTileSprites);
        }
        tileSender.SetObjectTemporary(tileSender.ObjectSprite);
    }

    private void MicroGameManager_MiniTileMouseOver(MiniTile tileSender)
    {
        if (tileSender.MiniCategory == MiniTileCategory.None)
        {
            tileSender.SetSpriteRaw(CategoryBaseTiles[(int)currentCategory]);
            tileSender.SetColor(Color.white);
            tileSender.SetColorAlpha(0.5f);
            tileSender.SetObjectTemporary(currentObject);
        }
    }

    private void MicroGameManager_MiniTileClicked(MiniTile tileSender)
    {
        if (tileSender.MiniCategory == MiniTileCategory.None)
        {
            Dictionary<Vec2Ser, MiniTile> temp = new Dictionary<Vec2Ser, MiniTile>();

            tileSender.MiniCategory = currentCategory;
            tileSender.SetMiniTypeSprite(CategoryBaseTiles);

            tileSender.Level++;

            tileSender.GetConnectedTiles(tileDict, ref temp);

            while (temp.Count >= 3)
            {
                if (tileSender.Level + 1 <= MAX_LEVEL)
                {
                    foreach (KeyValuePair<Vec2Ser, MiniTile> pair in temp)
                    {
                        if (pair.Value != tileSender)
                            pair.Value.ResetTile();
                    }

                    tileSender.Level++;
                    temp.Clear();
                    tileSender.GetConnectedTiles(tileDict, ref temp);
                }
                else if (tileSender.Level == MAX_LEVEL)
                    break;

            }

            SetMiniTileObject(tileSender);
            GetNextKind();
        }
        currentChunk.UpdateChunk(new Vec2Ser(PlayerPrefs.GetString(PlayerPrefEnum.ParentCoords.ToString())), tileDict);
        saveObject.SaveChunk(new Vec2Ser(PlayerPrefs.GetString(PlayerPrefEnum.ChunkCoords.ToString())), currentChunk);
    }

    private void GetNextKind()
    {
        int rand = Random.Range(0, 3);

        currentCategory = (MiniTileCategory)rand;
        switch(currentCategory)
        {
            case MiniTileCategory.Colonial:
                currentObject = ColonialSprites[0];
                break;
            case MiniTileCategory.Industrial:
                currentObject = IndustrialSprites[0];
                break;
            case MiniTileCategory.Military:
                currentObject = MilitarySprites[0];
                break;
        }
    }
    private void SetMiniTileObject(MiniTile tile)
    {
        switch(tile.MiniCategory)
        {
            case MiniTileCategory.Colonial:
                tile.SetObjectFinally(ColonialSprites[tile.Level]);
                break;
            case MiniTileCategory.Industrial:
                tile.SetObjectFinally(IndustrialSprites[tile.Level]);
                break;
            case MiniTileCategory.Military:
                tile.SetObjectFinally(MilitarySprites[tile.Level]);
                break;
        }
    }
}