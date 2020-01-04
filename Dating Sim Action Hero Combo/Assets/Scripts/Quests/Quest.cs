using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface Quest
{
    event Action OnCompleted;
    event Action OnFailed;
}
