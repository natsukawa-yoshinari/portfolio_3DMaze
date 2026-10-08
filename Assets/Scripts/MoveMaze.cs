using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

public class MoveMaze
{
    private CancellationTokenSource _cts = new();
    // private float _offset = 0.5f;
    private float _mazeHeight = 0f;
    private List<Vector3> _faceTarget = new();

    public MoveMaze(float mazeHeight)
    {
        _mazeHeight = mazeHeight;
        float halfHeight = _mazeHeight * 0.5f;

        Vector3 frontPos = new Vector3(-halfHeight, 0f, 0f);
        Vector3 bottomPos = new Vector3(0f, -halfHeight, 0f);
        Vector3 backPos = new Vector3(halfHeight, 0f, 0f);
        Vector3 topPos = new Vector3(0f, halfHeight, 0f);
        Vector3 rightPos = new Vector3(0f, 0f, -halfHeight);
        Vector3 leftPos = new Vector3(0f, 0f, halfHeight);

        _faceTarget.Add(frontPos);
        _faceTarget.Add(bottomPos);
        _faceTarget.Add(backPos);
        _faceTarget.Add(topPos);
        _faceTarget.Add(leftPos);
        _faceTarget.Add(rightPos);
    }

    public void InitializeFacePos(List<Transform> faceParents)
    {
        float distance = _mazeHeight;
        faceParents[1].localPosition = new Vector3(-distance, 0f, 0f);
        faceParents[2].localPosition = new Vector3(-distance * 2, 0f, 0f);
        faceParents[3].localPosition = new Vector3(distance, 0f, 0f);
        faceParents[4].localPosition = new Vector3(0f, 0f, distance);
        faceParents[5].localPosition = new Vector3(0f, 0f, -distance);
    }

    public async UniTask MoveMazeToSquare(List<Transform> faceList)
    {
        RotateFace(faceList[0], new Vector3(0f, 0f, 90f)).Forget();
        await MoveFace(faceList[0], _faceTarget[0]);

        RotateFace(faceList[1], new Vector3(0f, 0f, 180f)).Forget();
        await MoveFace(faceList[1], _faceTarget[1]);

        RotateFace(faceList[2], new Vector3(0f, 0f, 270f)).Forget();
        await MoveFace(faceList[2], _faceTarget[2]);

        RotateFace(faceList[3], new Vector3(0f, 0f, 0f)).Forget();
        await MoveFace(faceList[3], _faceTarget[3]);

        RotateFace(faceList[4], new Vector3(0f, 90f, 90f)).Forget();
        await MoveFace(faceList[4], _faceTarget[4]);

        RotateFace(faceList[5], new Vector3(0f, -90f, 90f)).Forget();
        await MoveFace(faceList[5], _faceTarget[5]);
    }

    private async UniTask RotateFace(Transform face, Vector3 angle)
    {
        await LMotion.Create(Vector3.zero, angle, 0.5f)
                    .Bind(value =>
                    {
                        face.localRotation
                            = Quaternion.Euler(value);
                    }).ToUniTask(_cts.Token);
    }

    private async UniTask MoveFace(Transform face, Vector3 target)
    {
        Vector3 from = face.position;
        await LMotion.Create(from, target, 0.5f)
                    .Bind(value =>
                    {
                        face.localPosition = value;
                    }).ToUniTask(_cts.Token);
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
