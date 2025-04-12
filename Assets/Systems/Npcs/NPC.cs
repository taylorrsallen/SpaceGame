using NPCs.AI.Base;
using UnityEngine;

public interface IDamage
{
    public void TakeDamage(float damage);
}


namespace NPCs.Base
{
    public abstract class NPC : MonoBehaviour, IDamage
    {
        private float _hp = 100;
        private AiType _aiType;

        protected float maxHp = 100;

        public void Awake()
        {
            SetDefaults();
            InitializeAI();
        }

        protected abstract void SetDefaults();

        private void InitializeAI()
        {
            _aiType = GetComponent<AiType>();
        }
        public void TakeDamage(float damage)
        {
            _hp -= damage;
            OnHit();
            if (_hp < 0)
            {
                OnDeath();
                Destroy(gameObject);
            }
        }
        protected virtual void OnHit()
        {
        }
        protected virtual void OnDeath()
        {
        }
    }
}

