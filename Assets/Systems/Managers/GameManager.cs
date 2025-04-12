using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance { get; private set; }

    public PlayerController player_controller;
    public ModularShipBuilder ship_builder;
    public ModularShipController ship_controller;

    public Vector3 spawn_point = new Vector3(0f, 10f, 0f);

    [SerializeField] public ShipComponentData[] ship_components;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }

        player_controller = FindAnyObjectByType<PlayerController>();
        ship_builder = FindAnyObjectByType<ModularShipBuilder>();
        ship_controller = FindAnyObjectByType<ModularShipController>(FindObjectsInactive.Include);
    }

    public void toggle_ship_builder() {
        if (ship_builder.gameObject.activeSelf) {
            enter_play_mode();
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

    public void enter_play_mode() {
        if (!ship_builder.is_ship_valid()) return;

        Debug.Log("Play Mode");

        // Get blueprint from ship builder
        ModularShipBlueprintData new_blueprint = ship_builder.get_ship_blueprint();

        // Spawn & assign blueprint
        ship_controller.load_blueprint(new_blueprint);
        ship_controller.set_active(true);

        // Hide ship builder
        ship_builder.gameObject.SetActive(false);

        // Swap player controls
        player_controller.character = ship_controller.GetComponent<Character>();
        player_controller.camera_rig.anchor_transform = ship_controller.transform;
        player_controller.camera_rig.anchor_lerp_speed = 15000f;
    }
}
