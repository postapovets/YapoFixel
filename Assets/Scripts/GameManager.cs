using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private UIController _uiController;
    [SerializeField] private RenderTexture _originalArt;
    [SerializeField] private RenderTexture _repairedArt;

    [SerializeField] private Camera _cameraOriginalArt;
    [SerializeField] private Camera _cameraRepairedArt;
    [SerializeField] private RawImage _originalArtDebug;
    [SerializeField] private RawImage _repairedArtDebug;

    [SerializeField] int _RTColorizeSize = 3;
    [SerializeField] int _RTAnalizeSize = 8;

    [SerializeField] private List<GameObject> _prefabChip;
    [SerializeField] private Transform _chipsRootNode;

    //EndLevelPanel
    [SerializeField] private RectTransform _endLevelPanel;
    [SerializeField] private Text _scoreText;
    //

    private List<Color> _colors;
    private bool _gameStarted;
    private List<ChipController> _chips;

    void Start()
    {
        _endLevelPanel.gameObject.SetActive(false);
    }

    public void GetColors()
    {
        SetTextureSize(_RTColorizeSize, _cameraOriginalArt, _originalArtDebug);

        _colors = GetColorsList(_cameraOriginalArt.targetTexture);
        
        SetTextureSize(_RTAnalizeSize, _cameraOriginalArt, _originalArtDebug);
        SetTextureSize(_RTAnalizeSize, _cameraRepairedArt, _repairedArtDebug);
        GenerateChips();
    }

    public void GenerateChips()
    {
        OnStartGame();
        _chips = new List<ChipController>();

        if (_chipsRootNode.childCount > 0)
        {
            //destroy old chips
            for (int i = _chipsRootNode.childCount - 1; i >= 0; --i)
            {
                GameObject.Destroy(_chipsRootNode.GetChild(i).gameObject);
            }
        }

        float offsetY = 3.4f / (_RTAnalizeSize * _RTAnalizeSize);
        float posY = -0.5f;

        for (int i = 0; i < _RTAnalizeSize * _RTAnalizeSize; i++)
        {
            Transform tr = ((GameObject) Instantiate(_prefabChip[Random.Range(0, _prefabChip.Count)], _chipsRootNode, true)).transform;

            tr.SetPositionAndRotation(new Vector3(Random.Range(-0.5f, 0.5f) - 0.2f, posY, 0.0f), Quaternion.identity);
            posY += offsetY;

            ChipController chip = tr.GetComponent<ChipController>();
            chip.Init(this, _colors[(int)Mathf.Repeat(i, _colors.Count)]);
            _chips.Add(chip);
        }

    }

    public void Analyze()
    {
        float result = CompareRT(_cameraOriginalArt.targetTexture, _cameraRepairedArt.targetTexture);
        _scoreText.text = "Match:\n" + result + "%";
        OnEndGame();
    }

    void SetTextureSize(int size, Camera camera, RawImage image)
    {
        if (camera.targetTexture != null)
        {
            camera.targetTexture.Release();
        }

        camera.targetTexture = new RenderTexture(size, size, 24);
        camera.targetTexture.filterMode = FilterMode.Point;
        image.texture = camera.targetTexture;
        camera.Render();
    }

    float CompareRT(RenderTexture origin, RenderTexture repaired)
    {
        List<Color> colorsOrig = GetColorsList(origin);
        List<Color> colorsRepaired = GetColorsList(repaired);

        float result = 0.0f;
        float perc = 100.0f / colorsOrig.Count;

        for (int i = 0; i < colorsOrig.Count; i++)
        {
            if (IsEqualFloat(colorsOrig[i].r, colorsRepaired[i].r, 0.1f) &&
                IsEqualFloat(colorsOrig[i].g, colorsRepaired[i].g, 0.1f) &&
                IsEqualFloat(colorsOrig[i].b, colorsRepaired[i].b, 0.1f))
            {
                result += perc;
            }
        }

        return result;
    }

    List<Color> GetColorsList(RenderTexture rt)
    {
        List<Color> colors = new List<Color>();

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(_RTColorizeSize, _RTColorizeSize, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, _RTColorizeSize, _RTColorizeSize), 0, 0);
        RenderTexture.active = null;

        for (int i = 0; i < _RTColorizeSize; i++)
        {
            for (int j = 0; j < _RTColorizeSize; j++)
            {
                colors.Add(tex.GetPixel(i, j));
            }
        }
        DestroyImmediate(tex);

        return colors;
    }

    bool IsEqualFloat(float float1, float float2, float delta = 0.00001f)
    {
        return (float1 + delta >= float2) && (float1 - delta <= float2);
    }

    public void ChipReleased(Vector2 chipPosition)
    {
        if (!_gameStarted)
        {
            if ((chipPosition.x > 615 && chipPosition.x < 815) && (chipPosition.y > 315 && chipPosition.y < 525))
            {
                _gameStarted = true;
                _repairedArtDebug.gameObject.SetActive(false);
            }
        }
    }

    void OnStartGame()
    {
        _gameStarted = false;
        _repairedArtDebug.gameObject.SetActive(true);
        _endLevelPanel.gameObject.SetActive(false);
    }

    void OnEndGame()
    {
        _repairedArtDebug.gameObject.SetActive(true);
        _endLevelPanel.gameObject.SetActive(true);
        if (_chips != null)
        {
            foreach (var chip in _chips)
            {
                chip.SetDisabled();
            }
        }
    }

}
