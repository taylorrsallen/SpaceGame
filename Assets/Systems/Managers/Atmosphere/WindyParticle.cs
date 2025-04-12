using UnityEngine;

public class WindyParticle : MonoBehaviour {
    private ParticleSystem particle_system;
    public float dust_lerp_speed = 25f;
    public float dust_wind_multiplier = 4f;

    private void Awake() {
        particle_system = GetComponent<ParticleSystem>();
    }

    private void Update() {
        ParticleSystem.VelocityOverLifetimeModule velocity = particle_system.velocityOverLifetime;
        velocity.space = ParticleSystemSimulationSpace.World;
        Vector3 wind_force = AtmosphereManager.instance.get_wind_force() * dust_wind_multiplier;
        velocity.x = Mathf.Lerp(velocity.x.curveMultiplier, wind_force.x, Time.deltaTime * dust_lerp_speed);
        velocity.y = Mathf.Lerp(velocity.y.curveMultiplier, wind_force.y, Time.deltaTime * dust_lerp_speed);
        velocity.z = Mathf.Lerp(velocity.z.curveMultiplier, wind_force.z, Time.deltaTime * dust_lerp_speed);
    }
}
