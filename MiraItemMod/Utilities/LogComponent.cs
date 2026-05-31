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
            Core.LoggerFew("Awake: " + name);
        }
        private void Start()
        {
            //Core.LoggerMany("Start: " + name);
        }
        private void OnDestroy()
        {
            Core.LoggerFew("OnDestroy: " + name);
            //Core.LoggerMany($"[OnDestroy] {gameObject.name} destroyed!\n{Environment.StackTrace}");
        }
    }
}
