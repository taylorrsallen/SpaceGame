using Sirenix.OdinInspector;
using UnityEngine;

public enum UIState {
    MENU,
    STARTING_CAREER,
    HUD_MENU,
    HUD,
}

public class UIManager : MonoBehaviour {
    public static UIManager instance { get; private set; }

    private IntrosMenu intros_menu;
    [TabGroup("Setup"), SerializeField] private Menu3D main_menu;
    [TabGroup("Setup"), SerializeField] private Menu3D pause_menu;
    private ConsoleMenu console_menu;
    private HUD hud;

    [TabGroup("Setup"), SerializeField] private Transform ui_3d;
    [TabGroup("Setup"), SerializeField] private Camera scene_camera;
    [TabGroup("Setup"), SerializeField] private Transform camera_intro_transform;
    [TabGroup("Setup"), SerializeField] private Transform camera_main_menu_transform;
    private Transform camera_transform_target;
    [TabGroup("Setup"), SerializeField] private float camera_max_lerp_speed = 5f;
    [TabGroup("Setup"), SerializeField] private float camera_lerp_speed_per_second = 1f;
    private float camera_lerp_speed = 0f;
    [TabGroup("Setup"), SerializeField] private bool skip_intro = false;
    [TabGroup("Setup"), SerializeField] private LayerMask ui_layer;

    [TabGroup("Cursor"), SerializeField] private Light cursor_light;
    [TabGroup("Cursor")] public float cursor_light_intensity;
    [TabGroup("Cursor")] public float cursor_light_normal_offset;
    [TabGroup("Cursor")] public float cursor_light_default_distance;
    [TabGroup("Cursor")] public Color cursor_color;

    [TabGroup("Prefabs")] public ContextMenu3D context_menu_prefab;

    private IUI3D hovered_ui_3d;
    private IUI3D pressed_ui_3d;
    private UIState state;

    #region Init
    private void Awake() {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
        DontDestroyOnLoad(this);

        intros_menu = FindAnyObjectByType<IntrosMenu>(FindObjectsInactive.Include);
        console_menu = FindAnyObjectByType<ConsoleMenu>(FindObjectsInactive.Include);
        hud = FindAnyObjectByType<HUD>(FindObjectsInactive.Include);
        hud.init();

        intros_menu.init();
    }

    private void OnEnable() {
        intros_menu.on_finished += show_main_menu;
    }

    private void OnDisable() {
        intros_menu.on_finished -= show_main_menu;
    }

    private void Start() {
        scene_camera.transform.position = camera_intro_transform.position;
        scene_camera.transform.rotation = camera_intro_transform.rotation;
        intros_menu.play();
    }
    #endregion

    private void Update() {
        if(!skip_intro && !intros_menu.is_finished()) return;

        if(main_menu.gameObject.activeSelf) camera_transform_target = camera_main_menu_transform;
        if(state == UIState.STARTING_CAREER) try_start_career_mode();

        update_scene_camera();
        update_ui_3d();

        if(Input.GetMouseButtonDown(0)) primary_pressed();
        if(Input.GetMouseButtonUp(0)) primary_released();

        if(!Input.GetMouseButton(0)) {
            if(pressed_ui_3d != null) {
                pressed_ui_3d.unpress();
                pressed_ui_3d = null;
            }
        }
    }

    #region Input
    public void primary_pressed() {
        if(hovered_ui_3d != null) {
            hovered_ui_3d.press();
            pressed_ui_3d = hovered_ui_3d;
        }
    }

    public void primary_held() {
        
    }

    public void primary_released() {
        if(hovered_ui_3d != null && hovered_ui_3d == pressed_ui_3d) {
            pressed_ui_3d.release();
            pressed_ui_3d = null;
        } else if (pressed_ui_3d != null) {
            pressed_ui_3d.unpress();
            pressed_ui_3d = null;
        }
    }
    #endregion

    #region UI 3D Interaction
    private void update_scene_camera() {
        camera_lerp_speed = Mathf.Min(camera_lerp_speed + Time.unscaledDeltaTime * camera_lerp_speed_per_second, camera_max_lerp_speed);
        scene_camera.transform.position = Vector3.Lerp(scene_camera.transform.position, camera_transform_target.position, Time.deltaTime * camera_lerp_speed);
        scene_camera.transform.rotation = Quaternion.Lerp(scene_camera.transform.rotation, camera_transform_target.rotation, Time.deltaTime * camera_lerp_speed);
    }

