using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Match3d.Gameplay.Item;

namespace Match3d.Gameplay.Item
{
    public class ItemBase : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;
        [SerializeField] private Rigidbody _rigidbody;

        #endregion

        public ItemData.ItemType Type { get; private set; }

        public bool IsColliderEnabled => _meshCollider.enabled;

        void OnDisable()
        {
            _meshCollider.enabled = true;
            _rigidbody.isKinematic = false;
            transform.position = Vector3.zero; 
            transform.rotation = Quaternion.identity;
            transform.localScale = new Vector3(3.33f, 3.33f, 3.33f);
        }

        public void SetItemData(ItemData data)
        {
            _meshFilter.sharedMesh = data.Filter;
            _meshCollider.sharedMesh = data.Collider;
            Type = data.Type;
        }

        public void JumpToSlot(Vector3 position, Action onCompleted = null)
        {
            const float duration = 0.6f;
            const float halfDuration = duration / 2;
            
            /*
            await transform.DOScale(transform.localScale * 1.4f, .2f).ToUniTask();
            var movements = new List<UniTask>();
            movements.Add(transform.DOScale(2, .4f).ToUniTask());
            movements.Add(transform.DORotate(new Vector3(80,359,0), .4f).ToUniTask());
            movements.Add(transform.DOMove(position + Vector3.up, .4f).SetEase(Ease.OutBack).ToUniTask());
            await UniTask.WhenAll(movements);
            onCompleted?.Invoke();
            */
            var sequence = DOTween.Sequence().Pause().SetLink(gameObject).SetId(transform);
            
            sequence.Insert(0, transform.DOScale(transform.localScale * 1.4f, .2f));
            sequence.Insert(.2f, transform.DOScale(2, .4f));
            sequence.Insert(.2f, transform.DORotate(new Vector3(80,359,0), .4f));
            sequence.Insert(.2f, transform.DOMove(position + Vector3.up, .4f).SetEase(Ease.OutBack));
            sequence.OnComplete(() =>
            {
                _meshCollider.enabled = false;
                _rigidbody.isKinematic = true;
                onCompleted?.Invoke();
            });
            sequence.Play();
        }
        
        public void ShiftNextSlot(Vector3 position)
        {
            DOTween.Kill(transform, true);

            const float duration = 0.15f;
            var halfDistance = (position.x - transform.position.x) / 2;

            var sequence = DOTween.Sequence().Pause().SetLink(gameObject).SetId(transform);
            sequence.Join(transform.DOMove(new Vector3 (transform.position.x + halfDistance, transform.position.y, transform.position.z + halfDistance), duration).SetEase(Ease.OutQuad));
            sequence.Append(transform.DOMove(new Vector3 (transform.position.x + halfDistance * 2, transform.position.y, transform.position.z), duration).SetEase(Ease.OutQuad));
            sequence.Play();
        }

        public void ShiftPreviousSlot(Vector3 position, int count)
        {
            DOTween.Kill(transform, true);

            const float duration = 0.075f;

            var halfDistance = (transform.position.x - (position.x)) / (2 * count);
            var positionX = transform.position.x;
            var seq = DOTween.Sequence();
            for (var i = 0; i < count; i++)
            {
                seq.Append(transform.DOMove(new Vector3 (positionX - halfDistance, transform.position.y, transform.position.z + halfDistance), duration).SetEase(Ease.OutQuad));
                seq.Append(transform.DOMove(new Vector3 (positionX - halfDistance * 2, transform.position.y, transform.position.z), duration).SetEase(Ease.OutQuad));
                positionX = transform.position.x - (halfDistance * 2) * (i + 1);
            }
            seq.Play();
        }
        
        public void MoveToMergePoint(Vector3 position, Action onCompleted = null)
        {
            DOTween.Kill(transform, true);

            const float duration = 0.4f;

            var seq = DOTween.Sequence();
            seq.Join(transform.DOMoveX(position.x, duration).SetEase(Ease.InCubic));
            seq.Join(transform.DOMoveZ(position.z, duration).SetEase(Ease.OutCubic));
            seq.OnComplete(() => onCompleted?.Invoke());
        }
    }

}