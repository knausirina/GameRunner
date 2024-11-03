using System.Collections;
using System.Collections.Generic;
using Player.Events;
using UniRx;
using UnityEngine;

namespace Player
{
    public class PlayerCollider : MonoBehaviour
    {
        protected void OnTriggerEnter(Collider colliderInteraction)
        {
            var layer = colliderInteraction.gameObject.layer;

            if (layer == Layers.Coints_layer)
            {
                MessageBroker.Default.Publish(new CoinEvent(colliderInteraction.gameObject.transform));
            }
            else if (layer == Layers.Obstractle_layer)
            {
                var obstacle = colliderInteraction.gameObject.GetComponent<Obstacle>();
                
                if (obstacle != null)
                {
                    obstacle.PlayAnimation();
                }
                MessageBroker.Default.Publish(new ObstacleEvent());
            }
        }
    }
}