    private void update_ui_3d() {
        foreach(Menu3D menu_3d in ui_3d.GetComponentsInChildren<Menu3D>()) {
            menu_3d.transform.rotation = scene_camera.transform.rotation;
            menu_3d.transform.position = scene_camera.transform.position + scene_camera.transform.TransformDirection(menu_3d.local_offset);
        }

        Ray ray = scene_camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 50f, ui_layer)) {
            IUI3D hit_ui_3d = hit.collider.GetComponent<IUI3D>();
            if(hit_ui_3d == null) hit_ui_3d = hit.collider.GetComponentInParent<IUI3D>();
            set_hovered_ui_3d(hit_ui_3d, hit.point);

            cursor_light.transform.position = hit.point + hit.normal * cursor_light_normal_offset;
        } else {
            set_hovered_ui_3d(null, Vector3.zero);

            cursor_light.transform.position = ray.origin + ray.direction * cursor_light_default_distance;
        }

        cursor_light.color = cursor_color * cursor_light_intensity;
    }

    private void set_hovered_ui_3d(IUI3D _hovered_ui_3d, Vector3 hit_point) {
        if(hovered_ui_3d != null) {
            if(hovered_ui_3d == _hovered_ui_3d) { hovered_ui_3d.set_hovered(hit_point); return; }
            hovered_ui_3d.set_unhovered();
        }

        hovered_ui_3d = _hovered_ui_3d;
        if(hovered_ui_3d != null) hovered_ui_3d.set_hovered(hit_point);
    }

    public void set_cursor_light_visible(bool visible) {
        cursor_light.gameObject.SetActive(visible);
    }
    #endregion

    #region State Change
    public void enter_menu_mode() {
        state = UIState.MENU;
        scene_camera.gameObject.SetActive(true);
        show_main_menu();
        set_cursor_light_visible(true);
    }

    public void enter_hud_menu_mode() {
        state = UIState.HUD_MENU;
        scene_camera.gameObject.SetActive(false);
        set_cursor_light_visible(true);
    }

    public void enter_hud_mode() {
        state = UIState.HUD;
        scene_camera.gameObject.SetActive(false);
        show_hud();
        hide_all_menus();
        set_cursor_light_visible(false);
    }

    public void enter_starting_career_mode() {
        if(state != UIState.STARTING_CAREER) {
            state = UIState.STARTING_CAREER;
            camera_transform_target = GameManager.instance.player_controller.camera_rig.get_camera().transform;
            camera_lerp_speed = 0f;
            hide_all_menus();
        }
    }

    private void try_start_career_mode() {
        if(Vector3.Distance(scene_camera.transform.position, GameManager.instance.player_controller.camera_rig.get_camera().transform.position) < 0.1f) {
            enter_hud_mode();
            GameManager.instance.start_career_mode();
        }
    }
    #endregion

    #region Show And Hide Menus
    public void escape_menu() {
        if(console_menu.gameObject.activeSelf) {
            hide_console_menu();
        } else if(pause_menu.gameObject.activeSelf) {
            hide_pause_menu();
        } else if(!main_menu.gameObject.activeSelf) {
            show_pause_menu();
        }
    }

    private void hide_all_menus() {
        main_menu.gameObject.SetActive(false);
        pause_menu.gameObject.SetActive(false);
    }

    public void show_main_menu() {
        hide_all_menus();
        main_menu.gameObject.SetActive(true);
    }

    public void hide_main_menu() {
        main_menu.gameObject.SetActive(false);
    }

    public void show_pause_menu() {
        hide_all_menus();
        pause_menu.gameObject.SetActive(true);
    }

    public void hide_pause_menu() {
        pause_menu.gameObject.SetActive(false);
    }

    public void show_hud() {
        hud.gameObject.SetActive(true);
    }

    public void hide_hud() {
        hud.gameObject.SetActive(false);
    }

    public void show_console_menu() {
        console_menu.gameObject.SetActive(true);
    }

    public void hide_console_menu() {
        console_menu.gameObject.SetActive(false);
    }
    #endregion
}
