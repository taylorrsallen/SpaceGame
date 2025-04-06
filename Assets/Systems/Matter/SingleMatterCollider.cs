using UnityEngine;
using UnityEngine.UIElements;

public class SingleMatterCollider : MonoBehaviour, IMatterCollider {
    [SerializeField] public byte matter_id;
    
    public byte get_matter_id(Vector3 position) { return matter_id; }
}
