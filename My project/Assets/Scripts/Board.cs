using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;

    private int xSize, ySize;
    private Tile tileGO;
    private List<Sprite> tileSprite = new List<Sprite>();

    private void Awake()
    {
        instance= this;
    }
    public void SetValue(int xSize, int ySize, Tile tileGO, List<Sprite> tileSprite)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.tileGO = tileGO;
        this.tileSprite = tileSprite;
    }
    public void CreateBoard()
    {

    }
}
