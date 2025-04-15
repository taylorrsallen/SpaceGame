using NPCs.Base;
using UnityEngine;

namespace NPCs.AI.Base 
{
    public abstract class AiType : MonoBehaviour
    {
        protected NPC _npcRoot;
        private Transform _playerTransform;
        protected Rigidbody _playerRigidBody;
        public void Initialize(NPC root)
        {
            _npcRoot = root;

            _playerTransform = GameObject.Find("RocketController").transform;

            _playerRigidBody = _playerTransform.GetComponent<Rigidbody>();
            

            OnInit();
        }
        protected abstract void OnInit();

        protected Vector3 GetPlayerPosition(float offsetX, float offsetY)
        {
            return _playerTransform.position + _playerRigidBody.centerOfMass + new Vector3(offsetX, offsetY, 0);
        }
        protected Vector3 GetPlayerPosition()
        {
            return _playerTransform.position + _playerRigidBody.centerOfMass;
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
