using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb2D;
    [SerializeField] private float _targetRot;
    [SerializeField] private float _force;

    private void Update()
    {
        _rb2D.MoveRotation(Mathf.LerpAngle(_rb2D.rotation, _targetRot, _force * Time.deltaTime));
    }
}
