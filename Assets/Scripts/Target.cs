using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    int hitboxCount;
    GameObject[] hitboxes;
    // Start is called before the first frame update
    void Start()
    {
      hitboxCount = transform.childCount;
      hitboxes = new GameObject[hitboxCount];

      // Disable all child elements (hitboxes)
      for (int i = 0; i < hitboxCount; i++) {
        GameObject child = transform.GetChild(i).gameObject;
        child.SetActive(false);
        // Add hitbox gameobjects to array
        hitboxes[i] = child;
      }
    }

    // Update is called once per frame
    void Update()
    {
      bool allHidden = true;
      // Check if any hitboxes are enabled
      foreach(GameObject hitbox in hitboxes) {
        if (hitbox.activeSelf) {
          allHidden = false;
        }
      }

      if (allHidden) {
        // Randomly enable a hitbox if all are disabled
        int randomHitboxIndex = Random.Range(0, hitboxCount);

        // Check that random is between 0 and the amount of hitboxes
        if (randomHitboxIndex >= 0 && randomHitboxIndex <= hitboxCount) {
          hitboxes[randomHitboxIndex].SetActive(true);
        } else {
          Debug.Log("Random hitbox index was not between 0 and " + hitboxCount + " :(");
        }
      }
    }
}
