using System.Collections.Generic;
using UnityEngine;

public class CalcCanMovePos
{
    private const MazeCreator.Face FRONT = MazeCreator.Face.front;
    private const MazeCreator.Face BOTTOM = MazeCreator.Face.bottom;
    private const MazeCreator.Face BACK = MazeCreator.Face.back;
    private const MazeCreator.Face TOP = MazeCreator.Face.top;
    private const MazeCreator.Face RIGHT = MazeCreator.Face.right;
    private const MazeCreator.Face LEFT = MazeCreator.Face.left;
    private int _mazeWidth;
    public CalcCanMovePos(int mazeWidth)
    {
        _mazeWidth = mazeWidth;
    }

    // 冗長に段落を消費するのを防ぐ
    private void AddCanMovePosList(List<CellTransform> moveList,
                                    MazeCreator.Face face, Vector2 pos)
    {
        CellTransform c = new CellTransform
        {
            cellFace = face,
            cellPos = pos
        };
        moveList.Add(c);
    }

    /// <summary>
    /// 現在地からまだ行っていない周囲のCellをリストに追加する
    /// </summary>
    public void SetCanMovePos(Dictionary<MazeCreator.Face, MazeCell[,]> maps,
                                List<CellTransform> moveList,
                                CellTransform current)
    {
        MazeCreator.Face face = current.cellFace;
        int x = (int)current.cellPos.x;
        int y = (int)current.cellPos.y;

        // 隣が同じFaceのケース
        // 右
        if (x + 1 < _mazeWidth && maps[face][x + 1, y].visited == false)
            AddCanMovePosList(moveList, face, new Vector2(x + 1, y));
        // 左
        if (x - 1 >= 0 && maps[face][x - 1, y].visited == false)
            AddCanMovePosList(moveList, face, new Vector2(x - 1, y));
        // 上
        if (y + 1 < _mazeWidth && maps[face][x, y + 1].visited == false)
            AddCanMovePosList(moveList, face, new Vector2(x, y + 1));
        // 下
        if (y - 1 >= 0 && maps[face][x, y - 1].visited == false)
            AddCanMovePosList(moveList, face, new Vector2(x, y - 1));

        // 隣が違うFaceのケース
        AddDifferentFaceCase(maps, moveList, current);
    }

    // 隣のPositionのFaceが異なる場合のCanMovePosを追加する
    private void AddDifferentFaceCase(Dictionary<MazeCreator.Face, MazeCell[,]> maps,
                                    List<CellTransform> moveList,
                                    CellTransform current)
    {
        MazeCreator.Face face = current.cellFace;
        int x = (int)current.cellPos.x;
        int y = (int)current.cellPos.y;
        int max = _mazeWidth - 1;
        switch (face)
        {
            case MazeCreator.Face.front:
                // front -> bottom
                if (x == 0 && maps[BOTTOM][max, y].visited == false)
                    AddCanMovePosList(moveList, BOTTOM, new Vector2(max, y));

                // front -> top
                if (x == max && maps[TOP][0, y].visited == false)
                    AddCanMovePosList(moveList, TOP, new Vector2(0, y));

                // front -> right
                if (y == max && maps[RIGHT][x, 0].visited == false)
                    AddCanMovePosList(moveList, RIGHT, new Vector2(x, 0));

                // front -> left
                if (y == 0 && maps[LEFT][x, max].visited == false)
                    AddCanMovePosList(moveList, LEFT, new Vector2(x, max));

                break;

            case MazeCreator.Face.bottom:
                // bottom -> back
                if (x == 0 && maps[BACK][max, y].visited == false)
                    AddCanMovePosList(moveList, BACK, new Vector2(max, y));

                // bottom -> front
                if (x == max && maps[FRONT][0, y].visited == false)
                    AddCanMovePosList(moveList, FRONT, new Vector2(0, y));

                // bottom -> right
                if (y == max && maps[RIGHT][0, (max - x)].visited == false)
                    AddCanMovePosList(moveList, RIGHT, new Vector2(0, (max - x)));

                // bottom -> left
                if (y == 0 && maps[LEFT][0, x].visited == false)
                    AddCanMovePosList(moveList, LEFT, new Vector2(0, x));

                break;

            case MazeCreator.Face.back:
                // back -> bottom
                if (x == max && maps[BOTTOM][0, y].visited == false)
                    AddCanMovePosList(moveList, BOTTOM, new Vector2(0, y));

                // back -> top
                if (x == 0 && maps[TOP][max, y].visited == false)
                    AddCanMovePosList(moveList, TOP, new Vector2(max, y));

                // back -> right
                if (y == max && maps[RIGHT][(max - x), max].visited == false)
                    AddCanMovePosList(moveList, RIGHT, new Vector2((max - x), max));

                // back -> left
                if (y == 0 && maps[LEFT][(max - x), 0].visited == false)
                    AddCanMovePosList(moveList, LEFT, new Vector2((max - x), 0));

                break;

            case MazeCreator.Face.top:
                // top -> front
                if (x == 0 && maps[FRONT][max, y].visited == false)
                    AddCanMovePosList(moveList, FRONT, new Vector2(max, y));

                // top -> back
                if (x == max && maps[BACK][0, y].visited == false)
                    AddCanMovePosList(moveList, BACK, new Vector2(0, y));

                // top -> right
                if (y == max && maps[RIGHT][max, x].visited == false)
                    AddCanMovePosList(moveList, RIGHT, new Vector2(max, x));

                // top -> left
                if (y == 0 && maps[LEFT][max, (max - x)].visited == false)
                    AddCanMovePosList(moveList, LEFT, new Vector2(max, (max - x)));

                break;

            case MazeCreator.Face.right:
                // right -> front
                if (y == 0 && maps[FRONT][x, max].visited == false)
                    AddCanMovePosList(moveList, FRONT, new Vector2(x, max));

                // right -> top
                if (x == max && maps[TOP][y, max].visited == false)
                    AddCanMovePosList(moveList, TOP, new Vector2(y, max));

                // right -> bottom
                if (x == 0 && maps[BOTTOM][(max - y), max].visited == false)
                    AddCanMovePosList(moveList, BOTTOM, new Vector2((max - y), max));

                // right -> back
                if (y == max && maps[BACK][(max - x), max].visited == false)
                    AddCanMovePosList(moveList, BACK, new Vector2((max - x), max));

                break;

            case MazeCreator.Face.left:
                // left -> front
                if (y == max && maps[FRONT][x, 0].visited == false)
                    AddCanMovePosList(moveList, FRONT, new Vector2(x, 0));

                // left -> top
                if (x == max && maps[TOP][(max - y), 0].visited == false)
                    AddCanMovePosList(moveList, TOP, new Vector2((max - y), 0));

                // left -> bottom
                if (x == 0 && maps[BOTTOM][y, 0].visited == false)
                    AddCanMovePosList(moveList, BOTTOM, new Vector2(y, 0));

                // left -> back
                if (y == 0 && maps[BACK][(max - x), 0].visited == false)
                    AddCanMovePosList(moveList, BACK, new Vector2((max - x), 0));

                break;
        }
    }

}
