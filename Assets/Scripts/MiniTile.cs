using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System.Collections.Generic;

public delegate void MiniTileClickedHandle(MiniTile tileSender);
public delegate void MiniTileOverHandle(MiniTile tileSender);
public delegate void MiniTileLeaveHandle(MiniTile tileSender);
public enum MiniTileCategory
{
    None = -1,
    Colonial = 0,
    Industrial = 1,
    Military = 2
}

public class MiniTile : MonoBehaviour {

    public event MiniTileClickedHandle MiniTileClicked;
    public event MiniTileOverHandle MiniTileMouseOver;
    public event MiniTileLeaveHandle MiniTileMouseLeave;

    //Saved Variables
    public float Height;
    public MacroTileCategory TileCategory;
    public MiniTileCategory MiniCategory;
    public Vec2Ser Coordinates; //Saved as Directory key
    public int Level;

    //Calculated Variables
    public Color HighColor;
    public Sprite BaseSprite;
    public Sprite ObjectSprite;
    void Start () {
	
	}
    public void SetSpriteRaw(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
    public void SetObjectFinally(Sprite objectsprite)
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = objectsprite;
        ObjectSprite = objectsprite;
    }
    public void SetObjectTemporary(Sprite objectsprite)
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = objectsprite;
    }
    public void SetColorAlpha(float alpha)
    {
        Color prevColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(prevColor.r, prevColor.g, prevColor.b, alpha);
    }

    public void SetTileBaseSprite(Sprite[] basesprites)
    {
        switch (TileCategory)
        {
            case MacroTileCategory.Water:
                BaseSprite = basesprites[(int)TileCategory];
                Color color = new Color(0, 165 / 255 * Height, 1 * Height, Color.cyan.a);
                HighColor = color;
                break;
            case MacroTileCategory.Grass:
                BaseSprite = basesprites[(int)TileCategory];
                Color color2 = new Color(Color.green.r, Color.green.g * ((Height) + 2 * (0.5f - Height)), Color.green.b, Color.green.a);
                HighColor = color2;
                break;
            case MacroTileCategory.Forest:
                BaseSprite = basesprites[(int)TileCategory];
                Color color3 = new Color(Color.green.r, Color.green.g * (1 - Height), Color.green.b, Color.green.a);
                HighColor = color3;
                break;
            case MacroTileCategory.Mountain:
                BaseSprite = basesprites[(int)TileCategory];
                Color color4 = new Color(90 * (1 - Height), 46 * (1 - Height), 15 * (1 - Height), 1);
                HighColor = color4;
                break;
        }
        SetSpriteRaw(BaseSprite);
        SetColor(HighColor);
    }
    public void ResetTile()
    {
        SetColor(HighColor);
        SetSpriteRaw(BaseSprite);
        Level = -1;
        MiniCategory = MiniTileCategory.None;
        SetObjectFinally(null);
    }
    public void SetMiniTypeSprite(Sprite[] categorybasesprites)
    {
        if (MiniCategory != MiniTileCategory.None)
        {
            SetColor(Color.white);
            SetSpriteRaw(categorybasesprites[(int)MiniCategory]);
        }
    }

    public Dictionary<Vec2Ser, MiniTile> CheckFourSides(Dictionary<Vec2Ser, MiniTile> tiledict)
    {
        Dictionary<Vec2Ser, MiniTile> output = new Dictionary<Vec2Ser, MiniTile>();
        Vec2Ser top = new Vec2Ser(Coordinates.X, Coordinates.Y + 1);
        Vec2Ser bottom = new Vec2Ser(Coordinates.X, Coordinates.Y - 1);
        Vec2Ser left = new Vec2Ser(Coordinates.X - 1, Coordinates.Y);
        Vec2Ser right = new Vec2Ser(Coordinates.X + 1, Coordinates.Y);
        if(tiledict.ContainsKey(top))
        {
            if (tiledict[top].Level == Level && tiledict[top].MiniCategory == MiniCategory)
            {
                //Debug.Log("top");
                output.Add(top, tiledict[top]);
            }
        }
        if (tiledict.ContainsKey(bottom))
        {
            if (tiledict[bottom].Level == Level && tiledict[bottom].MiniCategory == MiniCategory)
            {
                //Debug.Log("bottom");
                output.Add(bottom, tiledict[bottom]);
            }
        }
        if (tiledict.ContainsKey(left))
        {
            if (tiledict[left].Level == Level && tiledict[left].MiniCategory == MiniCategory)
            {
               // Debug.Log("left");
                output.Add(left, tiledict[left]);
            }
        }
        if (tiledict.ContainsKey(right))
        {
            if (tiledict[right].Level == Level && tiledict[right].MiniCategory == MiniCategory)
            {
               // Debug.Log("right");
                output.Add(right, tiledict[right]);
            }
        }
        return output;
    }

    public void GetConnectedTiles(Dictionary<Vec2Ser, MiniTile> tiledict, ref Dictionary<Vec2Ser, MiniTile> connectedtiles)
    {
        Dictionary<Vec2Ser, MiniTile> connected = CheckFourSides(tiledict);
        foreach(KeyValuePair<Vec2Ser, MiniTile> pair in connected)
        {
            if(connectedtiles.ContainsKey(pair.Key) == false)
            {
                connectedtiles.Add(pair.Key, pair.Value);
                pair.Value.GetConnectedTiles(tiledict, ref connectedtiles);
            }
        }
    }
    void OnMouseExit()
    {
        if (MiniTileMouseLeave != null)
            MiniTileMouseLeave(this);
    }
    void OnMouseOver()
    {
        if (MiniTileMouseOver != null)
            MiniTileMouseOver(this);
    }
    void OnMouseUpAsButton()
    {
        if (MiniTileClicked != null)
            MiniTileClicked(this);
    }
}