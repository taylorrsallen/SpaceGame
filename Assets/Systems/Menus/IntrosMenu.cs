using System;
using UnityEngine;

public class IntrosMenu : MonoBehaviour {
    private int play_index = 0;
    private bool _is_playing;

    public Action on_finished;

    public void init() {
        foreach(IIntroPlayer intro_player in GetComponentsInChildren<IIntroPlayer>(true)) intro_player.init();
    }

    private void Update() {
        if(!_is_playing) return;
        if(is_finished()) return;

        IIntroPlayer intro_player = transform.GetChild(play_index).GetComponent<IIntroPlayer>();
        if(intro_player == null) { play_index++; return; }

        if(intro_player.is_finished()) { play_index++; return; }
        if(!intro_player.is_playing()) intro_player.play();
    }

    public bool is_finished() { return play_index >= transform.childCount; }
    public bool is_playing() { return _is_playing; }

    public void play() { _is_playing = true; }
}
