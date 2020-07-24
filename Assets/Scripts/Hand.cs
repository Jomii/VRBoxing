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

    void OnCollisionEnter(Collision collision) {
      GameObject target = collision.gameObject;

      if (target != null && target.tag != "Player" && hitTimer <= 0) {
        Debug.Log("hand collision detected with: " + target.name + " with vel: " + collision.relativeVelocity.magnitude);
        hitTimer = 0.75f;

        // TODO: Connect hit with target spawn
        TargetSpawn targetSpawn = target.GetComponentInParent<TargetSpawn>();
        targetSpawn.MoveTargets();

        if (collision.relativeVelocity.magnitude > 1.75f) {
          Debug.Log("Big hit!");
        }
        // target.SetActive(false);
        // Destroy(target);
      }

    }
}
