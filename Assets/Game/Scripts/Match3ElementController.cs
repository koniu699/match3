using Game.Scripts.ScriptableEvents;
using Game.Scripts.ScriptableEvents.Events;
using UnityEngine;

namespace Game.Scripts
{
    public class Match3ElementController : MonoBehaviour
    {
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
    }
}