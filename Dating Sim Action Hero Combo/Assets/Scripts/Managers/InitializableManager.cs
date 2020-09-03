using System;

// these are manager classes that can be created/destroyed as the game requires them
public interface IInitializableManager
{
    void Initialize(Action<bool> initializationCallback = null);
    void Dispose();
}