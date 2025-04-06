using UnityEngine;

static public class DataManager {
    static private ContentDatabase content_database = Resources.Load("Data/ContentDatabase") as ContentDatabase;

    static public MatterData get_matter(byte matter_id) { return content_database.matter_database[matter_id]; }
}
