using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum GameMode {
    NONE,
    CAREER,
    SANDBOX,
}

public class GameManager : MonoBehaviour {
    public static GameManager instance { get; private set; }

    [HideInInspector] public PlayerController player_controller;
    [HideInInspector] public ModularShipBuilder ship_builder;
    [HideInInspector] public ModularShipController ship_controller;
    
    public string starting_ship;

    public Vector3 spawn_point = new Vector3(0f, 0f, 0f);

    [SerializeField] public ShipComponentData[] ship_components;

    private void Awake() {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
        DontDestroyOnLoad(this);

        player_controller = FindAnyObjectByType<PlayerController>();
        ship_builder = FindAnyObjectByType<ModularShipBuilder>(FindObjectsInactive.Include);
        ship_controller = FindAnyObjectByType<ModularShipController>(FindObjectsInactive.Include);

        ship_controller.init();
        ship_builder.init();

        for (int i = 0; i < ship_components.Length; i++) ship_components[i].id = i;
    }

    public void start_career_mode() {
        player_controller.camera_rig.gameObject.SetActive(true);
        enter_play_mode_with_loaded_ship(starting_ship);
    }

    public void toggle_ship_builder() {
        if (ship_builder.gameObject.activeSelf) {
            if (!ship_builder.set_hotkey_target) enter_play_mode_from_ship_builder();
        } else {
            enter_ship_builder();
        }
    }

    public void enter_ship_builder() {
        Debug.Log("Build Mode");

        // Get blueprint from existing ship
        ModularShipBlueprintData existing_blueprint = ship_controller.blueprint;
        if (existing_blueprint == null) existing_blueprint = new ModularShipBlueprintData(new List<Grid3DItemData>());

        // Clear existing ship
        ship_controller.transform.position = spawn_point;
        ship_controller.clear();
        ship_controller.set_active(false);

        // Load existing ship blueprint into build grid
        ship_builder.load_blueprint(existing_blueprint);

        // Show ship builder
        ship_builder.gameObject.SetActive(true);

        // Swap player controls
        player_controller.character = ship_builder.ship_builder_camera_anchor.GetComponent<Character>();
        player_controller.camera_rig.anchor_transform = ship_builder.ship_builder_camera_anchor.transform;
        player_controller.camera_rig.anchor_lerp_speed = 15f;
    }

    public void enter_play_mode_from_ship_builder() {
        if (!ship_builder.is_ship_valid()) return;

        Debug.Log("Play Mode");

        reset_ship_position();
        ModularShipBlueprintData blueprint = ship_builder.get_ship_blueprint();
        ship_controller.load_blueprint(blueprint);
        enter_play_mode_directly();
    }

    public void enter_play_mode_with_loaded_ship(string ship_name) {
        Debug.Log("Play Mode");
        
        reset_ship_position();
        ship_controller.load(ship_name);
        enter_play_mode_directly();
    }

    private void enter_play_mode_directly() {
        set_ship_to_spawn_position();
        ship_controller.set_active(true);

        // Hide ship builder
        ship_builder.gameObject.SetActive(false);

        // Swap player controls
        player_controller.character = ship_controller.GetComponent<Character>();
        player_controller.camera_rig.anchor_transform = ship_controller.transform;
        player_controller.camera_rig.anchor_lerp_speed = 15000f;
    }

    private void set_ship_to_spawn_position() {
        ship_controller.transform.position = get_ship_spawn_position();
        ship_controller.last_valid_camera_anchor_point = ship_controller.get_ship_position();
        ship_controller.transform.rotation = Quaternion.identity;
    }

    private void reset_ship_position() {
        ship_controller.transform.position = Vector3.zero;
        ship_controller.transform.rotation = Quaternion.identity;
    }

    private Vector3 get_ship_spawn_position() {
        return ship_controller.transform.position - ship_controller.rb.centerOfMass + Vector3.up * (ship_controller.get_bounds().size.y * 0.5f + spawn_point.y);
    }
}
