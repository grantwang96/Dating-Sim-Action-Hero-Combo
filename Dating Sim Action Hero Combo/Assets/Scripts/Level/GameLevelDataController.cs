using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelDataController : MonoBehaviour
{
    public static GameLevelDataController Instance { get; private set; }
    public GameLevelData CurrentGameLevelData { get; private set; }

    [SerializeField] private List<GameLevelData> _allGameLevelDatas = new List<GameLevelData>();
    private Dictionary<string, GameLevelData> _gameLevelDatasById = new Dictionary<string, GameLevelData>();

    private void Awake() {
        Instance = this;
        LoadAllGameLevelDatas();
        // temp
        SetCurrentGameLevelById(_allGameLevelDatas[0].name);
    }

    private void LoadAllGameLevelDatas() {
        for(int i = 0; i < _allGameLevelDatas.Count; i++) {
            _gameLevelDatasById.Add(_allGameLevelDatas[i].name, _allGameLevelDatas[i]);
        }
    }

    public void SetCurrentGameLevelById(string id) {
        if (_gameLevelDatasById.ContainsKey(id)) {
            CurrentGameLevelData = _gameLevelDatasById[id];
        }
    }
}
