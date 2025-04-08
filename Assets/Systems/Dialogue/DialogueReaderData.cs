using UnityEngine;

[CreateAssetMenu(fileName = "DialogueReaderData", menuName = "Scriptable Objects/DialogueReaderData")]
public class DialogueReaderData : ScriptableObject {
    [SerializeField] public float speed = 1f;
    [SerializeField] public float pitch = 1f;
    [SerializeField] public Color text_color = Color.black;
}
