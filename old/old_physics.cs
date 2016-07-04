      //Input.GetMouseButtonDown(0)) //Input.GetButtonDown("Fire1")
      //self.transform.rotation.y += rotate;
      //self.transform.rotation.x += lookUp;

      //Transform ak = transform.Find("AK MESH");
      //Transform cam = transform.Find("Main Camera");

      //cam.eulerAngles += new Vector3(lookUp, lookAround, 0);
      //ak.eulerAngles += new Vector3(0, lookAround, lookUp);
      //transform.eulerAngles += new Vector3(lookUp, lookAround, 0);

      //Quaternion rotCam = Quaternion.Euler(lookUp + cam.rotation.x, lookAround + cam.rotation.y, cam.rotation.z);
      //Quaternion rotAK = Quaternion.Euler(lookUp + ak.rotation.x, lookAround + ak.rotation.y, ak.rotation.z);

      //cam.rotation = rotCam; //Quaternion.Slerp(cam.rotation, rotCam, Time.deltaTime * 2.0f);
      //ak.rotation = Quaternion.Slerp(ak.rotation, rotAK, Time.deltaTime * 2.0f);


      /*if (jumpLeft > 0.0f) {
         currJump = 0.05f;
         jumpLeft -= 0.05f;
      }*/

 //new Vector3(moveHoriz, jump, moveVert);

         /*foreach (var v in bulletAngles)
            Destroy(v);*/


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

