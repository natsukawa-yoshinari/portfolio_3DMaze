using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class MazePresenter : MonoBehaviour
{
    /// <summary>
    /// MoveMaze.cs と MazeCreater.csを繋いて、
    /// GameManagerから迷路生成を一括で呼べるようにする
    /// </summary>

    private int _mazeHeight = 5;
    private int _mazeWidth = 0;
    [SerializeField]
    private float _genTimePerCell = 0.02f;

    [SerializeField]
    private GameObject _cellPrefab;
    [SerializeField]
    private GameObject _cornerBasePrefab;
    [SerializeField]
    private GameObject _vertexCornerPrefab;
    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private Transform _mazeParent;
    [SerializeField]
    private List<Transform> _faceParents;

    [SerializeField]
    private PlayerInput _playerInput;

    private float _offset = 0.5f;
    private MazeCreator _mazeCreator;
    private MoveMaze _moveMaze;

    private void Start()
    {

        _playerInput.doGetGoal.Skip(1)
            .Subscribe(x =>
            {
                _mazeCreator.GetGoalAsync().Forget();
            })
            .AddTo(this);
    }

    public void StartGame(int mazeHeight)
    {
        _mazeHeight = mazeHeight;
        _mazeWidth = _mazeHeight;

        float cameraY = _mazeHeight * 2.6f + 4.6f;
        Camera.main.transform.position = new Vector3(0f, cameraY, 0f);

        _mazeCreator = new MazeCreator(_mazeWidth, _mazeHeight, _genTimePerCell);
        _moveMaze = new MoveMaze(_mazeHeight);
        _moveMaze.InitializeFacePos(_faceParents);

        OnGameStart().Forget();
    }


    private async UniTask OnGameStart()
    {
        await CreateMazeAsync();
        await UniTask.WaitForSeconds(0.5f);
        await MoveMazeAsync();
        await SetCorners();
        _playerInput.EnableInput();
        _playerInput.SetActiveOrientation(true);
        SpawnPlayer();
    }

    private async UniTask CreateMazeAsync()
        => await CreateMaze(_mazeWidth, _mazeHeight);
    private async UniTask MoveMazeAsync()
        => await _moveMaze.MoveMazeToSquare(_faceParents);

    private async UniTask CreateMaze(int width, int height)
    {
        List<Vector3> objPosList = _mazeCreator.InitializeMaze();
        _mazeCreator.SetMapCells(InstantiateCells(objPosList));

        CellTransform startPos = _mazeCreator.SetStartPos(MazeCreator.Face.front, 0, 0);
        Stack<CellTransform> stack = new Stack<CellTransform>();
        stack.Push(startPos);

        await _mazeCreator.SetMaze(stack);
    }

    private Dictionary<MazeCreator.Face, CellObject[,]>
                    InstantiateCells(List<Vector3> objPosList)
    {
        Dictionary<MazeCreator.Face, CellObject[,]> mapCells = new();

        CellObject[,] cells = new CellObject[_mazeWidth, _mazeHeight];
        int y = 0, x = 0, face = 0;

        foreach (Vector3 pos in objPosList)
        {
            // CellObjの生成
            GameObject obj = Instantiate(_cellPrefab, pos, Quaternion.identity);
            obj.transform.SetParent(_faceParents[face]);

            cells[x, y] = obj.GetComponent<CellObject>();
            y += 1;
            if (y >= _mazeHeight)
            {
                y = 0;
                x += 1;
            }
            if (x >= _mazeWidth)
            {
                mapCells.Add((MazeCreator.Face)face, cells);

                x = 0;
                y = 0;
                face += 1;
                cells = new CellObject[_mazeWidth, _mazeHeight];
            }
        }
        return mapCells;
    }

    private async UniTask SetCorners()
    {
        float x = (_mazeHeight * 0.5f + _offset) * -1f;
        float y = _mazeHeight * 0.5f;
        float z = (_mazeHeight * 0.5f - _offset) * -1f;

        Vector3 topLeftPos = new Vector3(-x, y, z);
        Vector3 topRightPos = new Vector3(x, y, z);
        Vector3 topRightRot = new Vector3(0f, 180f, 0f);
        Vector3 bottomRightPos = new Vector3(x, -y, z);
        Vector3 bottomRightRot = new Vector3(180f, 180f, 0f);
        Vector3 bottomLeftPos = new Vector3(-x, -y, z);
        Vector3 bottomLeftRot = new Vector3(180f, 0f, 0f);

        GameObject obj;

        obj = Instantiate(_vertexCornerPrefab, new Vector3(-x, y, -z + 1), Quaternion.identity);
        obj.transform.SetParent(_mazeParent);

        obj = Instantiate(_vertexCornerPrefab, new Vector3(x, y, -z + 1), Quaternion.Euler(new Vector3(0f, -90f, 0f)));
        obj.transform.SetParent(_mazeParent);

        obj = Instantiate(_vertexCornerPrefab, new Vector3(x, y, z - 1), Quaternion.Euler(new Vector3(0f, -180f, 0f)));
        obj.transform.SetParent(_mazeParent);

        obj = Instantiate(_vertexCornerPrefab, new Vector3(-x, y, z - 1), Quaternion.Euler(new Vector3(0f, -270f, 0f)));
        obj.transform.SetParent(_mazeParent);


        obj = Instantiate(_vertexCornerPrefab, new Vector3(-x, -y, -z + 1), Quaternion.Euler(new Vector3(180f, -90f, 0f)));
        obj.transform.SetParent(_mazeParent);

        obj = Instantiate(_vertexCornerPrefab, new Vector3(x, -y, -z + 1), Quaternion.Euler(new Vector3(180f, -180f, 0f)));
        obj.transform.SetParent(_mazeParent);

        obj = Instantiate(_vertexCornerPrefab, new Vector3(x, -y, z - 1), Quaternion.Euler(new Vector3(180f, -270f, 0f)));
        obj.transform.SetParent(_mazeParent);

        obj = Instantiate(_vertexCornerPrefab, new Vector3(-x, -y, z - 1), Quaternion.Euler(new Vector3(180f, 0f, 0f)));
        obj.transform.SetParent(_mazeParent);

        float half = _mazeHeight * 0.5f;
        float offsetHalf = half + 0.5f;
        float startZ = -half + 0.5f;
        for (int i = 0; i < _mazeHeight; i++)
        {
            float currentZ = startZ + i;

            // Z軸方向の辺（前後・上下 4本）
            InstantiateCorner(_cornerBasePrefab, new Vector3(-offsetHalf, half, currentZ), Quaternion.Euler(0f, 180f, 0f));
            InstantiateCorner(_cornerBasePrefab, new Vector3(offsetHalf, half, currentZ), Quaternion.Euler(0f, 0f, 0f));
            InstantiateCorner(_cornerBasePrefab, new Vector3(offsetHalf, -half, currentZ), Quaternion.Euler(180f, 0f, 0f));
            InstantiateCorner(_cornerBasePrefab, new Vector3(-offsetHalf, -half, currentZ), Quaternion.Euler(180f, 180f, 0f));

            // X軸方向の辺（左右・上下 4本）
            InstantiateCorner(_cornerBasePrefab, new Vector3(currentZ, half, -offsetHalf), Quaternion.Euler(0f, 90f, 0f));
            InstantiateCorner(_cornerBasePrefab, new Vector3(currentZ, half, offsetHalf), Quaternion.Euler(0f, -90f, 0f));
            InstantiateCorner(_cornerBasePrefab, new Vector3(currentZ, -half, -offsetHalf), Quaternion.Euler(180f, 90f, 0f));
            InstantiateCorner(_cornerBasePrefab, new Vector3(currentZ, -half, offsetHalf), Quaternion.Euler(180f, -90f, 0f));

            // Y軸方向の辺（垂直 4本）
            InstantiateCorner(_cornerBasePrefab, new Vector3(-offsetHalf, currentZ, -offsetHalf + 0.5f), Quaternion.Euler(90f, 0f, 180f));
            InstantiateCorner(_cornerBasePrefab, new Vector3(offsetHalf - 0.5f, currentZ, -offsetHalf), Quaternion.Euler(90f, 90f, 0f));
            InstantiateCorner(_cornerBasePrefab, new Vector3(offsetHalf, currentZ, offsetHalf - 0.5f), Quaternion.Euler(90f, 180f, 180f));
            InstantiateCorner(_cornerBasePrefab, new Vector3(-offsetHalf + 0.5f, currentZ, offsetHalf), Quaternion.Euler(90f, 270f, 0f));

            // 生成時の演出ディレイ
            await UniTask.WaitForSeconds(0.1f);
        }
    }

    private void InstantiateCorner(GameObject cornerBasePrefab, Vector3 vector3, Quaternion quaternion)
    {
        GameObject obj = Instantiate(cornerBasePrefab, vector3, quaternion);
        obj.transform.SetParent(_mazeParent);
    }

    private void SpawnPlayer()
    {
        Vector3 spawnPos = new Vector3(0f, _mazeHeight * 0.5f, 0f);
        GameObject obj = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);
        obj.transform.SetParent(_mazeParent);
        _playerInput.SetPlayer(obj);
    }
}
