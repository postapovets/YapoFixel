using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipController : MonoBehaviour
{
    private Camera _camera;
    private Vector3 screenPoint;
    private Vector3 _offset;
    private float _offsetZ;
    private GameManager _gameManager;
    [SerializeField] SpriteRenderer _spriteRenderer;
    bool _isDisabled;

    void Awake()
    {
        _camera = Camera.main;
    }

    public void Init(GameManager gameManager, Color color)
    {
        _gameManager = gameManager;
        _spriteRenderer.color = color;
    }

    void OnMouseDown()
    {
        if (_isDisabled) return;
        _offsetZ = Mathf.Repeat(_offsetZ + 0.1f, 1.0f);
        transform.position = new Vector3(transform.position.x, transform.position.y, - _offsetZ);
        screenPoint = _camera.WorldToScreenPoint(transform.position);
        _offset = transform.position - _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        if (_isDisabled) return;
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = _camera.ScreenToWorldPoint(cursorPoint) + _offset;
        transform.position = cursorPosition;
    }

    private void OnMouseUpAsButton()
    {
        if (_isDisabled) return;
        _gameManager.ChipReleased(_camera.WorldToScreenPoint(transform.position));
    }

    public void SetDisabled()
    {
        _isDisabled = true;
    }
}
