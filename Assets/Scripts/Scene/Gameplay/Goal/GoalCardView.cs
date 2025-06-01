using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Match3d.Scene.Goal
{
    public class GoalCardView : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Image _goalItemImage;
        [SerializeField] private TextMeshProUGUI _goalItemAmount;

        #endregion

        public void Initialize(int amount, Sprite sprite)
        {
            _goalItemImage.sprite = sprite;
            _goalItemAmount.SetText(amount.ToString());
        }

        public void SetOrderCount(int amount)
        {
            _goalItemAmount.SetText(amount.ToString());

            if (amount == 0)
            {
                _goalItemAmount.gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
    }
}