using NPCs.AI.Base;
using UnityEngine;




namespace NPCs.Base
{
    public class NPC : MonoBehaviour, IDamageable
    {
        private float _hp = 100;
        private AiType _aiType;

        [SerializeField] protected float maxHp = 100;

        public void Awake()
        {
            InitializeAI();
        }


        private void InitializeAI()
        {
            _hp = maxHp;


            _aiType = GetComponent<AiType>();
            _aiType.Initialize(this);
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

        public void damage(DamageArgs args)
        {
            _hp -= args.damage;
            OnHit();
            if (_hp < 0)
            {
                OnDeath();
                Destroy(gameObject);
            }
        }
    }
}

