using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TileData")]
public class TileData : ScriptableObject
{
    [SerializeField] private Sprite _tileSprite;
    public Sprite TileSprite => _tileSprite;
    [SerializeField] private bool _isSolid;
    public bool IsSolid => _isSolid;
    [SerializeField] private int _health;
    public int Health => _health;
}
