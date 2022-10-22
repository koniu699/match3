using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.ScriptableEvents;
using Game.Scripts.ScriptableEvents.Events;
using UnityEngine;

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
            assignedElement.ElementDestroyed -= OnElementDestroyed;
            Destroy(this);
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
            transform.DOLocalMoveY(destinationY * gridElementSize, tweenSettings.TweenDuration).SetEase(tweenSettings.EaseType).OnComplete(
                () => { IsBusy = false; });
        }

        public async Task ShowSpawn()
        {
            isBusy = true;
            var currTransform = transform;
            var currentPos = currTransform.position;
            currTransform.position = new Vector3(currentPos.x, currentPos.y * 2);
            await currTransform.DOLocalMoveY(currentPos.y, tweenSettings.TweenDuration).SetEase(tweenSettings.EaseType)
                .OnComplete(
                    () => { IsBusy = false; });
        }
    }
}