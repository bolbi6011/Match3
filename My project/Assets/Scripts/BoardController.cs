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
    private Vector2[] dirRay=new Vector2[] {Vector2.up, Vector2.down,Vector2.left,Vector2.right};

    private bool isFindMatch = false;

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
    private void SelecTile(Tile tile)
    {
        tile.isSelected= true;
        tile.spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);
        oldSelectile= tile;
    }
    private void DeselecTile(Tile tile)
    {
        tile.isSelected= false;
        tile.spriteRenderer.color = new Color(1, 1, 1);
        oldSelectile= null;
    }
    private void ChecSelecTile(Tile tile)
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
            // Первое віделение тайла
            if (!tile.isSelected&& oldSelectile == null)
            {
                SelecTile(tile);
            }
            //Попытка выбрать другой тайл
            else
            {
                // если 2й выбраний тайл сосед предидущего
                if (AdjacentTiles().Contains(tile)) 
                {
                    SwapTwoTiles(tile);
                    FindAllMatch(tile);
                    DeselecTile(oldSelectile);
                }
                //новое выделение, забываем старый тайл
                else
                {
                    DeselecTile(oldSelectile);
                    SelecTile(tile);
                }
              
            }

        }
    }
    #region(Поиск совпадения, удаление спрайтов, движение тайлов, смена спрайтов у тайлов)
    private  List<Tile> FindMatch(Tile tile, Vector2 dir)
    {
        List<Tile> cashFindTiles = new List<Tile>();
        RaycastHit2D hit=Physics2D.Raycast(tile.transform.position, dir);
        while(hit.collider!=null 
            && hit.collider.gameObject.GetComponent<Tile>().spriteRenderer.sprite == tile.spriteRenderer.sprite)
        {
            cashFindTiles.Add(hit.collider.gameObject.GetComponent<Tile>());
            hit = Physics2D.Raycast(hit.collider.gameObject.transform.position, dir);
        }
        return cashFindTiles;
    }
    private void DeleteSprite(Tile tile, Vector2[] dirArray)
    { List<Tile> cashFindSprite = new List<Tile>();
      for (int i = 0; i <dirArray.Length; i++)
        {
            cashFindSprite.AddRange(FindMatch(tile, dirArray[i]));
        }
        if (cashFindSprite.Count>1)
        {
            for (int i=0; i < cashFindSprite.Count; i++)
            {
                cashFindSprite[i].spriteRenderer.sprite = null;
            }
            isFindMatch = true;
        }
    }
    private void FindAllMatch(Tile tile)
    {
        if (tile.isEmpty)
        {
            return;
        }
        DeleteSprite(tile, new Vector2[2] {Vector2.up,Vector2.down});
        DeleteSprite(tile, new Vector2[2] {Vector2.left, Vector2.right });
        if (isFindMatch)
        {
            isFindMatch= false;
            tile.spriteRenderer.sprite = null;
        }
    }
    #endregion
    private void SwapTwoTiles(Tile tile)
    {
        if (oldSelectile.spriteRenderer.sprite == tile.spriteRenderer.sprite)
        {
            return;
        }
        Sprite cashSprite = oldSelectile.spriteRenderer.sprite;
        oldSelectile.spriteRenderer.sprite = tile.spriteRenderer.sprite;
        tile.spriteRenderer.sprite = cashSprite;
    }
    private List<Tile> AdjacentTiles()
    {
        List<Tile> cashTiles= new List<Tile>();
        for (int i=0; i < dirRay.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(oldSelectile.transform.position, dirRay[i]);
            if (hit.collider != null)
            {
                
                cashTiles.Add(hit.collider.gameObject.GetComponent<Tile>());
            }
        }
        return cashTiles;
    }
}
