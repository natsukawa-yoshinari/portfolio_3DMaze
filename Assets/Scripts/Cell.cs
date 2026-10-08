using System.Collections.Generic;
using UnityEngine;

public class CellObject : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _wallList = new List<GameObject>();
    [SerializeField]
    private MeshRenderer mesh;
    private Color _myColor;

    private void Start()
    {
        _myColor = mesh.material.color;
    }

    public void BreakCellWall(MazeCell myCell)
    {
        if (myCell.top == false) _wallList[0].SetActive(false);
        if (myCell.bottom == false) _wallList[1].SetActive(false);
        if (myCell.left == false) _wallList[2].SetActive(false);
        if (myCell.right == false) _wallList[3].SetActive(false);
    }

    public void DeleteWall(MazeCell myCell)
    {
        if (myCell.top == false) Destroy(_wallList[0]);
        if (myCell.bottom == false) Destroy(_wallList[1]);
        if (myCell.left == false) Destroy(_wallList[2]);
        if (myCell.right == false) Destroy(_wallList[3]);
    }

    public void SetCellColor(Color color)
    {
        mesh.material.color = color;
    }
    public void SetDefaultColor()
    {
        mesh.material.color = _myColor;
    }
}

// Cellの位置情報などを保存するクラス
[System.Serializable]
public class MazeCell
{
    public CellTransform info;
    // コンストラクタ
    public MazeCell(MazeCreator.Face face, int x, int y)
    {
        info = new CellTransform();
        info.cellFace = face;
        info.cellPos.x = x;
        info.cellPos.y = y;
    }

    public bool visited;

    // 壁があるか情報
    public bool top = true;
    public bool right = true;
    public bool bottom = true;
    public bool left = true;
}

[System.Serializable]
public class CellTransform
{
    public MazeCreator.Face cellFace;
    public Vector2 cellPos;
}
