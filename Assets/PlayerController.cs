using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : NetworkBehaviour {
   //public GameObject self;
   public bool hideCursor;
   public float speed;
   public float bulletLife;
   public float jumpLeft;
   public float bulletSpeed; //default 300 //100-300 = slow. 1000=fast, 5000=almost cs speed
   public float frameCounter;
   public float frameSinceFire;

   public List<GameObject> cams;
   public List<GameObject> enemies;
   public List<GameObject> bullets;
   private List<Quaternion> bulletAngles;

   private float timeLastBulletShot;
   public int numEnemies;
   private GameObject cam_obj;
   private Vector3 movePhysics;

   void initCam() {
      cam_obj = new GameObject();
      cam_obj.name = "PlayerCam";
      var cam = cam_obj.AddComponent<Camera>();
      var camt = cam_obj.transform;
      camt.eulerAngles = new Vector3(20.0f, 265.1f, 0.0f);
      camt.position = new Vector3(2.0f, 4.5f, -0.5f); //5.0f, 0.0f);
      //cam_obj.transform.scale
      camt.parent = gameObject.transform;
   }

	 // Use this for initialization
   void Start() {
      //transform.position = new Vector3(0.0f, 0.0f, 0.0f);
      //if (!isLocalPlayer) return;

      movePhysics = new Vector3(0.0f, 0.0f, 0.0f);
      timeLastBulletShot = 0;
      frameCounter = 0;

      cams = new List<GameObject>();
      bullets = new List<GameObject>();
      bulletAngles = new List<Quaternion>();
      enemies = new List<GameObject>();

      transform.position += new Vector3(0.0f, 4.0f, 0.0f); //y = 4

      initCam();
      initEnemies();


      if (hideCursor) {
         //Screen.showCursor = false;
         //UnityEngine.Cursor.visible = false;
         //Screen.lockCursor = true;

         //this works pretty well:
         Cursor.lockState = CursorLockMode.Locked;
         Cursor.visible = false;
      }
   }

   void Update() {
      //GameObject.Find("PlayerCam").GetComponent<Camera>().enabled = false;
      cam_obj.GetComponent<Camera>().enabled = false;

      if (!isLocalPlayer) return;

      cam_obj.GetComponent<Camera>().enabled = true; //GameObject.Find("PlayerCam").

      destroyOldBullets();
      processEveryBulletMovement();

      var camT = transform; //GameObject.Find("Main Camera").transform;
      var camRot = camT.rotation;
     
      /*if (camRot.x > 24) camT.eulerAngles = new Vector3(24, camRot.y, camRot.z);
      if (camRot.x < 20) camT.eulerAngles = new Vector3(20, camRot.y, camRot.z);*/

      if (Input.GetButton("Fire1") && frameSinceFire > 10) {
         leftClick();
         frameSinceFire = 0;
      }

      /*if (transform.position.y > 2.9f)
         self.AddComponent<Rigidbody>();
      else if (transform.position.y < 1.64f)
         Destroy(self.GetComponent<Rigidbody>());*/

      float lookUp = -Input.GetAxis("Mouse Y");
      float lookAround = Input.GetAxis("Mouse X")*1.0f;
      transform.eulerAngles += new Vector3(0, lookAround, lookUp);

      //START FIXED UPDATE

      float moveHoriz = Input.GetAxis("Horizontal");
      float moveVert = -Input.GetAxis("Vertical");
      if (Input.GetKeyDown("space")) jumpLeft += 1.5f;
      float currJump = 0.0f;

      Vector3 move = new Vector3(moveVert, currJump, moveHoriz);
      move = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * move;

      /*var rb = GetComponent<Rigidbody>();
      rb.AddForce(move * speed);*/
      transform.position += move*speed;

      movePhysics = move;
      //END FIXED UPDATE
	}

   void initEnemies() {
      for (int i = 0; i < numEnemies; i++) {
         float x = Random.Range(-100, 100);
         float z = Random.Range(-100, 100);

         var enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
         enemy.AddComponent<EnemyMnMEminemAI>();
         enemy.transform.position = new Vector3(x, 1.27f, z);
         enemies.Add(enemy);
      }
   }

	void FixedUpdate() {
      //if (!isLocalPlayer) return;

      //kk tmp physics based movement:
      var rb = GetComponent<Rigidbody>();
      //rb.AddForce(movePhysics*1000);

      frameCounter += 1;
      frameSinceFire += 1;

      float currJump = 0.0f;
      if (jumpLeft > 0.0f) {
         currJump = 0.15f;
         jumpLeft -= 0.15f;
      } else if (transform.position.y > 0.5) {
         currJump = -0.15f;
      }
      //else rb.useGravity = false;
      Vector3 move_jump = new Vector3(0, currJump, 0);
      transform.position += move_jump;
   }

   void OnBulletCollide(GameObject bullet, Collision col) {
      if (col.gameObject.name != "Ground")
            Destroy(col.gameObject);
   }

   void destroyOldBullets() {
      if (bullets.Count > 0 && (Mathf.Abs(Time.time - timeLastBulletShot) > bulletLife)) {
         foreach (GameObject b in bullets)
            Destroy(b);
         bulletAngles.Clear();
         bullets.Clear();
      }
   }

   void processEveryBulletMovement() {
      var z = 0;
      foreach (GameObject b in bullets) {
         Quaternion camAngle = bulletAngles[z++];
         Vector3 moveBullet = camAngle * new Vector3(0.0f, 0.0f, 1.0f); // *bulletSpeed);
         if (b != null)
            b.transform.position += moveBullet;
      }
   }

   void leftClick() {
      Transform cam = cam_obj.transform; //transform.Find("PlayerCam");

      timeLastBulletShot = Time.time;
      var bulletObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
      bullets.Add(bulletObj);
      bulletAngles.Add(cam.rotation);

      bulletObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
      bulletObj.transform.eulerAngles = new Vector3(0, 90, 90) + cam.eulerAngles;

      Vector3 bulletPos = cam.position;
      bulletObj.transform.position = bulletPos;

      Rigidbody rb = bulletObj.AddComponent<Rigidbody>();
      rb.useGravity = false;
      Vector3 bulletSpeedVec = new Vector3(0, 0, 1*bulletSpeed);
      //Debug.Log(" x: " + cam.eulerAngles.x +  "y: " + cam.eulerAngles.y + "z: " + cam.eulerAngles.z);

      bulletSpeedVec = Quaternion.AngleAxis(cam.eulerAngles.y, Vector3.up) * bulletSpeedVec;
      bulletSpeedVec = Quaternion.AngleAxis(cam.eulerAngles.z, Vector3.right) * bulletSpeedVec;
      bulletSpeedVec = Quaternion.AngleAxis(-cam.eulerAngles.x, Vector3.forward) * bulletSpeedVec;

      ////PHYSICS BULLETS TESTING
      //rb.AddForce(bulletSpeedVec); //kk testing without using force but by modifying position manually on Update()

      EnemyCollider ec = bulletObj.AddComponent<EnemyCollider>();
      ec.callback = col => {
         OnBulletCollide(bulletObj, col);
      };
   }

   /*void OnGUI() {
      if (GUILayout.Button(" Initlized server")) {
         Network.InitializeServer(32,25001,false);
         Debug.Log("Server has been Initlized");
      }
      if (GUILayout.Button("connect to server")) {
         Network.Connect("127.0.0.1",25001);
      }
   }

   void OnConnectedToServer() {
      Debug.Log("Connected to server");
      // Send local player name to server ...
   }*/

}


public class EnemyCollider : MonoBehaviour {
	public delegate void CallBack(Collision col);
   public CallBack callback;

   void OnCollisionEnter(Collision col) {
      callback(col);
   }
}
