using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
 public class BoardSetting
{
    public int xSize, ySize;
    public Tile tileGo;
    public List<Sprite> tileSprite;
}

public class GameManeger : MonoBehaviour

{
    [Header("Параметри игровой доски")]
    public BoardSetting BoardSetting;

    private void Start()
    {
        BoardController.instance.SetValue(Board.instance.SetValue(BoardSetting.xSize, BoardSetting.ySize, BoardSetting.tileGo, BoardSetting.tileSprite),
            BoardSetting.xSize, BoardSetting.ySize,
            BoardSetting.tileSprite);

    }

}
