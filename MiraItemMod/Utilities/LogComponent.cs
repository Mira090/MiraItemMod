using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Utilities
{
    public class LogComponent : MonoBehaviour
    {
        private void Awake()
        {
            if (Core.LogFew)
                Core.Logger("Awake: " + name);
        }
        private void Start()
        {
            //if (Core.LogMany)
                //Core.Logger("Start: " + name);
        }
        private void OnDestroy()
        {
            if (Core.LogFew)
                Core.Logger("OnDestroy: " + name);
            //if (Core.LogMany)
                //Core.Logger($"[OnDestroy] {gameObject.name} destroyed!\n{Environment.StackTrace}");
        }
    }
}
