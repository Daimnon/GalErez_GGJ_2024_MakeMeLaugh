using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Transform _checkpoint;
    [SerializeField] private float _respawnTime = 0.5f;
    private const string _playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_playerTag))
            StartCoroutine(RespawnAtCheckpoint(_respawnTime, collision.GetComponentInParent<PlayerController>()));
    }

    private IEnumerator RespawnAtCheckpoint(float duration, PlayerController playerController)
    {
        float time = 0;
        Rigidbody2D playerBodyRb2D = playerController.Rb2D;
        Vector3 startPosition = playerBodyRb2D.position;

        IgnoreCollisions ignoreC = playerController.GetComponent<IgnoreCollisions>();

        for (int i = 0; i < ignoreC.Coliders.Length; i++)
        {
            Rigidbody2D currentRb2D = ignoreC.Coliders[i].GetComponent<Rigidbody2D>();
            ignoreC.Coliders[i].isTrigger = true;
            currentRb2D.isKinematic = true;
            currentRb2D.velocity = Vector2.zero;
        }

        while (time < duration)
        {
            for (int i = 0; i < ignoreC.Coliders.Length; i++)
            {
                Rigidbody2D currentRb2D = ignoreC.Coliders[i].GetComponent<Rigidbody2D>();
                currentRb2D.position = Vector3.Lerp(startPosition, _checkpoint.position, time / duration);
            }
            
            time += Time.deltaTime;
            yield return null;
        }
        playerBodyRb2D.position = _checkpoint.position;

        for (int i = 0; i < ignoreC.Coliders.Length; i++)
        {
            Rigidbody2D currentRb2D = ignoreC.Coliders[i].GetComponent<Rigidbody2D>();
            currentRb2D.velocity = Vector2.zero;
            currentRb2D.isKinematic = false;
            ignoreC.Coliders[i].isTrigger = false;
        }
    }
}
