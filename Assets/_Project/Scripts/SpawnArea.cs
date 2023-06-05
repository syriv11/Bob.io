using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    [SerializeField] Collider2D _collider;

    public Collider2D Collider { get => _collider; }
}
