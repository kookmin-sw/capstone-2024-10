using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField]
    protected int _level;
    [SerializeField]
    protected float _moveSpeed;

    public int Level { get { return _level; } set {  _level = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set {  _moveSpeed = value; } }

    private void Start()
    {
        _level = 1;
        _moveSpeed = 5;
    }
}
