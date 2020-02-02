using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickableObject
{

    void OnTouchBegan(Vector2 pos);

    void OnTouchEnded(Vector2 pos);

    void OnTouchMoved(Vector2 pos);
}
