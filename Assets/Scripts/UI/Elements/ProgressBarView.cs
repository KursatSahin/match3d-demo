using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3d.Scene
{
    public class ProgressBarView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        public void SetFillAmount(float amount)
        {
            _image.fillAmount = amount;
            _text.SetText($"{amount * 100:0}%");
        }
    }
}