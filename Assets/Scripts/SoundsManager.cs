using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bgMusic;

    private void Awake()
    {
        _bgMusic.volume = 0.1f;
    }
    private void Start()
    {
        _bgMusic.volume = 0.1f;
    }
}
