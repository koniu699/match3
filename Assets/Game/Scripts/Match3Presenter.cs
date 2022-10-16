using UnityEngine;

namespace Game.Scripts
{
    public class Match3Presenter : MonoBehaviour
    {
        [SerializeField] GridModel gridModel;
        [SerializeField] Transform boardOrigin;
        [SerializeField] float gridElementSize = 2f;
        [SerializeField] GameObject prefab;

        public void Awake()
        {
            gridModel.GridCreated += OnGridUpdated;
        }

        void OnGridUpdated()
        {
            Debug.Log("Draw grid triggered");
            boardOrigin.DestroyChildren();
            for (var i = 0; i < gridModel.Grid.GridReference.GetLength(0); i++)
            {
                for (var j = 0; j < gridModel.Grid.GridReference.GetLength(1); j++)
                {
                    var position = new Vector2(i * gridElementSize, j * gridElementSize);
                    var instance = Instantiate(prefab, boardOrigin);
                    instance.transform.localPosition = position;
                    var match3Controller = instance.GetComponent<Match3ElementController>();
                    if (match3Controller == null)
                        continue;
                    match3Controller.Setup(i, j, gridModel);
                }
            }
        }
    }
}