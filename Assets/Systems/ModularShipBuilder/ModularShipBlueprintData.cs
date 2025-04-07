using System.Collections.Generic;
using UnityEngine;

public class ModularShipBlueprintData {
    public List<Grid3DItemData> item_datas = new List<Grid3DItemData>();

    public ModularShipBlueprintData(List<Grid3DItemData> _item_datas) {
        item_datas = _item_datas;
    }
}
