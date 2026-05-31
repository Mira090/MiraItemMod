using MiraItemMod.Utilities;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Log("Count: " + sprites.Length);
                foreach (var sprite in sprites)
                {
                    Log($"name: {sprite.name}");
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
                Log("Count: " + melees.Length);
                foreach (var melee in melees)
                {
                    Log($"name: {melee.name}");
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
                LogCurrentWeapon();
                F1 = true;
            }
            else
            {
                F1 = false;
            }
        }
        private void LogAllDebuff()
        {
            var debuffs = ReflectionExtensions.GetDebuffEntities();
            Log("Count: " + debuffs.Count);
            foreach (var debuff in debuffs)
            {
                Log($"key: {debuff.Key}");
                Log($"name: {debuff.Value.name}");
            }
        }
        private void LogCurrentWeapon()
        {
            try
            {
                if (NetworkClient.localPlayer.gameObject.TryGetComponent<PlayerAvatar>(out var player))
                {
                    var cont = player.GetWeaponController();
                    var weapon = WeaponDatabase.FindWeaponById(cont.currentWeapon.NetworkentityId);
                    Log(weapon.ToAllString());
                    Log(cont.currentWeapon.basicComboAttacks.FirstOrDefault().ToAllString());
                }
            }
            catch (Exception ex)
            {
                Core.LoggerError(ex);
            }
        }
        private void Log(string text)
        {
            Core.Logger(text);
            GameLogWriter.Instance.WriteLog(text, Color.white);
        }
    }
}
