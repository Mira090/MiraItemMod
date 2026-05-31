using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiraItemMod
{
    public class ModSingletonBehavior : MonoBehaviour
    {
        public bool F1 { get; private set; }
        public bool F2 { get; private set; }
        public bool F3 { get; private set; }
        private void LateUpdate()
        {
            if (Keyboard.current.f2Key.isPressed)
            {
                if (F2)
                    return;
                var sprites = Sprite.FindObjectsByType<SpriteFx>(FindObjectsSortMode.None);
                Core.Logger("Count: " + sprites.Length);
                GameLogWriter.Instance.WriteLog("Count: " + sprites.Length, Color.white);
                foreach (var sprite in sprites)
                {
                    Core.Logger($"name: {sprite.name}");
                    GameLogWriter.Instance.WriteLog($"name: {sprite.name}", Color.white);
                }
                F2 = true;
            }
            else
            {
                F2 = false;
            }
            if (Keyboard.current.f3Key.isPressed)
            {
                if (F3)
                    return;
                var melees = Sprite.FindObjectsByType<MeleeCollision>(FindObjectsSortMode.None);
                Core.Logger("Count: " + melees.Length);
                GameLogWriter.Instance.WriteLog("Count: " + melees.Length, Color.white);
                foreach (var melee in melees)
                {
                    Core.Logger($"name: {melee.name}");
                    GameLogWriter.Instance.WriteLog($"name: {melee.name}", Color.white);
                }
                F3 = true;
            }
            else
            {
                F3 = false;
            }
            if (Keyboard.current.f1Key.isPressed)
            {
                if (F1)
                    return;
                var debuffs = ReflectionExtensions.GetDebuffEntities();
                Core.Logger("Count: " + debuffs.Count);
                GameLogWriter.Instance.WriteLog("Count: " + debuffs.Count, Color.white);
                foreach (var debuff in debuffs)
                {
                    Core.Logger($"key: {debuff.Key}");
                    GameLogWriter.Instance.WriteLog($"key: {debuff.Key}", Color.white);
                    Core.Logger($"name: {debuff.Value.name}");
                    GameLogWriter.Instance.WriteLog($"name: {debuff.Value.name}", Color.white);
                }
                F1 = true;
            }
            else
            {
                F1 = false;
            }
        }
    }
}
