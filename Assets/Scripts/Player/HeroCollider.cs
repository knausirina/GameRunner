using System.Collections;
using System.Collections.Generic;
using Player.Events;
using UniRx;
using UnityEngine;

namespace Player
{
    public class HeroCollider : MonoBehaviour
    {
        private const int OBSTACLE_lAYER = 8;
        private const int COINTS_LAYER = 9;

        protected void OnTriggerEnter(Collider colliderInteraction)
        {
            if (colliderInteraction.gameObject.layer == COINTS_LAYER)
            {
                MessageBroker.Default.Publish(new CoinEvent(colliderInteraction.gameObject.transform));
            }
            else if (colliderInteraction.gameObject.layer == OBSTACLE_lAYER)
            {
                Obstacle obstacle = colliderInteraction.gameObject.GetComponent<Obstacle>();
                
                if (obstacle != null)
                {
                    obstacle.PlayAnimation();
                }
                MessageBroker.Default.Publish(new ObstacleEvent());
            }
        }
    }
}
