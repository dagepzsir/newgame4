using UnityEngine;
using System.Collections;
using ProtoBuf;
using Assets.Scripts;

public delegate void MacroTileClickedHandle(MacroTile tileSender);
public delegate void MacroTileOverHandle(MacroTile tileSender);
public delegate void MacroTileLeaveHandle(MacroTile tileSender);

public enum MacroTileCategory
{
    Grass,
    Forest,
    Water,
    Mountain,
    VolcanoBase,
    VolcanoLava
}
public class MacroTile : MonoBehaviour {

    public event MacroTileClickedHandle MacroTileClicked;
    public event MacroTileLeaveHandle MacroTileMouseLeaved;
    public event MacroTileOverHandle MacroTileMouseOver;


    //Saved Variables
    public float Height;
    public Vec2Ser Coordinates { get; set; } //implicitly saved
    public MacroTileCategory TileCategory;

    //Calculated Variables
    public Color HighColor;
    public Sprite BaseSprite;
    public Vec2Ser ChunkCoordinates;
    public Vector2 ChunkVec;
    public BaseTileObject ObjectOnTile;
    public MacroTile(float height)
    {
        Height = height;
    }
    public void SetSpriteRaw(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
    public void SetTileBaseSprite(Sprite[] basesprites, GameObject[] enviromentalobjs)
    {
        switch (GetComponent<MacroTile>().TileCategory)
        {
            case MacroTileCategory.Water:
                BaseSprite = basesprites[(int)TileCategory];
                Color color = new Color(0, 165 / 255 * Height, 1 * Height, Color.cyan.a);
                HighColor = color;
                break;
            case MacroTileCategory.Grass:
                BaseSprite = basesprites[(int)TileCategory];
                Color color2 = new Color(Color.green.r, Color.green.g * ((Height) + 2 * (0.5f - Height)), Color.green.b, Color.green.a);
                if (Height >= 0.4f)
                    SetObject(enviromentalobjs[(int)EnviromentObjecType.Grass]);
                HighColor = color2;
                break;
            case MacroTileCategory.Forest:
                BaseSprite = basesprites[(int)TileCategory];
                Color color3 = new Color(Color.green.r, Color.green.g * (1 - Height), Color.green.b, Color.green.a);
                if (Height >= 0.6f)
                    SetObject(enviromentalobjs[(int)EnviromentObjecType.PineTree]);
                else
                    SetObject(enviromentalobjs[(int)EnviromentObjecType.NormalTree]);
                HighColor = color3;
                break;
            case MacroTileCategory.Mountain:
                BaseSprite = basesprites[(int)TileCategory];
                Color color4 = new Color(90 * (1 - Height), 46 * (1 - Height), 15 * (1 - Height), 1);
                HighColor = color4;
                break;
        }
        GetComponent<SpriteRenderer>().sprite = BaseSprite;
        GetComponent<SpriteRenderer>().color = HighColor;
    }

    public void SetObject(GameObject tileobject)
    {
        ObjectOnTile = tileobject.GetComponent<BaseTileObject>();
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = tileobject.GetComponent<BaseTileObject>().ObjectSprite;

    }

    void Start () {
        ChunkVec = new Vector2(ChunkCoordinates.X, ChunkCoordinates.Y);
	}

    void OnMouseExit()
    {
        if (MacroTileMouseLeaved != null)
            MacroTileMouseLeaved(this);
    }
    void OnMouseOver()
    {
        if (MacroTileMouseOver != null)
            MacroTileMouseOver(this);
    }
    void OnMouseUpAsButton()
    {
        if (MacroTileClicked != null)
            MacroTileClicked(this);
    }
}