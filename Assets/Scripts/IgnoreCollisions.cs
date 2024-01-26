using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisions : MonoBehaviour
{
    [SerializeField] private Collider2D _mainCollider;
    [SerializeField] private Collider2D[] _colliders;

    private void Start()
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            for (int j = i + 1; j < _colliders.Length; j++)
                Physics2D.IgnoreCollision(_colliders[i], _colliders[j]);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            Physics2D.IgnoreCollision(_mainCollider, collision.gameObject.GetComponent<Collider2D>());
    }
}
