using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    private bool _isHolding;
    public bool IsHolding { get => _isHolding; set => _isHolding = value; }

    private Rigidbody2D _otherRb2D = null;
    public Rigidbody2D OtherRb2D => _otherRb2D;

    private FixedJoint2D _tempFJ2D = null;
    public FixedJoint2D TempFJ2D => _tempFJ2D;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isHolding)
        {
            if (collision.gameObject.TryGetComponent(out Rigidbody2D otherRb2D))
                _otherRb2D = otherRb2D;

            if (_otherRb2D)
            {
                if (_tempFJ2D)
                    Destroy(_tempFJ2D);

                /*if (TryGetComponent(out FixedJoint2D leftoverJoint))
                    Destroy(leftoverJoint);*/

                _tempFJ2D = transform.gameObject.AddComponent<FixedJoint2D>();
                _tempFJ2D.connectedBody = _otherRb2D;
            }
        }
        else
        {
            if (_tempFJ2D)
                Destroy(_tempFJ2D);
        }
    }
}
