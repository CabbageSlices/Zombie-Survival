using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {

	//health properties
    private int Health {
           
        get { return _health; }

        set { _health = value >= 0 ? value : 0; }
    }
    
    public int initialHealth = 10;
    private int _health;

    private bool deathResponseActivated = false;

    //event triggered when this entity's health reaches zero
    public delegate void Response();
    public event Response onDeath;

    //event triggered when this entity get's hit (could do an animation, trigger a sound effect, etc.)
    //fractionHealthRemaining = what fraction of initialHealth is remaining
    public delegate void HitResponse(int damageReceived, float fractionHealthRemaining);
    public event HitResponse onHit;

    public void Start() {

        Health = initialHealth;
    }

    //receive damage and lose health
    //triggers the onHit event, and if the health reaches 0 it will trigger the onDeath event
    public void getHit(int damage) {
        
        Health = Health - damage;

        if(onHit != null)
            onHit(damage, (float) Health / (float) initialHealth);

        if (Health == 0 && !deathResponseActivated) {

            if(onDeath != null)
                onDeath();

            deathResponseActivated = true;
        }
    }
}
