using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColor : MonoBehaviour {
    public ParticleSystem particleSystem;
    void Start() {
        particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    public void UpdateParticleColor(string color) {
        var main = particleSystem.main;
        switch (color)
        {
            case "Red":
                main.startColor = new ParticleSystem.MinMaxGradient(Color.red);
                break;
            case "Orange":
                main.startColor = new ParticleSystem.MinMaxGradient(new Color(255,165,0));

                break;
            case "Yellow":
                main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow);
                break;
            case "Green":
                main.startColor = new ParticleSystem.MinMaxGradient(Color.green);
                break;

            case "Blue":
                main.startColor = new ParticleSystem.MinMaxGradient(Color.blue);
                break;

            case "Violet":
                main.startColor = new ParticleSystem.MinMaxGradient(Color.magenta);
                break;
            case "Brown":
                main.startColor = new ParticleSystem.MinMaxGradient(new Color(139,69,19));
                break;
            case "Black":
                main.startColor = new ParticleSystem.MinMaxGradient(Color.black);
                break;
            case "White":
                main.startColor = new ParticleSystem.MinMaxGradient(Color.white);
                break;

            default:
                break;
        }
    }
}
