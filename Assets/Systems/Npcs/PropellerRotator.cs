using UnityEngine;

public class PropellerRotator : MonoBehaviour
{
    [SerializeField] private float Speed = 2000;
    void Update()
    {
        transform.Rotate(0, 0, Speed * Time.deltaTime);
    }
}
