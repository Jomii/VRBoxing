using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    float hitTimer = 0.0f;

    void Update() {
      if (hitTimer > 0) {
        hitTimer -= Time.deltaTime;
      }
    }

    void OnTriggerEnter(Collider other) {
      GameObject target = other.gameObject;

      if (target != null && hitTimer <= 0) {
        hitTimer = 0.75f;
        target.SetActive(false);
      }
    }
}
