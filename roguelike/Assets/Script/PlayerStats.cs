using System;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    [SerializeField]
    private float maxHp = 100f;
    [SerializeField]
    private float speed = 5f;

    public float MaxHp { get => maxHp; }
    public float Speed { get => speed; }

    // --- Sprint 3에서 구현할 스탯 ---
    // [SerializeField] private float magnetRange;
    // public float MagnetRange { get => magnetRange; }
    // ... (등등)
}