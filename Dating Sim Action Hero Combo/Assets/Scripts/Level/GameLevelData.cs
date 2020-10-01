using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Level Data")]
public class GameLevelData : ScriptableObject
{
    [SerializeField] private MapData _mapData;
    [SerializeField] private List<Quest> _missionQuestDatas = new List<Quest>();
    [SerializeField] private List<Quest> _dateQuestdatas = new List<Quest>();
    [SerializeField] private List<EnemyData> _enemyDatas = new List<EnemyData>();
    [SerializeField] private DateData _dateData;
    // civilian data types
    // law enforcement data types?
    // cutscene data?
    // dialog data?
    
    public MapData MapData => _mapData;
    public IReadOnlyList<Quest> MissionQuestDatas => _missionQuestDatas;
    public IReadOnlyList<Quest> DateQuestDatas => _dateQuestdatas;
    public List<EnemyData> EnemyDatas => _enemyDatas;
    public DateData DateData => _dateData;
}
