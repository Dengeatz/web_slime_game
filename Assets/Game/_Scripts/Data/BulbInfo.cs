using System;
using System.Collections.Generic;
using TestTask.Core;
using UnityEngine;

namespace TestTask.Info
{
    [Serializable]
    public class BulbInfo
    {
        [SerializeField] public Bulb BulbHandler;
        [SerializeField] public List<Slime> StartSlimeObjects;
    }
}