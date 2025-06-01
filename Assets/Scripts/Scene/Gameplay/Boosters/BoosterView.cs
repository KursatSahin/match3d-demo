using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Match3d.Gameplay.Boosters
{
    public class BoosterView : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Button _boosterButton;
        [SerializeField] private Image _boosterIcon;

        #endregion

        private IBooster _booster;

        private void OnEnable()
        {
            _boosterButton.onClick.AddListener(OnBoosterButtonClicked);
        }

        private void OnDisable()
        {
            _boosterButton.onClick.RemoveListener(OnBoosterButtonClicked);
        }

        public void Initialize(IBooster booster, Sprite icon)
        {
            _boosterIcon.sprite = icon;
            _booster = booster;
        }

        private void OnBoosterButtonClicked()
        {
            _booster?.Execute(this.GetCancellationTokenOnDestroy()).Forget();
        }
    }
}