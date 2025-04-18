using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ModularShipBlueprintData {
    const string SHIPS_FOLDER = "/Ships";

    public List<Grid3DItemData> item_datas = new List<Grid3DItemData>();

    public ModularShipBlueprintData() {}

    public ModularShipBlueprintData(List<Grid3DItemData> _item_datas) {
        item_datas = _item_datas;
    }

    public void save_as(string ship_name) {
        string destination = get_ship_file_destination(ship_name);

        Directory.CreateDirectory(get_ship_folder());
        FileStream file = File.Exists(destination) ? File.OpenWrite(destination) : File.Create(destination);

        Grid3DItemDataSerialized[] serialized_datas = get_serialized_datas();
        BinaryFormatter binary_formatter = new BinaryFormatter();
        binary_formatter.Serialize(file, serialized_datas);
        file.Close();
    }

    public void load_premade(string ship_name) {
        load_from_destination(get_premade_ship_file_destination(ship_name));
    }

    public void load(string ship_name) {
        load_from_destination(get_ship_file_destination(ship_name));
    }

    private void load_from_destination(string destination) {
        FileStream file;
        if (File.Exists(destination)) file = File.OpenRead(destination);
        else {
            Debug.LogError("Ship file not found.");
            return;
        }

        BinaryFormatter binary_formatter = new BinaryFormatter();
		Grid3DItemDataSerialized[] serialized_datas = (Grid3DItemDataSerialized[])binary_formatter.Deserialize(file);
		file.Close();

        item_datas = new List<Grid3DItemData>();
        foreach(Grid3DItemDataSerialized serialized_data in serialized_datas) item_datas.Add(new Grid3DItemData(serialized_data));
    }

    public string get_ship_folder() { return Application.persistentDataPath + SHIPS_FOLDER; }
    public string get_ship_file_destination(string ship_name) { return Application.persistentDataPath + SHIPS_FOLDER + "/" + ship_name + ".dat"; }
    public string get_premade_ship_file_destination(string ship_name) { return "Assets/Resources" + SHIPS_FOLDER + "/" + ship_name + ".dat"; }

    public Grid3DItemDataSerialized[] get_serialized_datas() {
        Grid3DItemDataSerialized[] serialized_datas = new Grid3DItemDataSerialized[item_datas.Count];
        for (int i = 0; i < item_datas.Count; i++) serialized_datas[i] = item_datas[i].get_serialized();
        return serialized_datas;
    }
}
