using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Utilities
{
    public class SimpleRotateAnimation : MonoBehaviour
    {
        public float speed = 90f;
        public RectTransform RectTransform { get; private set; }
        private void Awake()
        {
            RectTransform = transform as RectTransform;
        }
        void Update()
        {
            RectTransform.localRotation *= Quaternion.Euler(0, 0, speed * Time.deltaTime);
        }
    }
}
