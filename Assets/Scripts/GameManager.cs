using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _startCanvas;
    [SerializeField]
    private GameObject _endCanvas;
    [SerializeField]
    private TMP_Dropdown _mazeSizeDropdown;
    [SerializeField]
    private MazePresenter _mazePresenter;

    static public bool isGameEnd = false;

    // mazeの大きさをDropDownに設定する
    private readonly List<string> _sizeOptions = new List<string>
    {
        "Size: 3",
        "Size: 5",
        "Size: 7",
        "Size: 9",
    };
    private readonly int[] _sizeValues = { 3, 5, 7, 9 };

    private void Start()
    {
        isGameEnd = false;

        _mazeSizeDropdown.ClearOptions();
        _mazeSizeDropdown.AddOptions(_sizeOptions);
        _mazeSizeDropdown.value = 1;

        _startCanvas.SetActive(true);
        _endCanvas.SetActive(false);
    }

    private void Update()
    {
        if (isGameEnd && Time.timeScale != 0f)
        {
            // EndCanvasを表示
            Time.timeScale = 0f;
            _endCanvas.SetActive(true);
        }
    }

    public void OnStartButtonClicked()
    {
        int selectedIndex = _mazeSizeDropdown.value;
        int mazeHeight = _sizeValues[selectedIndex];

        _startCanvas.SetActive(false);
        _mazePresenter.StartGame(mazeHeight);
    }

    public void OnRetryButtonClicked()
    {
        isGameEnd = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
