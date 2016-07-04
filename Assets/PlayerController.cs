using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
   public GameObject self;
   public float speed;
   public float bulletLife;
   public float jumpLeft;
   public float bulletSpeed; //100-300 = slow. 1000=fast, 5000=almost cs speed
   public float frameCounter;
   public float frameSinceFire;

   public List<GameObject> enemies;
   public List<GameObject> bullets;

   private float timeLastBulletShot;
   public int numEnemies;

	// Use this for initialization
	void Start () {

      timeLastBulletShot = 0;
      frameCounter = 0;

      bullets = new List<GameObject>();
      enemies = new List<GameObject>();

      initEnemies();

      //Screen.showCursor = false;
      //UnityEngine.Cursor.visible = false;
      //Screen.lockCursor = true;
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
	}

   void Update() {
      destroyOldBullets();

      frameCounter += 1;
      frameSinceFire += 1;

      float currJump = 0.0f;
      if (jumpLeft > 0.0f) {
         currJump = 0.15f;
         jumpLeft -= 0.15f;
      }
      //else rb1.useGravity = false;
      Vector3 move_jump = new Vector3(0, currJump, 0);
      transform.position += move_jump;

      //Input.GetMouseButtonDown(0)) //Input.GetButtonDown("Fire1")
      if (Input.GetButton("Fire1") && frameSinceFire > 10) {
         leftClick();
         frameSinceFire = 0;
      }

      //onGUI();
      if (transform.position.y > 2.9f)
         self.AddComponent<Rigidbody>();
      else if (transform.position.y < 1.64f)
         Destroy(self.GetComponent<Rigidbody>());

      float lookUp = -Input.GetAxis("Mouse Y");
      float lookAround = Input.GetAxis("Mouse X")*1.0f;
      //self.transform.rotation.y += rotate;
      //self.transform.rotation.x += lookUp;

      //Transform ak = transform.Find("AK MESH");
      //Transform cam = transform.Find("Main Camera");

      //cam.eulerAngles += new Vector3(lookUp, lookAround, 0);
      //ak.eulerAngles += new Vector3(0, lookAround, lookUp);
      //transform.eulerAngles += new Vector3(lookUp, lookAround, 0);
      transform.eulerAngles += new Vector3(0, lookAround, lookUp);

      //Quaternion rotCam = Quaternion.Euler(lookUp + cam.rotation.x, lookAround + cam.rotation.y, cam.rotation.z);
      //Quaternion rotAK = Quaternion.Euler(lookUp + ak.rotation.x, lookAround + ak.rotation.y, ak.rotation.z);

      //cam.rotation = rotCam; //Quaternion.Slerp(cam.rotation, rotCam, Time.deltaTime * 2.0f);
      //ak.rotation = Quaternion.Slerp(ak.rotation, rotAK, Time.deltaTime * 2.0f);


      //START FIXED UPDATE

      float moveHoriz = Input.GetAxis("Horizontal");
      float moveVert = -Input.GetAxis("Vertical");
      if (Input.GetKeyDown("space")) jumpLeft += 1.5f;
      //float currJump = 0.0f;
      /*
      if (jumpLeft > 0.0f) {
         currJump = 0.05f;
         jumpLeft -= 0.05f;
      }*/

      Vector3 move = new Vector3(moveVert, currJump, moveHoriz); //new Vector3(moveHoriz, jump, moveVert);
      move = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * move;

      //rb.AddForce(move * speed);
      transform.position += move*speed;
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

	void FixedUpdate() { }

   void OnBulletCollide(GameObject bullet, Collision col) {
      if (col.gameObject.name != "Ground")
            Destroy(col.gameObject);
   }

   void destroyOldBullets() {
      if (bullets.Count > 0 && (Mathf.Abs(Time.time - timeLastBulletShot) > bulletLife)) {
         foreach (GameObject b in bullets)
            Destroy(b);
         bullets.Clear();
      }
   }

   void leftClick() {
      timeLastBulletShot = Time.time;
      var bulletObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
      bullets.Add(bulletObj);

      Transform cam = transform.Find("Main Camera");

      bulletObj.transform.eulerAngles = new Vector3(0, 90, 90) + cam.eulerAngles;
      bulletObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

      //Vector3 bulletPos = transform.position;
      Vector3 bulletPos = cam.position;
      //bulletPos.z -= 0.28f;
      //bulletPos.y += 0.1f;
      //bulletPos.x += 3.0f;
      bulletObj.transform.position = bulletPos;

      //bulletObj.transform.eulerAngles = new Vector3(0, 0, 90) + cam.eulerAngles; //+ transform.eulerAngles;

      Rigidbody rb = bulletObj.AddComponent<Rigidbody>();
      rb.useGravity = false;
      /*Vector3 bulletSpeedVec = new Vector3(-1.0f*bulletSpeed, 0.0f, 0.0f);

      bulletSpeedVec = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * bulletSpeedVec;
      bulletSpeedVec = Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.right) * bulletSpeedVec;
      bulletSpeedVec = Quaternion.AngleAxis(transform.eulerAngles.x, Vector3.back) * bulletSpeedVec;*/
      //Vector3 bulletSpeedVec = new Vector3(0, 0, 90); //* cam.eulerAngles; //cam.rotation * Vector3.forward; //cam.rotation; //cam.eulerAngles;

      /*bulletSpeedVec = Quaternion.AngleAxis(cam.eulerAngles.y, Vector3.up) * bulletSpeedVec;
      bulletSpeedVec = Quaternion.AngleAxis(cam.eulerAngles.z, Vector3.right) * bulletSpeedVec;*/

      Vector3 bulletSpeedVec = new Vector3(-1, 0, 0) * bulletSpeed;
      rb.AddForce(bulletSpeedVec);

      EnemyCollider ec = bulletObj.AddComponent<EnemyCollider>();
      ec.callback = col => {
         OnBulletCollide(bulletObj, col);
      };
   }

}


public class EnemyCollider : MonoBehaviour {
	public delegate void CallBack(Collision col);
   public CallBack callback;

   void OnCollisionEnter(Collision col) {
      callback(col);
   }
}
