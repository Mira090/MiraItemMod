using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Utilities
{
    public class LogComponent : MonoBehaviour
    {
        public static bool Log = false;
        private void Awake()
        {
            if (Log)
                Core.LoggerFew("Awake: " + name);
        }
        private void Start()
        {
            //Core.LoggerMany("Start: " + name);
        }
        private void OnDestroy()
        {
            if (Log)
                Core.LoggerFew("OnDestroy: " + name);
            //Core.LoggerMany($"[OnDestroy] {gameObject.name} destroyed!\n{Environment.StackTrace}");
        }
    }
}
