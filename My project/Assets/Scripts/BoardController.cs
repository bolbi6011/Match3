using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController instance;

    private int xSize, ySize;
    private List<Sprite> tileSprite = new List<Sprite>();
    private Tile[,] tileArray;

    private Tile oldSelectile;
    private Vector2[] dirRay = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private bool isFindMatch = false;
    private bool isShift = false;
    private bool isSearchEmptyTile = false;

    public void SetValue(Tile[,] tileArray, int xSize, int ySize, List<Sprite> tileSprite)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.tileArray = tileArray;
        this.tileSprite = tileSprite;
    }
    private void Awake()
    {
        instance = this;
    }
   

    // Update is called once per frame
    void Update()
    {
        if (isSearchEmptyTile)
        {
            SearchEmptyTile();

        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D ray = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (ray != false)
            {
                ChecSelecTile(ray.collider.gameObject.GetComponent<Tile>());
            }
        }
    }
    #region(Выделить тайлб Снять выделение с тайла, управление выделением)
    private void SelecTile(Tile tile)
    {
        tile.isSelected = true;
        tile.spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);
        oldSelectile = tile;
    }
    private void DeselecTile(Tile tile)
    {
        tile.isSelected = false;
        tile.spriteRenderer.color = new Color(1, 1, 1);
        oldSelectile = null;
    }
    private void ChecSelecTile(Tile tile)
    {
        if (tile.isEmpty || isShift)
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
            if (!tile.isSelected && oldSelectile == null)
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
    #endregion
    #region(Поиск совпадения, удаление спрайта, Поиск всех совпадений)
    private List<Tile> FindMatch(Tile tile, Vector2 dir)
    {
        List<Tile> cashFindTiles = new List<Tile>();
        RaycastHit2D hit = Physics2D.Raycast(tile.transform.position, dir);
        while (hit.collider != null
            && hit.collider.gameObject.GetComponent<Tile>().spriteRenderer.sprite == tile.spriteRenderer.sprite)
        {
            cashFindTiles.Add(hit.collider.gameObject.GetComponent<Tile>());
            hit = Physics2D.Raycast(hit.collider.gameObject.transform.position, dir);
        }
        return cashFindTiles;
    }
    private void DeleteSprite(Tile tile, Vector2[] dirArray)
    {
        List<Tile> cashFindSprite = new List<Tile>();
        for (int i = 0; i < dirArray.Length; i++)
        {
            cashFindSprite.AddRange(FindMatch(tile, dirArray[i]));
        }
        if (cashFindSprite.Count > 1)
        {
            for (int i = 0; i < cashFindSprite.Count; i++)
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
        DeleteSprite(tile, new Vector2[2] { Vector2.up, Vector2.down });
        DeleteSprite(tile, new Vector2[2] { Vector2.left, Vector2.right });
        if (isFindMatch)
        {
            isFindMatch = false;
            tile.spriteRenderer.sprite = null;
            isSearchEmptyTile= true;
        }
    }
    #endregion
    #region(Cмена 2х тайлов, Соседние тайли)
    private void SwapTwoTiles(Tile tile)
    {
        if (oldSelectile.spriteRenderer.sprite == tile.spriteRenderer.sprite)
        {
            return;
        }
        Sprite cashSprite = oldSelectile.spriteRenderer.sprite;
        oldSelectile.spriteRenderer.sprite = tile.spriteRenderer.sprite;
        tile.spriteRenderer.sprite = cashSprite;

        UI.instance.Moves(1);
    }
    private List<Tile> AdjacentTiles()
    {
        List<Tile> cashTiles = new List<Tile>();
        for (int i = 0; i < dirRay.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(oldSelectile.transform.position, dirRay[i]);
            if (hit.collider != null)
            {

                cashTiles.Add(hit.collider.gameObject.GetComponent<Tile>());
            }
        }
        return cashTiles;
    }
    #endregion
    #region(Поиск пустого тайла, Сдвиг тайла вниз, Установить новое изображение, Выибрать новое изображение)
    private void SearchEmptyTile()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tileArray[x, y].isEmpty)
                {
                    ShiftTileDown(x, y);
                    break;
                }
                if (x == xSize && y == ySize-1)
                {
                    isSearchEmptyTile = false;
                }
            }
        }
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                FindAllMatch(tileArray[x, y]);
            }
        }



    }
    private void ShiftTileDown(int xPos, int yPos)
    {
        isShift = true;
        List<SpriteRenderer> cashRenderer= new List<SpriteRenderer>();
        int count = 0;
        for (int y = yPos; y < ySize; y++)
        {
            Tile tile = tileArray[xPos,y];
            if (tile.isEmpty)
            {
                count++;
            }
            
                cashRenderer.Add(tile.spriteRenderer);
            
        }
        for (int i=0; i < count; i++)
        {
            UI.instance.Score(10);
            SetNewSprite(xPos, cashRenderer);
        }
         isShift = false;
    }
    private void SetNewSprite(int xPos, List<SpriteRenderer> renderer)
    {
        for (int y =0; y< renderer.Count -1; y++)
        {
            renderer[y].sprite = renderer[y+1].sprite;
            renderer[y + 1].sprite = GetNewSprite(xPos, ySize-1);
        }
    }
    private Sprite GetNewSprite(int xPos, int yPos)
    {
        List<Sprite> cashSprite = new List<Sprite>();
        cashSprite.AddRange(tileSprite);

        if (xPos > 0)
        {
            cashSprite.Remove(tileArray[xPos - 1, yPos].spriteRenderer.sprite);
        }
        if (yPos < xSize - 1)
        {
            cashSprite.Remove(tileArray[yPos -1, yPos].spriteRenderer.sprite);
        }
        return cashSprite[Random.Range(0, cashSprite.Count)];
    }
    #endregion
}
