﻿using System.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public abstract class View : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}