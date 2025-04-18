using System.Collections.Generic;
using UnityEngine;

public enum SpawnDirection {
    LEFT_RIGHT,
    ABOVE,
    BELOW,
    ANYWHERE,
}

public struct Spawner {
    public SpawnData data;
    public float spawn_timer;
    public GameObject[] spawned;

    public Spawner(SpawnData _data) {
        data = _data;
        spawn_timer = 0f;
        spawned = new GameObject[data.max_spawned];
    }
}

public class ObstacleSpawner : MonoBehaviour {
    public float no_spawn_radius = 60f;
    public float max_spawn_radius = 150f;
    public float minimum_height = 50f;

    public SpawnData[] spawn_datas;

    public Spawner[] troposphere_spawns;
    public Spawner[] stratosphere_spawns;
    public Spawner[] mesosphere_spawns;
    public Spawner[] thermosphere_spawns;
    public Spawner[] exosphere_spawns;
    public Spawner[] space_spawns;
    public Spawner[] aliens_spawns;
    public Spawner[] mars_spawns;

    public void init() {
        int troposphere_spawn_count = 0;
        int stratosphere_spawn_count = 0;
        int mesosphere_spawn_count = 0;
        int thermosphere_spawn_count = 0;
        int exosphere_spawn_count = 0;
        int space_spawn_count = 0;
        int aliens_spawn_count = 0;
        int mars_spawn_count = 0;

        foreach(SpawnData spawn_data in spawn_datas) {
            foreach(AtmosphereLayer atmosphere_layer in spawn_data.atmosphere_layers) {
                switch (atmosphere_layer) {
                    case AtmosphereLayer.TROPOSPHERE: { troposphere_spawn_count++; break; }
                    case AtmosphereLayer.STRATOSPHERE: { stratosphere_spawn_count++; break; }
                    case AtmosphereLayer.MESOSPHERE: { mesosphere_spawn_count++; break; }
                    case AtmosphereLayer.THERMOSPHERE: { thermosphere_spawn_count++; break; }
                    case AtmosphereLayer.EXOSPHERE: { exosphere_spawn_count++; break; }
                    case AtmosphereLayer.SPACE: { space_spawn_count++; break; }
                    case AtmosphereLayer.ALIENS: { aliens_spawn_count++; break; }
                    case AtmosphereLayer.PEACEFUL_SPACE: { break; }
                    default: { mars_spawn_count++; break; }
                }
            }
        }

        troposphere_spawns = new Spawner[troposphere_spawn_count];
        stratosphere_spawns = new Spawner[stratosphere_spawn_count];
        mesosphere_spawns = new Spawner[mesosphere_spawn_count];
        thermosphere_spawns = new Spawner[thermosphere_spawn_count];
        exosphere_spawns = new Spawner[exosphere_spawn_count];
        space_spawns = new Spawner[space_spawn_count];
        aliens_spawns = new Spawner[aliens_spawn_count];
        mars_spawns = new Spawner[mars_spawn_count];

        troposphere_spawn_count = 0;
        stratosphere_spawn_count = 0;
        mesosphere_spawn_count = 0;
        thermosphere_spawn_count = 0;
        exosphere_spawn_count = 0;
        space_spawn_count = 0;
        aliens_spawn_count = 0;
        mars_spawn_count = 0;

        foreach(SpawnData spawn_data in spawn_datas) {
            foreach(AtmosphereLayer atmosphere_layer in spawn_data.atmosphere_layers) {
                switch (atmosphere_layer) {
                    case AtmosphereLayer.TROPOSPHERE: {
                        troposphere_spawns[troposphere_spawn_count] = new Spawner(spawn_data);
                        troposphere_spawn_count++;
                        break;
                    }
                    case AtmosphereLayer.STRATOSPHERE: {
                        stratosphere_spawns[stratosphere_spawn_count] = new Spawner(spawn_data);
                        stratosphere_spawn_count++;
                        break;
                    }
                    case AtmosphereLayer.MESOSPHERE: {
                        mesosphere_spawns[mesosphere_spawn_count] = new Spawner(spawn_data);
                        mesosphere_spawn_count++;
                        break;
                    }
                    case AtmosphereLayer.THERMOSPHERE: {
                        thermosphere_spawns[thermosphere_spawn_count] = new Spawner(spawn_data);
                        thermosphere_spawn_count++;
                        break;
                    }
                    case AtmosphereLayer.EXOSPHERE: {
                        exosphere_spawns[exosphere_spawn_count] = new Spawner(spawn_data);
                        exosphere_spawn_count++;
                        break;
                    }
                    case AtmosphereLayer.SPACE: {
                        space_spawns[space_spawn_count] = new Spawner(spawn_data);
                        space_spawn_count++;
                        break;
                    }
                    case AtmosphereLayer.ALIENS: {
                        aliens_spawns[aliens_spawn_count] = new Spawner(spawn_data);
                        aliens_spawn_count++;
                        break;
                    }
                    case AtmosphereLayer.PEACEFUL_SPACE: { break; }
                    default: {
                        mars_spawns[mars_spawn_count] = new Spawner(spawn_data);
                        mars_spawn_count++;
                        break;
                    }
                }
            }
        }
    }

