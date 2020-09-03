using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Data")]
public class MapData : ScriptableObject
{
    [SerializeField] private int _mapSizeX;
    [SerializeField] private int _mapSizeY;
    [SerializeField] private TileData _defaultTileData;
    [SerializeField] private List<TileData> _tileDatas = new List<TileData>();

    public int MapSizeX => _mapSizeX;
    public int MapSizeY => _mapSizeX;
    public TileData DefaultTileData => _defaultTileData;
    public IReadOnlyList<TileData> TileDatas => _tileDatas;
}
