using RIAR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIAR
{
    public interface ICharacterCollision 
    {
        List<ColliderFeedback> CastRayDown();
        List<ColliderFeedback> CastRayUp();
        List<ColliderFeedback> CastRayRight();
        List<ColliderFeedback> CastRayLeft();
    }

    public class ColliderFeedback
    {
        public float Distance;
        public RaycastHit2D Hit;

        public ColliderFeedback() { }

        public ColliderFeedback(float distance, RaycastHit2D hit)
        {
            Distance = distance;
            Hit = hit;
        }
    }

    public class CharacterCollision : MonoBehaviour, ICharacterCollision
    {
        //create rays in all 4 directions
        //check if any of ray over lap with any of the collider

        [SerializeField] private BoxCollider2D _collider;
        [SerializeField] private LayerMask _interactableLayer;

        private float _rayLength = 2f;
        private int _rayCount = 5;
        private float _skinWidth = 0.05f;

        public float SkinWidth { get => _skinWidth; }

        public List<ColliderFeedback> CastRayDown()
        {
            var colFeedbacks = new List<ColliderFeedback>();

            Bounds colBounds = _collider.bounds;

            //cast down, start from bottom left
            for (int i = 0; i < _rayCount; i++)
            {
                float incr = (colBounds.size.x / (_rayCount - 1)) * i;
                var bottomLeft = new Vector2(colBounds.min.x + incr, colBounds.min.y - SkinWidth);
                RaycastHit2D hit = Physics2D.Raycast(bottomLeft, Vector2.down, _rayLength);
                UnityEngine.Debug.DrawLine(bottomLeft, bottomLeft + Vector2.down * _rayLength, Color.green);

                if (hit)
                    colFeedbacks.Add(new ColliderFeedback(hit.distance, hit));
            }

            return colFeedbacks;
        }

        public List<ColliderFeedback> CastRayUp()
        {
            var colFeedbacks = new List<ColliderFeedback>();

            Bounds colBounds = _collider.bounds;

            //cast up, start from top left
            for (int i = 0; i < _rayCount; i++)
            {
                float incr = (colBounds.size.x / (_rayCount - 1)) * i;
                var topLeft = new Vector2(colBounds.min.x + incr, colBounds.max.y + SkinWidth);
                RaycastHit2D hit = Physics2D.Raycast(topLeft, Vector2.up, _rayLength);
                UnityEngine.Debug.DrawLine(topLeft, topLeft + Vector2.up * _rayLength, Color.green);

                if (hit)
                    colFeedbacks.Add(new ColliderFeedback(hit.distance, hit));
            }

            return colFeedbacks;
        }

        public List<ColliderFeedback> CastRayRight()
        {
            var colFeedbacks = new List<ColliderFeedback>();

            Bounds colBounds = _collider.bounds;

            for (int i = 0; i < _rayCount; i++)
            {
                float incr = (colBounds.size.y / (_rayCount - 1)) * i;
                var topLeft = new Vector2(colBounds.max.x + SkinWidth, colBounds.max.y - incr);
                RaycastHit2D hit = Physics2D.Raycast(topLeft, Vector2.right, _rayLength, _interactableLayer);
                UnityEngine.Debug.DrawLine(topLeft, topLeft + Vector2.right * _rayLength, Color.green);

                if (hit)
                    colFeedbacks.Add(new ColliderFeedback(hit.distance, hit));
            }

            return colFeedbacks;
        }

        public List<ColliderFeedback> CastRayLeft()
        {
            var colFeedbacks = new List<ColliderFeedback>();

            Bounds colBounds = _collider.bounds;

            //cast left, start from top left
            for (int i = 0; i < _rayCount; i++)
            {
                float incr = (colBounds.size.y / (_rayCount - 1)) * i;
                var topLeft = new Vector2(colBounds.min.x - SkinWidth, colBounds.max.y - incr);
                RaycastHit2D hit = Physics2D.Raycast(topLeft, Vector2.left, _rayLength, _interactableLayer);
                UnityEngine.Debug.DrawLine(topLeft, topLeft + Vector2.left * _rayLength, Color.green);

                if (hit)
                    colFeedbacks.Add(new ColliderFeedback(hit.distance, hit));
            }

            return colFeedbacks;
        }
    }

}

