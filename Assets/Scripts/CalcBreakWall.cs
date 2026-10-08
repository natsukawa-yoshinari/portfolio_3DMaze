using System.Collections.Generic;
using UnityEngine;

public class CalcBreakWall
{
    private const MazeCreator.Face FRONT = MazeCreator.Face.front;
    private const MazeCreator.Face BOTTOM = MazeCreator.Face.bottom;
    private const MazeCreator.Face BACK = MazeCreator.Face.back;
    private const MazeCreator.Face TOP = MazeCreator.Face.top;
    private const MazeCreator.Face RIGHT = MazeCreator.Face.right;
    private const MazeCreator.Face LEFT = MazeCreator.Face.left;

    private Dictionary<MazeCreator.Face, MazeCell[,]> _maps;
    private Dictionary<MazeCreator.Face, CellObject[,]> _mapCells;

    public CalcBreakWall
    (
        Dictionary<MazeCreator.Face, MazeCell[,]> maps,
        Dictionary<MazeCreator.Face, CellObject[,]> mapCells
    )
    {
        _maps = maps;
        _mapCells = mapCells;
    }

    // 同じFaceで現在地と次の地点の間の壁を壊す
    public void BreakWall(CellTransform current, CellTransform next)
    {
        MazeCreator.Face currentFace = current.cellFace;
        int currentX = (int)current.cellPos.x;
        int currentY = (int)current.cellPos.y;

        MazeCreator.Face nextFace = next.cellFace;
        int nextX = (int)next.cellPos.x;
        int nextY = (int)next.cellPos.y;

        if (currentFace == nextFace)
        {
            // 同じFaceの壁破壊
            // 右
            if (nextX == currentX + 1)
            {
                _maps[currentFace][currentX, currentY].right = false;
                _maps[nextFace][nextX, nextY].left = false;
            }
            // 左
            else if (nextX == currentX - 1)
            {
                _maps[currentFace][currentX, currentY].left = false;
                _maps[nextFace][nextX, nextY].right = false;
            }
            // 上
            else if (nextY == currentY + 1)
            {
                _maps[currentFace][currentX, currentY].top = false;
                _maps[nextFace][nextX, nextY].bottom = false;
            }
            // 下
            else if (nextY == currentY - 1)
            {
                _maps[currentFace][currentX, currentY].bottom = false;
                _maps[nextFace][nextX, nextY].top = false;
            }
        }
        else
        {
            // 異なるFaceの壁破壊
            BreakDifferentFaceWall(current, next);
        }

        // WallをDisableする
        _mapCells[currentFace][currentX, currentY].BreakCellWall(_maps[currentFace][currentX, currentY]);
        _mapCells[nextFace][nextX, nextY].BreakCellWall(_maps[nextFace][nextX, nextY]);
    }

