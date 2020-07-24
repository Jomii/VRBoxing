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
      LS, RS, LH, RS, LU, LS, RS, LH, RS, LU
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

    // Update is called once per frame
    void Update()
    {
      // UpdateTargetAndPreview();

      // moveTimer -= Time.deltaTime;
      // movingTargets.ForEach(t => t.transform.position = Vector3.MoveTowards(t.transform.position, leftEnd.position, 1.0f * Time.deltaTime));
      // movingTarget.transform.position = Vector3.MoveTowards(movingTarget.transform.position, leftEnd.position, 1.0f * Time.deltaTime);
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

    void SpawnTargets() {
      for (int i = 1; i < trackStep; i++) {
        if (i >= moveList.Count) return;

        nextHitbox = parseMove(moveList[i]);
        bool isLeftSideTarget = nextHitbox.tag == "LeftTarget" ? true : false;

        Vector3 startPos = isLeftSideTarget ? leftStart.position : rightStart.position;
        Vector3 endPos = isLeftSideTarget ? leftEnd.position : rightEnd.position;

        GameObject target = Instantiate(previewPrefab, startPos, nextHitbox.transform.rotation);

        // if (i == 0) {
        //   target = Instantiate(targetPrefab, startPos, nextHitbox.transform.rotation);
        // } else {
        //   target = Instantiate(previewPrefab, startPos, nextHitbox.transform.rotation);
        // }

        target.transform.SetParent(this.transform);
        // target.transform.parent = this.transform; // move instantiated object inside TargetSpawn object

        target.transform.position = Vector3.MoveTowards(target.transform.position, endPos, step * (trackStep - i)); 

        movingTargets.Add(target);

        // moveIndex += 1;
      }

      // movingTargets.RemoveAt(0);
      // moveIndex = 0;
    }

    public void MoveTargets() {
      if (activeHitbox) activeHitbox.SetActive(false); // disable previous
      Debug.Log("targets remaining: " + movingTargets.Count);
      movingTargets.ForEach(target => 
        target.transform.position = Vector3.MoveTowards(target.transform.position, target.tag == "LeftTarget" ? leftEnd.position : rightEnd.position, step)
      );

      if (movingTargets.Count > 0) {
        GameObject preview = movingTargets[0];
        movingTargets.RemoveAt(0);
        Destroy(preview);
      }


      activeHitbox = parseMove(moveList[moveIndex + 1]);
      if (activeHitbox) activeHitbox.SetActive(true);
      moveIndex += 1;
    }
}
