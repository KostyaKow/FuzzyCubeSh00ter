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
   //private List<Vector3> bulletAngles;
   private List<Quaternion> bulletAngles;

   private float timeLastBulletShot;
   public int numEnemies;

	// Use this for initialization
	void Start () {

      timeLastBulletShot = 0;
      frameCounter = 0;

      bullets = new List<GameObject>();
      bulletAngles = new List<Quaternion>(); //<Vector3>();
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
      processEveryBulletMovement(); //TODO: testing bullets without physics


      //Input.GetMouseButtonDown(0)) //Input.GetButtonDown("Fire1")
      if (Input.GetButton("Fire1") && frameSinceFire > 10) {
         leftClick();
         frameSinceFire = 0;
      }

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
      float currJump = 0.0f;
      /*if (jumpLeft > 0.0f) {
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

	void FixedUpdate() {
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


    }

   void OnBulletCollide(GameObject bullet, Collision col) {
      if (col.gameObject.name != "Ground")
            Destroy(col.gameObject);
   }

   void destroyOldBullets() {
      if (bullets.Count > 0 && (Mathf.Abs(Time.time - timeLastBulletShot) > bulletLife)) {
         foreach (GameObject b in bullets)
            Destroy(b);
         /*foreach (var v in bulletAngles)
            Destroy(v);*/
         bulletAngles.Clear();
         bullets.Clear();
      }
   }

   void processEveryBulletMovement() {
      var z = 0;
      foreach (GameObject b in bullets) {
         //Vector3 camAngle = bulletAngles[z];
         Quaternion camAngle = bulletAngles[z];

         /*Vector3 moveBullet = Quaternion.AngleAxis(camAngle.y, Vector3.up) *
                              Quaternion.AngleAxis(camAngle.z, Vector3.right) *
                              Quaternion.AngleAxis(camAngle.x, Vector3.forward) *
                              new Vector3(-1, 0.0f, 0.0f);*/
         Vector3 moveBullet = camAngle * new Vector3(0.0f, 0.0f, 1.0f);

         //Vector3 moveBullet = Quaternion.AngleAxis(camAngle.y, Vector3.);// *
                              //new Vector3(1, 0, 0);

         /*moveBullet = Quaternion.AngleAxis(camAngle.z, Vector3.right) *
                      moveBullet;*/
         /*moveBullet = Quaternion.AngleAxis(camAngle.x, Vector3.forward) *
                      moveBullet;*/


         //b.transform.position += new Vector3(0, 0, 0.1f);
         b.transform.position += moveBullet;

         z++;
      }
   }

   void leftClick() {
      Transform cam = transform.Find("Main Camera");

      timeLastBulletShot = Time.time;
      var bulletObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
      bullets.Add(bulletObj);
      bulletAngles.Add(cam.rotation);//eulerAngles);

      bulletObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
      bulletObj.transform.eulerAngles = new Vector3(0, 90, 90) + cam.eulerAngles;

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

      Vector3 bulletSpeedVec = new Vector3(0, 0, 1*bulletSpeed);
      Debug.Log(" x: " + cam.eulerAngles.x +  "y: " + cam.eulerAngles.y + "z: " + cam.eulerAngles.z);

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

}


public class EnemyCollider : MonoBehaviour {
	public delegate void CallBack(Collision col);
   public CallBack callback;

   void OnCollisionEnter(Collision col) {
      callback(col);
   }
}
