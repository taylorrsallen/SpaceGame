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
    
    [Header("Modes")]
    [ReadOnly] GameMode game_mode = GameMode.NONE;

    [Header("Career Mode")]
    public string starting_ship;
    public Vector3 spawn_point = new Vector3(0f, 0f, 0f);

    [Header("Game Data")]
    public ShipComponentData[] ship_components;
    public GameResourceData[] game_resources;
    [HideInInspector] public ulong[] player_resources;

    [HideInInspector] public GameResource resource_for_height;

    private void Awake() {
        if(instance != null && instance != this) Destroy(this);
        else instance = this;
        DontDestroyOnLoad(this);

        player_controller = FindAnyObjectByType<PlayerController>();
        ship_builder = FindAnyObjectByType<ModularShipBuilder>(FindObjectsInactive.Include);
        ship_controller = FindAnyObjectByType<ModularShipController>(FindObjectsInactive.Include);

        ship_controller.init();
        ship_builder.init();

        for(int i = 0; i < ship_components.Length; i++) ship_components[i].id = i;
        player_resources = new ulong[game_resources.Length];
        for(uint i = 0; i < game_resources.Length; i++) { player_resources[i] = 0; }
    }

    public void start_career_mode() {
        game_mode = GameMode.CAREER;
        player_controller.camera_rig.gameObject.SetActive(true);
        enter_play_mode_with_loaded_ship(starting_ship);
    }

    public string get_player_resource_as_string(int resource_id) { return game_resources[resource_id].get_amount_as_string(player_resources[resource_id]); }

    public void add_resource(GameResource resource) { player_resources[resource.id] += resource.amount; }
    public void remove_resource(GameResource resource) { player_resources[resource.id] -= resource.amount; }

    public void spawn_resource_add_particle(GameResource resource, Transform spawn_transform) {
        // TEMP
        add_resource(resource);
    }

    public void spawn_resource_remove_particle(GameResource resource, Transform target_transform) {
        // TEMP
        remove_resource(resource);
    }

    public void update_height_resource(float height) {
        resource_for_height.amount = (ulong)Mathf.Max(resource_for_height.amount, AtmosphereManager.instance.get_height_cash(height));
    }

    #region ShipBuilder
    public void toggle_ship_builder() {
        if(game_mode == GameMode.NONE) return;
        if(ship_builder.gameObject.activeSelf) {
            if(!ship_builder.set_hotkey_target) enter_play_mode_from_ship_builder();
        } else {
            enter_ship_builder();
        }
    }

    public void enter_ship_builder() {
        Debug.Log("Build Mode");

        if(resource_for_height.amount > 0) {
            add_resource(resource_for_height);
            resource_for_height.amount = 0;
        }

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
        ship_builder._is_ship_valid = ship_builder.is_ship_valid();

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
        ship_controller.load_premade(ship_name);
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
    #endregion
}
