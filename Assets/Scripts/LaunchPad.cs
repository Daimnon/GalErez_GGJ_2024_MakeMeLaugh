using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [Range(100.0f, 1000.0f)][SerializeField] private float _launchForce = 500.0f;
    private const string _playerTag = "Player";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject go = collision.gameObject;
        if (go.CompareTag(_playerTag))
            go.GetComponentInParent<PlayerController>().Rb2D.AddForce(_launchForce * Vector2.up, ForceMode2D.Impulse);
    }
}
