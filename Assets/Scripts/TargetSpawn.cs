using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawn : MonoBehaviour
{
    public float moveInterval = 1.0f;
    public GameObject previewPrefab;
    public GameObject targetPrefab;
    public GameObject LeftStraightTarget;
    public GameObject LeftHookTarget;
    public GameObject RightStraightTarget;
    public GameObject RightHookTarget;
    public GameObject LeftUppercutTarget;
    public GameObject RightUppercutTarget;
    public GameObject LeftStraightPreview;
    public GameObject LeftHookPreview;
    public GameObject RightStraightPreview;
    public GameObject RightHookPreview;
    public GameObject LeftUppercutPreview;
    public GameObject RightUppercutPreview;
    public Transform rightStart;
    public Transform rightEnd;

    const string W = "W"; // Wait
    const string LS = "LS"; // Left Straight
    const string LH = "LH"; // Left Hook
    const string RS = "RS"; // Right Straight
    const string RH = "RH"; // Right Hook
    const string LU = "LU"; // Left Uppercut
    const string RU = "RU"; // Right Uppercut

    float levelDuration; // movelist / moveInterval - note/movecount better variable name?
    float moveTimer = 0.0f;
    int trackStep = 7;
    float dist; // Doesn't take right side into account properly
    float step;

    GameObject activeHitbox;
    GameObject nextHitbox;
    Transform leftStart;
    Transform leftEnd;
    List<GameObject> movingTargets;
    List<string> moveList = new List<string>{
      LS, W, LS, W, RS, W, RS, W, LS, LS, W, RS, RS, W, LS, RS, LS, W, LS, RS, LS, W, LS, RS, LS, RS, LS, RS, W, LS, RS, LS, RS, LS, RS, W, LS, RS, LH, RS, LU, W, LS, RS, LH, RS, LU, RU, LS, RH
    };

    int moveIndex;
    int previewIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
      levelDuration = moveList.Count / moveInterval;
      moveIndex = 0;
      moveTimer = moveInterval;
      activeHitbox = parseMove(moveList[0]);
      nextHitbox = parseMove(moveList[1], true);

      if (activeHitbox) activeHitbox.SetActive(true);
      // if (nextHitbox) nextHitbox.SetActive(true);

      // TODO: init these like right side
      leftStart = transform.Find("LeftStart");
      leftEnd = transform.Find("LeftEnd");
      dist = Vector3.Distance(leftStart.position, leftEnd.position); // Doesn't take right side into account properly
      step = dist / trackStep;

      if (!leftStart) Debug.Log("No left spawn found");
      if (!leftEnd) Debug.Log("No left end found");

      movingTargets = new List<GameObject>();
      SpawnTargets();
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
          case LU:
            return isPreview ? LeftUppercutPreview : LeftUppercutTarget;
          case RU:
            return isPreview ? RightUppercutPreview : RightUppercutTarget;
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

    void SpawnTargets(bool spawnSingle = false) {
      int indexesMoved = 0;
      int amountToSpawn = spawnSingle ? 2 : trackStep;

      for (int i = 1; i < amountToSpawn; i++) {
        if (i + previewIndex >= moveList.Count) return;

        indexesMoved = i;
        nextHitbox = parseMove(moveList[i + previewIndex]);

        if (nextHitbox == null) break;

        bool isLeftSideTarget = nextHitbox.tag == "LeftTarget" ? true : false;

        Vector3 startPos = isLeftSideTarget ? leftStart.position : rightStart.position;
        Vector3 endPos = isLeftSideTarget ? leftEnd.position : rightEnd.position;
        GameObject target = Instantiate(previewPrefab, startPos, nextHitbox.transform.rotation);

        target.transform.SetParent(this.transform);

        if (!spawnSingle) {
          target.transform.position = Vector3.MoveTowards(target.transform.position, endPos, step * (trackStep - i)); 
        }

        movingTargets.Add(target);
      }

      previewIndex += indexesMoved;
    }

    IEnumerator SpawnTargetsWithDelay() {
      yield return new WaitForSeconds(2);

      SpawnTargets();
      MoveTargets(); // Move targets once to activate hitbox and start the new combo
    }

    public void MoveTargets() {
      if (activeHitbox) activeHitbox.SetActive(false); // disable previous

      if (movingTargets.Count > 0) {
        GameObject preview = movingTargets[0];
        movingTargets.RemoveAt(0);
        Destroy(preview);
      }

      // Spawn a target of the current combo
      if (parseMove(moveList[previewIndex]) != null) {
        SpawnTargets(true);
      }

      Debug.Log("targets remaining: " + movingTargets.Count);

      // Move targets
      movingTargets.ForEach(target => 
        target.transform.position = Vector3.MoveTowards(target.transform.position, target.tag == "LeftTarget" ? leftEnd.position : rightEnd.position, step)
      );

      // Enable new target
      moveIndex += 1;
      if (moveIndex >= moveList.Count) return;

      activeHitbox = parseMove(moveList[moveIndex]);
      if (activeHitbox) {
        activeHitbox.SetActive(true);
      } else {
        // wait a bit and spawn new combo set
        StartCoroutine(SpawnTargetsWithDelay());
      }

      Debug.Log("moveIndex: " + moveIndex + " previewIndex: " + previewIndex);
    }
}
