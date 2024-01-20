using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public float speed;

    private Vector2 _movement;
    private Rigidbody2D _rigid;

    public static PlayerManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void Reset()
    {
        _rigid.velocity = Vector2.zero;
        transform.localScale = Vector2.one;
        transform.position = Vector2.zero;
    }

    public void Load(Vector3 position, Vector2 scale)
    {
        _rigid.velocity = Vector2.zero;
        transform.position = position;
        transform.localScale = scale;
    }
}