    // 異なるFaceで現在地と次の地点の間の壁を壊す
    private void BreakDifferentFaceWall(CellTransform current, CellTransform next)
    {
        MazeCreator.Face currentFace = current.cellFace;
        int currentX = (int)current.cellPos.x;
        int currentY = (int)current.cellPos.y;

        MazeCreator.Face nextFace = next.cellFace;
        int nextX = (int)next.cellPos.x;
        int nextY = (int)next.cellPos.y;

        // front, bottom間
        if ((currentFace == FRONT && nextFace == BOTTOM)
            || (currentFace == BOTTOM && nextFace == FRONT))
        {
            if (currentFace == FRONT)
            {
                _maps[currentFace][currentX, currentY].left = false;
                _maps[nextFace][nextX, nextY].right = false;
            }
            else
            {
                _maps[currentFace][currentX, currentY].right = false;
                _maps[nextFace][nextX, nextY].left = false;
            }
        }

        // front, top間
        else if ((currentFace == FRONT && nextFace == TOP)
                || (currentFace == TOP && nextFace == FRONT))
        {
            if (currentFace == FRONT)
            {
                _maps[currentFace][currentX, currentY].right = false;
                _maps[nextFace][nextX, nextY].left = false;
            }
            else
            {
                _maps[currentFace][currentX, currentY].left = false;
                _maps[nextFace][nextX, nextY].right = false;
            }
        }

        // front, right間
        else if ((currentFace == FRONT && nextFace == RIGHT)
                || (currentFace == RIGHT && nextFace == FRONT))
        {
            if (currentFace == FRONT)
            {
                _maps[currentFace][currentX, currentY].top = false;
                _maps[nextFace][nextX, nextY].bottom = false;
            }
            else
            {
                _maps[currentFace][currentX, currentY].bottom = false;
                _maps[nextFace][nextX, nextY].top = false;
            }
        }

        // front, left間
        else if ((currentFace == FRONT && nextFace == LEFT)
                || (currentFace == LEFT && nextFace == FRONT))
        {
            if (currentFace == FRONT)
            {
                _maps[currentFace][currentX, currentY].bottom = false;
                _maps[nextFace][nextX, nextY].top = false;
            }
            else
            {
                _maps[currentFace][currentX, currentY].top = false;
                _maps[nextFace][nextX, nextY].bottom = false;
            }
        }

        // bottom, back間
        else if ((currentFace == BOTTOM && nextFace == BACK)
                || (currentFace == BACK && nextFace == BOTTOM))
        {
            if (currentFace == BOTTOM)
            {
                _maps[currentFace][currentX, currentY].left = false;
                _maps[nextFace][nextX, nextY].right = false;
            }
            else
            {
                _maps[currentFace][currentX, currentY].right = false;
                _maps[nextFace][nextX, nextY].left = false;
            }
        }

        // bottom, Right間
        else if ((currentFace == BOTTOM && nextFace == RIGHT)
                || (currentFace == RIGHT && nextFace == BOTTOM))
        {
            if (currentFace == BOTTOM)
            {
                _maps[currentFace][currentX, currentY].top = false;
                _maps[nextFace][nextX, nextY].left = false;
            }
            else
            {
                _maps[currentFace][currentX, currentY].left = false;
                _maps[nextFace][nextX, nextY].top = false;
            }
        }

        // bottom, left間
        else if ((currentFace == BOTTOM && nextFace == LEFT)
                || (currentFace == LEFT && nextFace == BOTTOM))
        {
            if (currentFace == BOTTOM)
            {
                _maps[currentFace][currentX, currentY].bottom = false;
                _maps[nextFace][nextX, nextY].left = false;
            }
            else
            {
                _maps[currentFace][currentX, currentY].left = false;
                _maps[nextFace][nextX, nextY].bottom = false;
            }
        }

        // back, top間
        else if ((currentFace == BACK && nextFace == TOP)
                || (currentFace == TOP && nextFace == BACK))
        {
            if (currentFace == BACK)
            {
                _maps[currentFace][currentX, currentY].left = false;
                _maps[nextFace][nextX, nextY].right = false;
            }
            else
            {
                _maps[currentFace][currentX, currentY].right = false;
                _maps[nextFace][nextX, nextY].left = false;
            }
        }

        // back, Right間
        else if ((currentFace == BACK && nextFace == RIGHT)
                || (currentFace == RIGHT && nextFace == BACK))
        {
            if (currentFace == BACK)
            {
                _maps[currentFace][currentX, currentY].top = false;
                _maps[nextFace][nextX, nextY].top = false;
            }
            else
            {
                _maps[currentFace][currentX, currentY].top = false;
                _maps[nextFace][nextX, nextY].top = false;
            }
        }

        // back, Left間
        else if ((currentFace == BACK && nextFace == LEFT)
                || (currentFace == LEFT && nextFace == BACK))
        {
            if (currentFace == BACK)
            {
                _maps[currentFace][currentX, currentY].bottom = false;
                _maps[nextFace][nextX, nextY].bottom = false;
            }
            else
            {
                _maps[currentFace][currentX, currentY].bottom = false;
                _maps[nextFace][nextX, nextY].bottom = false;
            }
        }

        // top, right間
        else if ((currentFace == TOP && nextFace == RIGHT)
                || (currentFace == RIGHT && nextFace == TOP))
        {
            if (currentFace == TOP)
            {
                _maps[currentFace][currentX, currentY].top = false;
                _maps[nextFace][nextX, nextY].right = false;
            }
            else
            {
                _maps[currentFace][currentX, currentY].right = false;
                _maps[nextFace][nextX, nextY].top = false;
            }
        }

        // top, left間
        else if ((currentFace == TOP && nextFace == LEFT)
                || (currentFace == LEFT && nextFace == TOP))
        {
            if (currentFace == TOP)
            {
                _maps[currentFace][currentX, currentY].bottom = false;
                _maps[nextFace][nextX, nextY].right = false;
            }
            else
            {
                _maps[currentFace][currentX, currentY].right = false;
                _maps[nextFace][nextX, nextY].bottom = false;
            }
        }
    }
}
