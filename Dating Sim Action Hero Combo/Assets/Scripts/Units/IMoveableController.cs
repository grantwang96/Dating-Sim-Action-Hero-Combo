using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveableUnitController : IUnitController
{
    float Speed { get; }
}
