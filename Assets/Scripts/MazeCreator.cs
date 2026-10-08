using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MazeCreator
{
    private const float RANDOM_BREAK_RATE = 0.05f;

    private int _mazeWidth = 0;
    private int _mazeHeight = 0;

    public enum Face
    {
        None = -1,
        front = 0,
        bottom,
        back,
        top,
        right,
        left,
    }

    // コンストラクタ
    public MazeCreator(int width, int height, float genTimePerCell)
    {
        _mazeWidth = width;
        _mazeHeight = height;
        _maps = new Dictionary<Face, MazeCell[,]>();
        _genTimePerCell = genTimePerCell;
    }

    private Dictionary<Face, MazeCell[,]> _maps;
    private Dictionary<Face, CellObject[,]> _mapCells;
    private float _genTimePerCell;
    private FindGoal _findGoal;

    // 初期化の実行と展開図上でのCellObjectのPositionを返す
    public List<Vector3> InitializeMaze()
    {
        List<Vector3> objPosList = new List<Vector3>();

        for (int face = 0; face < 6; face++)
        {
            MazeCell[,] maze = new MazeCell[_mazeWidth, _mazeHeight];

            for (int x = 0; x < _mazeWidth; x++)
            {
                for (int y = 0; y < _mazeHeight; y++)
                {
                    // 初期化
                    maze[x, y] = new MazeCell((Face)face, x, y);

                    // maze[x, y]に対応するCellObjectのPosition
                    float posX = x - (_mazeWidth / 2f) + GetFaceXOffset((Face)face) + 0.5f;
                    float posZ = y - (_mazeHeight / 2f) + GetFaceZOffset((Face)face) + 0.5f;
                    objPosList.Add(new Vector3(posX, 0f, posZ));
                }
            }

            // faceごとにmazeを管理
            switch (face)
            {
                case 0: _maps.Add(Face.front, maze); break;
                case 1: _maps.Add(Face.bottom, maze); break;
                case 2: _maps.Add(Face.back, maze); break;
                case 3: _maps.Add(Face.top, maze); break;
                case 4: _maps.Add(Face.right, maze); break;
                case 5: _maps.Add(Face.left, maze); break;
            }
        }

        return objPosList;
    }

    // 各面における座標のOffset計算
    private float GetFaceXOffset(Face face)
    {
        switch (face)
        {
            case Face.front: return 0f;
            case Face.bottom: return _mazeWidth * -1f;
            case Face.back: return (_mazeWidth * 2f) * -1f;
            case Face.top: return _mazeWidth;
            case Face.right: return 0f;
            case Face.left: return 0f;

            default: return 0f;
        }
    }
    private float GetFaceZOffset(Face face)
    {
        switch (face)
        {
            case Face.front: return 0f;
            case Face.bottom: return 0f;
            case Face.back: return 0f;
            case Face.top: return 0f;
            case Face.right: return _mazeHeight;
            case Face.left: return _mazeHeight * -1f;

            default: return 0f;
        }
    }

    // ObjectのInstanceをPresenterからもらう
    public void SetMapCells(Dictionary<Face, CellObject[,]> mapCells)
        => _mapCells = mapCells;

    // 初期地点の到達フラグを立てる
    public CellTransform SetStartPos(Face face, int posX, int posY)
    {
        _maps[face][posX, posY].visited = true;

        return new CellTransform()
        {
            cellFace = face,
            cellPos = new Vector2(posX, posY),
        };
    }

    /// <summary>
    /// DFS(深さ有線探索)を利用した迷路生成アルゴリズム
    /// Cellが立法体のどこの面にあるかが重要なため、PositionとFaceを持つCellTransformクラスを作成、利用
    /// </summary>
    public async UniTask SetMaze(Stack<CellTransform> visitedStack)
    {
        // 探索可能場所の検索クラス
        CalcCanMovePos calcCanMovePos = new CalcCanMovePos(_mazeWidth);
        CalcBreakWall calcBreakWall = new CalcBreakWall(_maps, _mapCells);

        // DFS
        while (visitedStack.Count > 0)
        {
            // 最後に訪れた場所を現在地に
            CellTransform current = visitedStack.Peek();

            // 移動可能な場所を検索
            List<CellTransform> moveList = new();
            calcCanMovePos.SetCanMovePos(_maps, moveList, current);

            // どこにも行けなくなったら一つ戻る
            if (moveList.Count == 0)
            {
                _mapCells[current.cellFace][(int)current.cellPos.x, (int)current.cellPos.y]
                        .SetCellColor(Color.softRed);
                visitedStack.Pop();
                await UniTask.WaitForSeconds(_genTimePerCell);
                continue;
            }

            // 次の地点をランダムに決める
            int randomId = Random.Range(0, moveList.Count);
            CellTransform next = new CellTransform()
            {
                cellFace = moveList[randomId].cellFace,
                cellPos = moveList[randomId].cellPos,
            };

            // 穴あけ
            calcBreakWall.BreakWall(current, next);
            await UniTask.WaitForSeconds(_genTimePerCell);

            // 到達フラグを立てる
            _maps[next.cellFace][(int)next.cellPos.x, (int)next.cellPos.y].visited = true;
            _mapCells[next.cellFace][(int)next.cellPos.x, (int)next.cellPos.y]
                    .SetCellColor(Color.softGreen);
            visitedStack.Push(next);
        }

        BreakRandomWalls();

        // 後のゴール探索のためにVisitedをFalseに戻す
        for (int face = 0; face < 6; face++)
        {
            for (int x = 0; x < _mazeWidth; x++)
            {
                for (int y = 0; y < _mazeHeight; y++)
                {
                    _maps[(Face)face][x, y].visited = false;
                    _mapCells[(Face)face][x, y].SetDefaultColor();
                    _mapCells[(Face)face][x, y].DeleteWall(_maps[(Face)face][x, y]);
                }
            }
        }

        // ゴール地点を設定する
        int half = _mazeHeight / 2;
        _mapCells[Face.top][half, half].SetCellColor(Color.softRed);

        Color goal = new Color(0x60 / 256f, 0x6e / 256f, 0xb2 / 256f);
        _mapCells[Face.bottom][half, half].SetCellColor(goal);
        _mapCells[Face.bottom][half, half].gameObject.AddComponent<GoalCell>();

        _findGoal = new FindGoal(_maps, _mapCells, _mazeWidth);
    }

    public async UniTask GetGoalAsync()
    {
        await _findGoal.GetGoal();

    }

    // ランダムな壁を破壊し自然な迷路にする
    public void BreakRandomWalls()
    {
        CalcBreakWall calcBreakWall = new CalcBreakWall(_maps, _mapCells);
        for (int face = 0; face < 6; face++)
        {
            for (int x = 0; x < _mazeWidth; x++)
            {
                for (int y = 0; y < _mazeHeight; y++)
                {
                    CellTransform current = new CellTransform
                    {
                        cellFace = (Face)face,
                        cellPos = new Vector2(x, y)
                    };

                    if (x < _mazeWidth - 1 && _maps[(Face)face][x, y].right)
                    {
                        if (Random.value < RANDOM_BREAK_RATE)
                        {
                            CellTransform next = new CellTransform
                            {
                                cellFace = (Face)face,
                                cellPos = new Vector2(x + 1, y)
                            };
                            calcBreakWall.BreakWall(current, next);
                        }
                    }

                    if (y < _mazeHeight - 1 && _maps[(Face)face][x, y].top)
                    {
                        if (Random.value < RANDOM_BREAK_RATE)
                        {
                            CellTransform next = new CellTransform
                            {
                                cellFace = (Face)face,
                                cellPos = new Vector2(x, y + 1)
                            };
                            calcBreakWall.BreakWall(current, next);
                        }
                    }
                }
            }
        }
    }
}
