using System;

public class QuestInfoDisplayManager : IInitializableManager {

    private const string QuestDisplayInfoPrefabId = "";

    public void Initialize(Action<bool> initializationCallback = null) {
        initializationCallback?.Invoke(true);
    }

    public void Dispose() {
        
    }

}
