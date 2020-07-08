using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawn : MonoBehaviour
{
    public float moveInterval = 1.0f;
    public GameObject LeftStraightTarget;
    public GameObject LeftHookTarget;
    public GameObject RightStraightTarget;
    public GameObject RightHookTarget;
    public GameObject UppercutTarget;
    public GameObject LeftStraightPreview;
    public GameObject LeftHookPreview;
    public GameObject RightStraightPreview;
    public GameObject RightHookPreview;
    public GameObject UppercutPreview;

    const string W = "W"; // Wait
    const string LS = "LS"; // Left Straight
    const string LH = "LH"; // Left Hook
    const string RS = "RS"; // Right Straight
    const string RH = "RH"; // Right Hook
    const string U = "U"; // Uppercut

    float levelDuration; // movelist / moveInterval - note/movecount better variable name?
    float moveTimer = 0.0f;
    GameObject activeHitbox;
    GameObject nextHitbox;
    List<string> moveList = new List<string>{
      W, W, W, W, W, W, W, W, LS, RS, LS, RH, U, U, LS, RS, LS, W, W, W,
      LS, RS, LH, RH, LS, RS, LH, RS, U, U, LH, RH, W, W, LS, RS, W, LS,
      U, U
    };

    int moveIndex;

    // Start is called before the first frame update
    void Start()
    {
      levelDuration = moveList.Count / moveInterval;
      moveIndex = 0;
      moveTimer = moveInterval;
      activeHitbox = parseMove(moveList[0]);
      nextHitbox = parseMove(moveList[1], true);

      if (activeHitbox) activeHitbox.SetActive(true);
      if (nextHitbox) nextHitbox.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
      UpdateTargetAndPreview();

      moveTimer -= Time.deltaTime;
    }

    /// <summary> Parses move string and returns corresponding gameobject or null if not found </summary>
    GameObject parseMove(string move, bool isPreview = false) {
      switch (move)
      {
          case W:
            return null;
          case LS:
            return isPreview ? LeftStraightPreview : LeftStraightTarget;
          case LH:
            return isPreview ? LeftHookPreview : LeftHookTarget;
          case RS:
            return isPreview ? RightStraightPreview : RightStraightTarget;
          case RH:
            return isPreview ? RightHookPreview : RightHookTarget;
          case U:
            return isPreview ? UppercutPreview : UppercutTarget;
          default:
            return null;
      }
    }

    void UpdateTargetAndPreview() {
      if (moveTimer <= 0) {
        // change move to next on the list
        moveIndex++;

        // Hide previous target and preview
        if (activeHitbox) activeHitbox.SetActive(false);
        if (nextHitbox) nextHitbox.SetActive(false);

        if (moveIndex < moveList.Count) {
          activeHitbox = parseMove(moveList[moveIndex]);

          if (activeHitbox) activeHitbox.SetActive(true);
          
          if (moveIndex + 1 < moveList.Count) {
            nextHitbox = parseMove(moveList[moveIndex + 1], true);
          } else {
            nextHitbox = null;
          }

          if (nextHitbox) nextHitbox.SetActive(true);
          
          // Reset movetimer
          moveTimer = moveInterval;
        }
      }
    }
}
