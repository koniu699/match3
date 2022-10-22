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

        public void Setup(int x, int y, GridModel targetModel)
        {
            this.x = x;
            this.y = y;
            gridModel = targetModel;
            assignedElement = gridModel.Grid.GetGridObject(x, y);
            assignedElement.ElementDestroyed += OnElementDestroyed;
            ResetView();
        }

        void OnElementDestroyed()
        {
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
            var prevPosition = transform.position;
            await transform.DOMove(position, tweenSettings.TweenDuration).SetEase(tweenSettings.EaseType)
                .OnComplete(() => { transform.position = prevPosition; });
        }

        public async Task TweenHide()
        {
            var prevColor = spriteRenderer.color;
            await spriteRenderer.DOFade(0f, tweenSettings.TweenDuration);
        }
    }
}