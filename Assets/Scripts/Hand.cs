using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    float hitTimer = 0.0f;
    Renderer materialRenderer;
    Color color;

    void Start() {
      materialRenderer = gameObject.GetComponent<Renderer>();
      color = materialRenderer.material.GetColor("_Color");
    }

    void Update() {
      materialRenderer = gameObject.GetComponent<Renderer>();
      if (hitTimer > 0) {
        materialRenderer.material.SetColor("_Color", Color.white);
        hitTimer -= Time.deltaTime;
      } else {
        materialRenderer.material.SetColor("_Color", color);
      }
    }

    void OnTriggerEnter(Collider other) {
      GameObject target = other.gameObject;

      if (target != null && target.tag == "hitbox" && hitTimer <= 0) {
        hitTimer = 0.75f;
        target.SetActive(false);
      }
    }
}
