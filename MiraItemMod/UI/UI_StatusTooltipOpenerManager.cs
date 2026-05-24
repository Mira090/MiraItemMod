using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.UI
{
    public class UI_StatusTooltipOpenerManager : MonoBehaviour
    {
        public List<UI_StatusTooltipOpener> Stats = new List<UI_StatusTooltipOpener>();
        public UI_StatsCategory Special;
        public UI_LocalizationStringText Name;
        public void Init()
        {
            Stats.Clear();
            for (int q = 0; q < transform.childCount; q++)
            {
                var child = transform.GetChild(q);
                if (child != null && child.TryGetComponent<UI_StatusTooltipOpener>(out var stat))
                {
                    Stats.Add(stat);
                }
            }
            if(gameObject.TryGetComponent<UI_StatsCategory>(out var category))
            {
                Special = category;
                if (transform.childCount > 0 && transform.GetChild(0).TryGetComponent<UI_LocalizationStringText>(out var name))
                    Name = name;
            }
        }
        public void SetStats(params string[] stats)
        {
            foreach (var stat in Stats)
            {
                stat.gameObject.SetActive(false);
            }

            int count = 0;
            foreach(var id in stats)
            {
                var entity = StatusDatabase.GetStatusEntity(id);
                if (entity == null)
                    continue;
                if (Stats.Count <= count)
                {
                    var @new = Instantiate(Stats[0], transform);
                    Stats.Add(@new);
                }

                var opener = Stats[count];
                opener.gameObject.SetActive(true);
                opener.SetStatus(entity);

                count++;
            }
        }
        public void SetStats(LocalizedString name, params string[] stats)
        {
            if(Name != null)
            {
                Name.UpdateKey(name.key);
            }
            SetStats(stats);
        }
    }
}
