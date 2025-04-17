using UnityEngine;

public interface IUI3D {
    public void set_hovered(Vector3 hit_position);
    public void set_unhovered();
    public void press();
    public void unpress();
    public void release();
}

public class Menu3D : MonoBehaviour {
    public Vector3 local_offset;
}