    private void FixedUpdate() {
        switch (AtmosphereManager.instance.current_layer) {
            case AtmosphereLayer.TROPOSPHERE: { try_spawn(troposphere_spawns); break; }
            case AtmosphereLayer.STRATOSPHERE: { try_spawn(stratosphere_spawns); break; }
            case AtmosphereLayer.MESOSPHERE: { try_spawn(mesosphere_spawns); break; }
            case AtmosphereLayer.THERMOSPHERE: { try_spawn(thermosphere_spawns); break; }
            case AtmosphereLayer.EXOSPHERE: { try_spawn(exosphere_spawns); break; }
            case AtmosphereLayer.SPACE: { try_spawn(space_spawns); break; }
            case AtmosphereLayer.ALIENS: { try_spawn(aliens_spawns); break; }
            case AtmosphereLayer.PEACEFUL_SPACE: { break; }
            default: { try_spawn(mars_spawns); break; }
        }
    }

    private void try_spawn(Spawner[] spawn_list) {
        // Debug.Log(spawn_list.Length);
        for(uint i = 0; i < spawn_list.Length; i++) {
            spawn_list[i].spawn_timer += Time.deltaTime;
            if(spawn_list[i].spawn_timer >= spawn_list[i].data.spawn_rate) {
                spawn_list[i].spawn_timer -= spawn_list[i].data.spawn_rate;

                Queue<int> available_spawn_indexes = new Queue<int>();
                for(int j = 0; j < spawn_list[i].spawned.Length; j++) if(spawn_list[i].spawned[j] == null) available_spawn_indexes.Enqueue(j);

                if(available_spawn_indexes.Count > 0) {
                    int spawn_index = available_spawn_indexes.Dequeue();
                    Vector3 spawn_point = get_spawn_point(spawn_list[i].data.spawn_direction);
                    spawn_point.y = Mathf.Max(spawn_point.y, minimum_height);
                    spawn_list[i].spawned[spawn_index] = Instantiate(spawn_list[i].data.spawn_prefab, spawn_point, spawn_list[i].data.spawn_prefab.transform.rotation);
                }
            }
        }
    }

    private Vector3 get_spawn_point(SpawnDirection spawn_direction) {
        switch(spawn_direction) {
            case SpawnDirection.LEFT_RIGHT: { return get_left_right_spawn_point(); }
            case SpawnDirection.ABOVE: { return get_above_spawn_point(); }
            case SpawnDirection.BELOW: { return get_below_spawn_point(); }
            default: { return get_anywhere_spawn_point(); }
        }
    }

    private Vector3 get_left_right_spawn_point() {
        float value = (Random.value - 0.5f) * 2f;
        if(value > 0f) {
            // Right
            return GameManager.instance.ship_controller.get_ship_position() + new Vector3(Mathf.Max(Random.Range(no_spawn_radius, max_spawn_radius)), 0f, 0f);
        } else {
            // Left
            return GameManager.instance.ship_controller.get_ship_position() + new Vector3(Random.Range(-no_spawn_radius, -max_spawn_radius), 0f, 0f);
        }
    }

    private Vector3 get_above_spawn_point() {
        float value = (Random.value - 0.5f) * 2f;
        Vector3 offset = Vector3.zero;
        if(value > 0f) {
            // Right
            offset = new Vector3(Mathf.Max(Random.Range(no_spawn_radius, max_spawn_radius)), 0f, 0f);
        } else {
            // Left
            offset = new Vector3(Random.Range(-no_spawn_radius, -max_spawn_radius), 0f, 0f);
        }

        return GameManager.instance.ship_controller.get_ship_position() + new Vector3(0f, Random.Range(no_spawn_radius, max_spawn_radius), 0f) + offset;
    }

    private Vector3 get_below_spawn_point() {
        return GameManager.instance.ship_controller.get_ship_position() + new Vector3(0f, Random.Range(-no_spawn_radius, -max_spawn_radius), 0f);
    }

    private Vector3 get_anywhere_spawn_point() {
        switch(Random.Range(0, 3)) {
            case 0: return get_left_right_spawn_point();
            case 1: return get_above_spawn_point();
            default: return get_below_spawn_point();
        }
    }
}
