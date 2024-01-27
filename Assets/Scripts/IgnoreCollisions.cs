using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisions : MonoBehaviour
{
    [SerializeField] private Collider2D[] _colliders;
    public Collider2D[] Coliders => _colliders;

    private void Start()
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            for (int j = i + 1; j < _colliders.Length; j++)
                Physics2D.IgnoreCollision(_colliders[i], _colliders[j]);
        }
    }
}
