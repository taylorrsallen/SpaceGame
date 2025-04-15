using NPCs.Base;
using UnityEngine;

namespace NPCs.AI.Base 
{
    public abstract class AiType : MonoBehaviour
    {
        protected NPC _npcRoot;
        public void Initialize(NPC root)
        {
            _npcRoot = root;
        
            OnInit();
        }
        protected abstract void OnInit();
        protected Vector3 GetPlayerPosition(float offsetX, float offsetY)
        {
            

            return GameManager.instance.ship_controller.get_ship_position() + new Vector3(offsetX, offsetY, 0);
        }
        protected Vector3 GetPlayerPosition()
        {
            return GameManager.instance.ship_controller.get_ship_position();
        }

        protected Vector3 GetDirectionOfTarget(Vector3 positionOfTarget)
        {
            Vector3 dir = (positionOfTarget - transform.position).normalized;

            return dir;
        }
        protected float GetDistanceToPlayer()
        {
            float distance = Vector3.Distance(transform.position, GetPlayerPosition());
            return distance;
        }
        protected float GetDistanceToPlayer(float offsetX, float offsetY)
        {
            float distance = Vector3.Distance(transform.position, GetPlayerPosition() + new Vector3(offsetX, offsetY, 0));
            return distance;
        }
        private int _KillDistanceCheck = 6;
        protected void CheckForKillDistance()
        {
            _KillDistanceCheck--;
            if (_KillDistanceCheck <= 0)
            {
                _KillDistanceCheck = 8;
                if (GetDistanceToPlayer() >= 400)
                    Destroy(gameObject);
            } 
        }        
    }
}
