using UnityEngine;

[CreateAssetMenu(fileName = "ContentDatabase", menuName = "Scriptable Objects/ContentDatabase")]
public class ContentDatabase : ScriptableObject {
    [SerializeField] public MatterData[] matter_database;
}
