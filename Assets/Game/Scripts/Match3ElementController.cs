using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.ScriptableEvents;
using Game.Scripts.ScriptableEvents.Events;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts
{
    public class Match3ElementController : MonoBehaviour
    {
        [SerializeField] TweenSettings tweenSettings;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] GridValueUpdatedEvent gridValueUpdatedEvent;
        [SerializeField] GridElementClickedEvent gridElementClickedEvent;
        [SerializeField] GameObject borderObject;
        [SerializeField] GridModel gridModel;
        int x;
        int y;

        bool isBusy = false;

        float gridElementSize;

        bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                ResetView();
            }
        }

        public UnityAction<int, int> ElementDestroyed;

        Vector2 dragBeginPos;
        GridElement assignedElement;

        void OnMouseDown()
        {
            gridElementClickedEvent.Raise(gridModel.Grid.GetGridObject(x, y));
        }

        void Awake()
        {
            gridElementClickedEvent.AddListener(OnGridElementClicked);
            gridValueUpdatedEvent.AddListener(OnGridValueUpdated);
        }

        void OnGridValueUpdated(GridValueUpdatedArgs updateArgs)
        {
            if (!isBusy)
                ResetView();
        }

        void OnGridElementClicked(GridElement clickedElement)
        {
            if (clickedElement != assignedElement)
            {
                ResetView();
            }
            else
                Highlight();
        }

        public void Setup(int x, int y, GridModel targetModel, float gridElementSize)
        {
            this.x = x;
            this.y = y;
            this.gridElementSize = gridElementSize;
            gridModel = targetModel;
            assignedElement = gridModel.Grid.GetGridObject(x, y);
            assignedElement.ElementDestroyed += OnElementDestroyed;
            ResetView();
        }

        void OnElementDestroyed()
        {
            Debug.Log($"Element Destroyed {x}/{y}");
            ElementDestroyed?.Invoke(x, y);
            Destroy(gameObject);
        }

        void ResetView()
        {
            if (assignedElement != null)
                spriteRenderer.sprite = assignedElement.Match3Element.Sprite;
            borderObject.SetActive(false);
        }

        void Highlight()
        {
            borderObject.SetActive(true);
        }

        void OnDestroy()
        {
            assignedElement.ElementDestroyed -= OnElementDestroyed;
            gridElementClickedEvent.RemoveListener(OnGridElementClicked);
            gridValueUpdatedEvent.RemoveListener(OnGridValueUpdated);
        }

        public async Task TweenTo(Vector3 position)
        {
            IsBusy = true;
            var prevPosition = transform.position;
            await transform.DOMove(position, tweenSettings.TweenDuration).SetEase(tweenSettings.EaseType)
                .OnComplete(() =>
                {
                    transform.position = prevPosition;
                    IsBusy = false;
                });
        }

        public async Task TweenHide()
        {
            IsBusy = true;
            await spriteRenderer.DOFade(0f, tweenSettings.TweenDuration).OnComplete(() => { IsBusy = false; });
        }

        public void Drop(int destinationY)
        {
            IsBusy = true;
            transform.DOLocalMoveY(destinationY * gridElementSize, tweenSettings.TweenDuration)
                .SetEase(tweenSettings.EaseType).OnComplete(
                    () =>
                    {
                        IsBusy = false;
                    });
        }

        public async Task ShowSpawn()
        {
            isBusy = true;
            var currTransform = transform;
            var currentPos = currTransform.localPosition;
            currTransform.localPosition = new Vector3(currentPos.x, currentPos.y + gridModel.Grid.GridReference.GetLength(1));
            await currTransform.DOLocalMoveY(currentPos.y, tweenSettings.TweenDuration).SetEase(tweenSettings.EaseType)
                .OnComplete(
                    () => { IsBusy = false; });
        }

        public void Show()
        {
            transform.localPosition = new Vector3(x * gridElementSize, y * gridElementSize);
            spriteRenderer.DOFade(1f, 0);
        }
    }
}