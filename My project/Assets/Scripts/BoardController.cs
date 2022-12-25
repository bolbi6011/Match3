using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController instance;

    private int xSize, ySize;
    private List<Sprite> tileSprite = new List<Sprite>();
    private Tile[,] tileArray;
    private Tile oldSelectile;

    public void SetValue(Tile[,] tileArray, int xSize, int ySize, List<Sprite> tileSprite)
    {
      this.xSize = xSize;
      this.ySize = ySize;
      this.tileArray = tileArray;
      this.tileSprite = tileSprite;
    }
    private void Awake()
    {
        instance=this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D ray = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (ray!=false)
            {
                ChecSelecTile(ray.collider.gameObject.GetComponent<Tile>());
            }
        }
    }
    public void SelecTile(Tile tile)
    {
        tile.isSelected= true;
        tile.spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);
        oldSelectile= tile;
    }
    public void DeselecTile(Tile tile)
    {
        tile.isSelected= false;
        tile.spriteRenderer.color = new Color(1, 1, 1);
        oldSelectile= null;
    }
    public void ChecSelecTile(Tile tile)
    {
        if (tile.isEmpty)
        {
            return;
        }
        if (tile.isSelected)
        {
            DeselecTile(tile);
        }
        else
        {
            if (!tile.isSelected&& oldSelectile == null)
            {
                SelecTile(tile);
            }

        }
    }
    
}
