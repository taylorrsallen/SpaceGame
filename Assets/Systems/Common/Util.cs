using UnityEngine;

static public class Util {
    public static Quaternion multiply_quat_by_scalar(Quaternion quat, float scalar) {
        Quaternion return_quat = quat;
        return_quat.x *= scalar;
        return_quat.y *= scalar;
        return_quat.z *= scalar;
        return_quat.w *= scalar;
        return return_quat;
    }

    public static Quaternion shortest_rotation(Quaternion to, Quaternion from) {
        if (Quaternion.Dot(to, from) < 0f) {
            return to * Quaternion.Inverse(multiply_quat_by_scalar(from, -1f));
        } else {
            return to * Quaternion.Inverse(from);
        }
    }

    public static void DrawAABB2D(Vector3 origin, Vector2 size, Color color) {
        DrawBox2D(origin, origin + Vector3.right * size.x, origin + Vector3.up * size.y, origin + Vector3.right * size.x + Vector3.up * size.y, color);
    }

    // c d
    // a b
    public static void DrawBox2D(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color) {
        Debug.DrawLine(a, c, color);
        Debug.DrawLine(a, b, color);
        Debug.DrawLine(c, d, color);
        Debug.DrawLine(b, d, color);
    }
}
