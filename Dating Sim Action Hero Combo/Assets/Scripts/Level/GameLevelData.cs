using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Level Data")]
public class GameLevelData : ScriptableObject
{
    [SerializeField] private MapData _mapData;
    [SerializeField] private List<Quest> _questDatas = new List<Quest>();
    [SerializeField] private List<EnemyData> _enemyDatas = new List<EnemyData>();
    // civilian data types
    // law enforcement data types?
    // cutscene data?
    // dialog data?
    
    public MapData MapData => _mapData;
    public IReadOnlyList<Quest> QuestDatas => _questDatas;
    public List<EnemyData> EnemyDatas => _enemyDatas;
}
