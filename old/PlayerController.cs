using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
   public GameObject self;
   public float speed;
   public float jumpLeft;
   public float bulletSpeed; //100-300 = slow. 1000=fast
   //public Texture2D crosshairImage;
   //public GUITexture crosshairImage;
   public float frameCounter;
   public float frameSinceFire;
   //private Rigidbody rb;

   public List<GameObject> enemies;

	// Use this for initialization
	void Start () {
      //enemies = new List<GameObject>();

      //GameObject g =

      frameCounter = 0;
      //Screen.showCursor = false;
      //UnityEngine.Cursor.visible = false;
      Screen.lockCursor = true;
	}

	// Update is called once per frame
	void FixedUpdate () {
      frameCounter += 1;
      frameSinceFire += 1;
      //else rb1.useGravity = false;
      float currJump = 0.0f;
      if (jumpLeft > 0.0f) {
         currJump = 0.15f;
         jumpLeft -= 0.15f;
      }

      Vector3 move = new Vector3(0, currJump, 0);
      transform.position += move;
   }

   void onGUI() {
     //float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
     //float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
     //GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), new Texture2D(100, 100)); //crosshairImage);
      //Rect position = Rect((Screen.width - crosshairImage.width) / 2, (Screen.height - corsshairImage.height) / 2,
      //Rect pos = new Rect(0, 0, 500, 500);
      //GUI.DrawTexture(pos, crosshairImage);

   }

   void leftClick() {
      var bulletType = PrimitiveType.Capsule;
      var bulletObj = GameObject.CreatePrimitive(bulletType);

      Transform cam = transform.Find("Main Camera");

      //Vector3 bulletPos = transform.position;
      Vector3 bulletPos = cam.position;
      //bulletPos.z -= 0.28f;
      //bulletPos.y += 0.1f;
      //bulletPos.x += 3.0f;
      bulletObj.transform.position = bulletPos;

      bulletObj.transform.eulerAngles = new Vector3(0, 0, 90) + cam.eulerAngles; //+ transform.eulerAngles;
      bulletObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);


      Rigidbody rb = bulletObj.AddComponent<Rigidbody>();
      rb.useGravity = false;
      Vector3 bulletSpeedVec = new Vector3(-1.0f*bulletSpeed, 0.0f, 0.0f);

      bulletSpeedVec = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * bulletSpeedVec;
      bulletSpeedVec = Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.right) * bulletSpeedVec;

      rb.AddForce(bulletSpeedVec);

      EnemyCollider ec = bulletObj.AddComponent<EnemyCollider>();
      ec.callback = col => {
         OnCollide(bulletObj, col);
      };
   }

   void OnCollide(GameObject bullet, Collision col) {
      if (col.gameObject.name != "Ground")
            Destroy(col.gameObject);
   }

   void Update() {
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

      Transform ak = transform.Find("AK MESH");
      Transform cam = transform.Find("Main Camera");

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
      float currJump = 0.0f;/*
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
}


public class EnemyCollider : MonoBehaviour {
	public delegate void CallBack(Collision col);
   public CallBack callback;

   void OnCollisionEnter(Collision col) {
      callback(col);
   }
}
