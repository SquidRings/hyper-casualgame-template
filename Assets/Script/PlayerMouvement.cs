using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouvement : MonoBehaviour
{
    public float speed;
    private float velocity;
    private Camera mainCam;
    public float roadEndPoint;

    private Transform player;
    private Vector3 firstMousePos, firstPlayerPos;
    private bool moveTheBall;

    private float camVelocity;
    public float camspeed = 0.4f;
    private Vector3 offset;

    public float playerZSpeed = 20f;

    public GameObject bodyPrefab;
    public int gap = 2;
    public float bodySpeed = 15f;

    private List<GameObject> bodyParts = new List<GameObject>();
    private List<int> bodyPartsIndex = new List<int>();
    private List<Vector3> PositionHistory = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        player = this.transform;
        offset = mainCam.transform.position - player.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            moveTheBall = true;
        } else if (Input.GetMouseButtonUp(0)) {
            moveTheBall = false;
    }


    if (moveTheBall) {
         Plane newPlane = new Plane (Vector3.up, 0.8f);
         Ray ray = mainCam.ScreenPointToRay (Input.mousePosition);


    if (newPlane.Raycast(ray, out var distance)) {
        Vector3 newMousePos = ray.GetPoint(distance) - firstMousePos;
        Vector3 newPlayerPos = newMousePos + firstMousePos;
        newPlayerPos.x = Mathf.Clamp(newPlayerPos.x, -roadEndPoint, roadEndPoint);
        player.position = new Vector3(Mathf.SmoothDamp(player.position.x, newPlayerPos.x , ref velocity, speed), 
        player.position.y, player.position.z);
            }
        }
    }

    private void FixedUpdate() {
        player.position += Vector3.forward * playerZSpeed * Time.fixedDeltaTime;


        PositionHistory.Insert(0, transform.position);
        int index =0;
        foreach(var body in bodyParts){
            Vector3 point = PositionHistory[Mathf.Min(index * gap, PositionHistory.Count -1)];
            Vector3 moveDir = point - body.transform.position;
            body.transform.position += moveDir * bodySpeed * Time.fixedDeltaTime;
            body.transform.position = PositionHistory[index];
            index++;
        }
    }

    private void LateUpdate () {
        Vector3 newCamPos = mainCam.transform.position;
        mainCam.transform.position = new Vector3(Mathf.SmoothDamp(newCamPos.x, player.position.x , ref camVelocity, camspeed), 
        newCamPos.y, player.position.z + offset.z);
        }

    public void GrowBody() {
        GameObject body = Instantiate(bodyPrefab, transform.position, transform.rotation);
        bodyParts.Add(body);
        int index = 0;
        index++;
        bodyPartsIndex.Add(index);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "CellObs") {
            Destroy(other.gameObject, 0.005F);
            GrowBody();
        }
    }
}