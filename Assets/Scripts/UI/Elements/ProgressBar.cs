using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectMM.UI.Elements
{
    public class ProgressBar : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Image _FillImage;
        [SerializeField] private TextMeshProUGUI _ProgressText;

        #endregion
        
        public void SetFillAmount(float amount)
        {
            _FillImage.fillAmount = amount;
            _ProgressText.SetText($"{amount * 100:0}%");
        }

        public void SetActiveProgressText(bool active)
        {
            _ProgressText.gameObject.SetActive(active);
        }
    }
}