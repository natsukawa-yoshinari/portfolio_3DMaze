using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FindGoal
{
    private const MazeCreator.Face FRONT = MazeCreator.Face.front;
    private const MazeCreator.Face BOTTOM = MazeCreator.Face.bottom;
    private const MazeCreator.Face BACK = MazeCreator.Face.back;
    private const MazeCreator.Face TOP = MazeCreator.Face.top;
    private const MazeCreator.Face RIGHT = MazeCreator.Face.right;
    private const MazeCreator.Face LEFT = MazeCreator.Face.left;
    private Dictionary<MazeCreator.Face, MazeCell[,]> _maps;
    private Dictionary<MazeCreator.Face, CellObject[,]> _mapCells;
    private int _mazeWidth;

    public FindGoal(Dictionary<MazeCreator.Face, MazeCell[,]> maps,
                    Dictionary<MazeCreator.Face, CellObject[,]> mapCells,
                    int mazeWidth)
    {
        _maps = maps;
        _mapCells = mapCells;
        _mazeWidth = mazeWidth;
    }

    public async UniTask GetGoal()
    {
        CellTransform startPos = new CellTransform
        {
            cellFace = MazeCreator.Face.top,
            cellPos = new Vector2(_mazeWidth / 2, _mazeWidth / 2),
        };

        // DFM探索
        Stack<CellTransform> stack = new Stack<CellTransform>();
        stack.Push(startPos);

        if (stack.Count > 0)
        {
            CellTransform start = stack.Peek();
            _maps[start.cellFace][(int)start.cellPos.x, (int)start.cellPos.y].visited = true;
        }

        while (stack.Count > 0)
        {
            CellTransform current = stack.Peek();

            // ゴール地点でBreak
            if (current.cellFace == MazeCreator.Face.bottom
                && current.cellPos.x == _mazeWidth / 2
                && current.cellPos.y == _mazeWidth / 2)
                break;

            List<CellTransform> moveList = new();
            SetCanMovePos(moveList, current);

            if (moveList.Count == 0)
            {
                _mapCells[current.cellFace][(int)current.cellPos.x, (int)current.cellPos.y]
                    .SetDefaultColor();
                stack.Pop();
                continue;
            }

            int randomId = Random.Range(0, moveList.Count);
            CellTransform next = new CellTransform()
            {
                cellFace = moveList[randomId].cellFace,
                cellPos = moveList[randomId].cellPos,
            };

            _maps[next.cellFace][(int)next.cellPos.x, (int)next.cellPos.y].visited = true;
            _mapCells[next.cellFace][(int)next.cellPos.x, (int)next.cellPos.y]
                    .SetCellColor(Color.softGreen);
            stack.Push(next);
            await UniTask.WaitForSeconds(0.01f);
        }

        int half = _mazeWidth / 2;
        Color goal = new Color(0x60 / 256f, 0x6e / 256f, 0xb2 / 256f);
        _mapCells[MazeCreator.Face.bottom][half, half].SetCellColor(goal);
    }

    // 探索時に動ける場所
    private void SetCanMovePos(List<CellTransform> moveList, CellTransform current)
    {
        MazeCreator.Face face = current.cellFace;
        int x = (int)current.cellPos.x;
        int y = (int)current.cellPos.y;

        if (x + 1 < _mazeWidth && _maps[face][x + 1, y].visited == false
            && _maps[face][x, y].right == false)
            AddCanMovePosList(moveList, face, new Vector2(x + 1, y));

        if (x - 1 >= 0 && _maps[face][x - 1, y].visited == false
            && _maps[face][x, y].left == false)
            AddCanMovePosList(moveList, face, new Vector2(x - 1, y));

        if (y + 1 < _mazeWidth && _maps[face][x, y + 1].visited == false
            && _maps[face][x, y].top == false)
            AddCanMovePosList(moveList, face, new Vector2(x, y + 1));

        if (y - 1 >= 0 && _maps[face][x, y - 1].visited == false
            && _maps[face][x, y].bottom == false)
            AddCanMovePosList(moveList, face, new Vector2(x, y - 1));

        AddDifferentFaceCase(moveList, current);
    }

    // 面から面に動くとき
    private void AddDifferentFaceCase(List<CellTransform> moveList, CellTransform current)
    {
        MazeCreator.Face face = current.cellFace;
        int x = (int)current.cellPos.x;
        int y = (int)current.cellPos.y;
        int max = _mazeWidth - 1;
        switch (face)
        {
            case MazeCreator.Face.front:
                if (x == 0 && _maps[BOTTOM][max, y].visited == false
                    && _maps[FRONT][x, y].left == false)
                    AddCanMovePosList(moveList, BOTTOM, new Vector2(max, y));

                if (x == max && _maps[TOP][0, y].visited == false
                    && _maps[FRONT][x, y].right == false)
                    AddCanMovePosList(moveList, TOP, new Vector2(0, y));

                if (y == max && _maps[RIGHT][x, 0].visited == false
                    && _maps[FRONT][x, y].top == false)
                    AddCanMovePosList(moveList, RIGHT, new Vector2(x, 0));

                if (y == 0 && _maps[LEFT][x, max].visited == false
                    && _maps[FRONT][x, y].bottom == false)
                    AddCanMovePosList(moveList, LEFT, new Vector2(x, max));

                break;

            case MazeCreator.Face.bottom:
                if (x == 0 && _maps[BACK][max, y].visited == false
                    && _maps[BOTTOM][x, y].left == false)
                    AddCanMovePosList(moveList, BACK, new Vector2(max, y));

                if (x == max && _maps[FRONT][0, y].visited == false
                    && _maps[BOTTOM][x, y].right == false)
                    AddCanMovePosList(moveList, FRONT, new Vector2(0, y));

                if (y == max && _maps[RIGHT][0, (max - x)].visited == false
                    && _maps[BOTTOM][x, y].top == false)
                    AddCanMovePosList(moveList, RIGHT, new Vector2(0, (max - x)));

                if (y == 0 && _maps[LEFT][0, x].visited == false
                    && _maps[BOTTOM][x, y].bottom == false)
                    AddCanMovePosList(moveList, LEFT, new Vector2(0, x));

                break;

            case MazeCreator.Face.back:
                if (x == max && _maps[BOTTOM][0, y].visited == false
                    && _maps[BACK][x, y].right == false)
                    AddCanMovePosList(moveList, BOTTOM, new Vector2(0, y));

                if (x == 0 && _maps[TOP][max, y].visited == false
                    && _maps[BACK][x, y].left == false)
                    AddCanMovePosList(moveList, TOP, new Vector2(max, y));

                if (y == max && _maps[RIGHT][(max - x), max].visited == false
                    && _maps[BACK][x, y].top == false)
                    AddCanMovePosList(moveList, RIGHT, new Vector2((max - x), max));

                if (y == 0 && _maps[LEFT][(max - x), 0].visited == false
                    && _maps[BACK][x, y].bottom == false)
                    AddCanMovePosList(moveList, LEFT, new Vector2((max - x), 0));

                break;

            case MazeCreator.Face.top:
                if (x == 0 && _maps[FRONT][max, y].visited == false
                    && _maps[TOP][x, y].left == false)
                    AddCanMovePosList(moveList, FRONT, new Vector2(max, y));

                if (x == max && _maps[BACK][0, y].visited == false
                    && _maps[TOP][x, y].right == false)
                    AddCanMovePosList(moveList, BACK, new Vector2(0, y));

                if (y == max && _maps[RIGHT][max, x].visited == false
                    && _maps[TOP][x, y].top == false)
                    AddCanMovePosList(moveList, RIGHT, new Vector2(max, x));

                if (y == 0 && _maps[LEFT][max, (max - x)].visited == false
                    && _maps[TOP][x, y].bottom == false)
                    AddCanMovePosList(moveList, LEFT, new Vector2(max, (max - x)));

                break;

            case MazeCreator.Face.right:
                if (y == 0 && _maps[FRONT][x, max].visited == false
                    && _maps[RIGHT][x, y].bottom == false)
                    AddCanMovePosList(moveList, FRONT, new Vector2(x, max));

                if (x == max && _maps[TOP][y, max].visited == false
                    && _maps[RIGHT][x, y].right == false)
                    AddCanMovePosList(moveList, TOP, new Vector2(y, max));

                if (x == 0 && _maps[BOTTOM][(max - y), max].visited == false
                    && _maps[RIGHT][x, y].left == false)
                    AddCanMovePosList(moveList, BOTTOM, new Vector2((max - y), max));

                if (y == max && _maps[BACK][(max - x), max].visited == false
                    && _maps[RIGHT][x, y].top == false)
                    AddCanMovePosList(moveList, BACK, new Vector2((max - x), max));

                break;

            case MazeCreator.Face.left:
                if (y == max && _maps[FRONT][x, 0].visited == false
                    && _maps[LEFT][x, y].top == false)
                    AddCanMovePosList(moveList, FRONT, new Vector2(x, 0));

                if (x == max && _maps[TOP][(max - y), 0].visited == false
                    && _maps[LEFT][x, y].right == false)
                    AddCanMovePosList(moveList, TOP, new Vector2((max - y), 0));

                if (x == 0 && _maps[BOTTOM][y, 0].visited == false
                    && _maps[LEFT][x, y].left == false)
                    AddCanMovePosList(moveList, BOTTOM, new Vector2(y, 0));

                if (y == 0 && _maps[BACK][(max - x), 0].visited == false
                    && _maps[LEFT][x, y].bottom == false)
                    AddCanMovePosList(moveList, BACK, new Vector2((max - x), 0));

                break;
        }
    }

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
}
