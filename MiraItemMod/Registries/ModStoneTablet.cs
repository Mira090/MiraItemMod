using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModStoneTablet : ModItem
    {
        public static ModStoneTablet Create(string name, string query, bool isRotatable)
            => new ModStoneTablet().SetStoneTablet(name, query, isRotatable);
        public static ModStoneTablet Create<T>(string name, string query, bool isRotatable) where T : StoneTablet
            => new ModStoneTablet().SetStoneTablet<T>(name, query, isRotatable);
        internal ModStoneTablet SetStoneTablet(string name, string query, bool isRotatable)
            => SetStoneTablet<StoneTablet>(name, query, isRotatable);
        internal ModStoneTablet SetStoneTablet<T>(string name, string query, bool isRotatable) where T : StoneTablet
        {
            SetItem(name, EItemType.StoneTablet);
            Query = query;
            StoneTabletType = typeof(T);
            IsRotatable = isRotatable;

            return this;
        }
        public string Query { get; internal set; }
        public string ConditionQuery { get; internal set; } = "";
        public bool IsRotatable { get; internal set; }
        public Type StoneTabletType { get; internal set; }
        public override GameObject CreateResourcePrefab()
        {
            var o = new GameObject(ResourcePrefabName);
            //o.gameObject.SetActive(false);
            var tablet = o.AddComponent(StoneTabletType) as StoneTablet;
            tablet.query = Query;
            tablet.conditionQuery = ConditionQuery;
            tablet.isRotatable = IsRotatable;
            //Core.Logger($"CreateStoneTablet");
            o.AddComponent<LogComponent>();
            o.hideFlags = HideFlags.HideAndDontSave;
            tablet.enabled = false;
            return o;
        }
    }
}
