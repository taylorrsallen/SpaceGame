using UnityEngine;

public enum IntroState {
    FADE_IN,
    HOLD,
    FADE_OUT,
}

public interface IIntroPlayer {
    public void init();
    public void play();
    public bool is_playing();
    public bool is_finished();
}