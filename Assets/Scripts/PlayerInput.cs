using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    private const int MOUSE_DELTA = 0;
    private const int LEFT_CLICK = 1;
    private const int KEY_Q = 2;
    private const int KEY_E = 3;

    [SerializeField]
    private List<InputAction> _inputs;

    [SerializeField]
    private Transform _mazeTransform;
    [SerializeField]
    private Transform _orientation;
    [SerializeField]
    private GameObject _orientationView;

    [SerializeField]
    private Toggle _xToggle;
    [SerializeField]
    private Toggle _yToggle;
    [SerializeField]
    private Toggle _zToggle;
    [SerializeField]
    private GameObject _dummyPlayer;

    private float _xInvert = 1f;
    private float _yInvert = 1f;
    private float _zInvert = 1f;
    private GameObject _playerObj;
    private GameObject _dummyObj;
    private Rigidbody _playerRb;
    private bool _isPlayerStop = false;
    public ReactiveProperty<bool> doGetGoal = new(false);

    private void Start()
    {
        _xToggle.onValueChanged.AddListener(_ => _xInvert *= -1f);
        _yToggle.onValueChanged.AddListener(_ => _yInvert *= -1f);
        _zToggle.onValueChanged.AddListener(_ => _zInvert *= -1f);
    }

    private void Update()
    {
        // 左クリックされてなかったらReturn
        if (_inputs[LEFT_CLICK].IsPressed())
        {
            Vector2 delta = _inputs[MOUSE_DELTA].ReadValue<Vector2>();

            float directionY = 0f;
            if (_inputs[KEY_Q].IsPressed()) directionY = -1f;
            else if (_inputs[KEY_E].IsPressed()) directionY = 1f;

            directionY *= 40f * Time.deltaTime;
            delta *= 10f * Time.deltaTime;

            Vector3 addRot = new Vector3(delta.y * _xInvert,
                                        directionY * _yInvert,
                                        -delta.x * _zInvert);

            _mazeTransform.Rotate(addRot, Space.World);

            Quaternion rot = _mazeTransform.rotation;
            _orientation.rotation = rot;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            ConstraintPlayer(_isPlayerStop);

        if (Keyboard.current.gKey.wasPressedThisFrame)
            doGetGoal.Value = true;
    }

    public void SetActiveOrientation(bool value)
    {
        _orientation.gameObject.SetActive(value);
        _orientationView.gameObject.SetActive(value);
    }

    public void SetPlayer(GameObject obj)
    {
        _playerObj = obj;
        _playerRb = obj.GetComponent<Rigidbody>();
    }

    public void ConstraintPlayer(bool value)
    {
        if (value == false)
        {
            _isPlayerStop = true;
            _playerRb.isKinematic = true;

            if (_dummyObj == null) _dummyObj = Instantiate(_dummyPlayer);
            else _dummyObj.SetActive(true);
            _dummyObj.transform.localPosition = _playerObj.transform.localPosition;
            _dummyObj.transform.SetParent(_mazeTransform.GetChild(0));

            _playerObj.SetActive(false);
        }
        else
        {
            _isPlayerStop = false;
            _playerRb.isKinematic = false;

            _dummyObj.SetActive(false);
            _playerObj.transform.localPosition = _dummyObj.transform.localPosition;

            _playerObj.SetActive(true);
        }
    }

    public void EnableInput()
    {
        foreach (InputAction input in _inputs)
            input.Enable();
    }

    public void DisableInput()
    {
        foreach (InputAction input in _inputs)
            input.Disable();
    }

}
