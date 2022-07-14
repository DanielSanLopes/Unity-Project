using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;
using Cinemachine;

public class DanielScript : MonoBehaviour
{
    //private CharacterController characControl;
    private Rigidbody2D thisRigid;
    private static Animator thisAnimator;
    private static Transform thisTrans;
    private Collider2D thisColli;
    private static Vector2 jumpOrigin;
    private static Vector3 scaleOrigin;
    private Camera cam;

    private Vector3 ballPositionWhenGrabed = new Vector3 (1.84f, 0.07f, -0.14f);
    private Vector3 ballOriginalPosition = new Vector3(1.461f, 0.082f, 0);
    private Quaternion ballOriginalLocalRotation = new Quaternion(0, 0, 0.0808105f, 1);
    private Vector3 ballOriginalLocalScale = new Vector3(1, 1, 1);

    private List<string> currentCollsStrings = new List<string>();
       

    public bool isPC;


    public GameObject ShootPrefab;
    public Vector3 mousePosition;
    public Vector3 mousePosition2D;
    public Transform bone0;
    public Transform originalBoneParent;
    public GameObject objectWithRender;
    public Transform originalPositionOfBone4;
    public Transform tempParentOfBone4;
    public Transform boneTemp;
    public Transform ballTrans;
    public Vector3 originalPositionOfBall;
    public float runForce;
    public float jumpForce;
    public Animator ballAnimator;
    public float timeCount;
    public bool shouldCount;
    public CinemachineVirtualCamera vCam;
    private CinemachineFramingTransposer vCamTransposer;


    // Start is called before the first frame update
    void Start(){
        thisAnimator = this.GetComponent<Animator>();
        thisTrans = this.GetComponent<Transform>();
        thisRigid = this.GetComponent<Rigidbody2D>();
        thisColli = this.GetComponent<Collider2D>();
        cam = Camera.main;
        jumpOrigin = Vector2.zero;
        vCamTransposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();

        
        
        
        
    }

    // Update is called once per frame
    void Update() {



        print(ballTrans.localRotation);

        mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
        //print("Posição Mouse" + mousePosition);

        //Se o tempo do pressionar do mouse deve ser contato
        if (shouldCount)
            timeCount += Time.deltaTime; //Inicia a contagem de tempo desde o último acionamento do tiro
        else
            timeCount = 0; //Para a contagem de tempo

        if ((boneTemp.localScale.x < 0 && thisTrans.localScale.x > 0) || (boneTemp.localScale.x > 0 && thisTrans.localScale.x < 0))
            boneTemp.localScale = new Vector3(boneTemp.localScale.x * -1, boneTemp.localScale.y, boneTemp.localScale.z);

        camReposition(thisTrans, thisAnimator);

        if (thisAnimator.GetBool("grabWall")) {


            //ballTrans.localPosition = new Vector3(0.131f, 0.09f, -0.113f);
            if (mousePosition2D.y < (thisTrans.position.y - 0.78f)) {
                if (thisTrans.localScale.x > 0) {
                    if (mousePosition2D.x > (thisTrans.position.x + 3f)) {
                        print("Looking...");
                        boneTemp.LookAt(mousePosition);
                        boneTemp.rotation = new Quaternion(0, 0, boneTemp.rotation.x * -boneTemp.rotation.y, boneTemp.rotation.w * (Vector2.Distance(thisTrans.position, mousePosition2D) / 14));
                        //boneTemp.eulerAngles = new Vector3(boneTemp.eulerAngles.x, 0, 0);

                    }
                } else {

                    // boneTemp.localScale = new Vector3(boneTemp.localScale.x * -1, boneTemp.localScale.y, boneTemp.localScale.z);



                    if (mousePosition2D.x < (thisTrans.position.x - 3f)) {



                        boneTemp.LookAt(mousePosition);
                        if (boneTemp.rotation.x > 170 && boneTemp.rotation.x < 181)
                            boneTemp.rotation = new Quaternion(0, 180, boneTemp.rotation.x * boneTemp.rotation.y, boneTemp.rotation.w * (Vector2.Distance(thisTrans.position, mousePosition2D) / 14));
                        else {
                            boneTemp.rotation = new Quaternion(boneTemp.rotation.x, 0, boneTemp.rotation.x * boneTemp.rotation.y, boneTemp.rotation.w * (Vector2.Distance(thisTrans.position, mousePosition2D) / 14));
                            boneTemp.eulerAngles = new Vector3(0, 180, -boneTemp.eulerAngles.z);
                        }
                    }

                }
            }
        }

        if (thisAnimator.GetBool("grabWall")) {

            thisAnimator.SetBool("run", false);

            if (scaleOrigin == thisTrans.localScale)
                Flip();
        }


        //Se passaram-se X tempo desde o acionamento do tiro sem que fosse descionado, disparar o supertiro 
        //(segurar pra disparar)
        if (timeCount >= 0.9f)
            SuperFire();


        //Se o Axis de Jump for detectado, pular
        if (Input.GetAxis("Jump") > 0) {
            Jump();
        }

        //Se a velocidade vertical for menor que 0.001(início de queda), e o player não estiver no meio de um salto, tocar animação de queda.
        if (thisRigid.velocity.y < 0.001f && thisAnimator.GetBool("jump"))
            Fall();


        //while (thisAnimator.GetBool("grabWall"))
        //    thisAnimator.SetBool("superShooting", false);



        //Se o axis Horizontal estiver sendo usado, correr.
        if (Input.GetAxis("Horizontal") != 0) {

            Run();
            //Se, durante a utilização do Axis e o valor deste for menor que 0 enquanto o scale do player é maior,
            //ou menor enquanto o scale for maior, significa que o Input é contrário à direção atual para o qual o
            //player está apontando. Portanto, virar
            if (thisAnimator.GetBool("grabWall") == false && ((Input.GetAxis("Horizontal") < 0 && thisTrans.localScale.x > 0) || (Input.GetAxis("Horizontal") > 0 && thisTrans.localScale.x < 0)))
                Flip();
            else
                return;


        } else //Caso o Axis não esteja sendo utilizado, parar de correr.
            runToFalse();



        if (Input.GetMouseButtonDown(0) && isPC) {
            OnInput();

        }

        if (Input.GetMouseButtonUp(0) && isPC) {
            OnRelease();

        }

      

        tempParentOfBone4.position = new Vector3(cam.transform.position.x, cam.transform.position.y + 2000, cam.transform.position.z);

        if (currentCollsStrings.Contains("ground") || currentCollsStrings.Contains("enemy")) {
            thisAnimator.SetBool("grounded", true);
            fallToFalse();
        }
            
        else
            thisAnimator.SetBool("grounded", false);

        
        
    }

    private void FixedUpdate() {

        if (this.transform.position.y > jumpOrigin.y + 2.5f) {
            //this.transform.position = new Vector2(this.transform.position.x, jumpOrigin.y +2.75f);

            thisRigid.AddForce(Vector2.down * (jumpForce * 0.1f), ForceMode2D.Impulse);
        }

        
    }

    private void LateUpdate() {
      

    }

    //ballTrans.localPosition = new Vector3(1.469f, -0.627f, 0);

    private void OnCollisionEnter2D(Collision2D collision) {

        
        currentCollsStrings.Add(collision.collider.tag);

        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "enemy") {
            fallToFalse();
            grabWallToFalse();
            bonesReposition();
        }
            

        if(collision.gameObject.tag == "wall") {

            

            scaleOrigin = thisTrans.localScale;
            if ((thisTrans.position.y > (collision.collider.bounds.center.y - collision.collider.bounds.extents.y)) && (thisTrans.position.y < (collision.collider.bounds.center.y + collision.collider.bounds.extents.y))) {

                


                
                
                //ballTrans.position = new Vector3(0.131f, 0.09f, -0.113f);
                //ballTrans.rotation = new Quaternion (-6.579f, 90, -102.514f, 0);
                //boneTemp.rotation = new Quaternion(77.941f, -90, 0, 0);

                //Flip();
                fallToFalse();
                jumpToFalse();
                thisAnimator.SetBool("grabWall", true);
                StopRigidMovement();
                thisRigid.bodyType = RigidbodyType2D.Static;


                //ballOriginalLocalRotation = ballTrans.rotation;

                objectWithRender.SetActive(true);
                boneTemp.gameObject.SetActive(true);
                originalPositionOfBall = ballTrans.position;
                ballTrans.SetParent(boneTemp.GetChild(0));
                ballTrans.localPosition = ballPositionWhenGrabed;
                originalBoneParent.SetParent(tempParentOfBone4);
               

               


                //if (thisTrans.localScale.x < 0)
                //    boneTemp.localRotation = new Quaternion(0, 180, 360, boneTemp.rotation.w);
            } 

        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag == "ground") {
            fallToFalse();
            
        }
            
            
    }

    private void OnCollisionExit2D(Collision2D collision) {
        currentCollsStrings.Clear();
        //if (collision.gameObject.tag == "wall") {
        //    thisAnimator.SetBool("grabWall", false);
        //}
    }

    //Executa toda vez que o mouse é clickado ou a tela é tocada
    void OnInput() {
        Fire();
        shouldCount = true;
    } 

    //Executa toda vez que o mouse ou a tela é solta
    void OnRelease() {
        thisAnimator.SetBool("shoot", false);
        ballAnimator.SetBool("shoot", false);
        shouldCount = false;
        
    } 
    
    //Vira o personagem para a esquerda ou parafa direita
    public void Flip() {
        
        thisTrans.localScale = new Vector3 (thisTrans.localScale.x * -1, thisTrans.localScale.y, thisTrans.localScale.z);
        //if (thisAnimator.GetBool("grabWall") && thisTrans.localScale.x < 0) {
        //    boneTemp.localPosition = new Vector3(0.166f, 1.456f, 0);
        //    boneTemp.localRotation = new Quaternion(0, 180, 3.049f, boneTemp.localRotation.w);
        //    ballTrans.localPosition = new Vector3(0.005103485f, -0.01128378f, 0.01726536f);
        //    ballTrans.localRotation = new Quaternion(0, 180, 9.487f, ballTrans.localRotation.w);
        //}
    }

    public static void FlipStatic() {
        thisTrans.localScale = new Vector3(thisTrans.localScale.x * -1, thisTrans.localScale.y, thisTrans.localScale.z);
    }

    //Toca a animação de correr
    public void Run() {

        

        if (/*thisAnimator.GetBool("grabWall") == false &&*/ thisAnimator.GetBool("superShooting") == false && thisAnimator.GetBool("shoot") == false && thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("ShootGround") == false && thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("ShootGround2") == false) {
            //print("run");
            thisAnimator.SetBool("run", true);

            if (this.transform.localScale.x < 0)
                thisRigid.velocity = new Vector2(-runForce, thisRigid.velocity.y);
            //thisRigid.AddRelativeForce(Vector2.left * runForce * (Time.deltaTime + 1));
            //this.thisRigid.MovePosition(new Vector2(thisRigid.position.x - runForce, thisRigid.position.y));

            else if (this.transform.localScale.x > 0)
                thisRigid.velocity = new Vector2(runForce, thisRigid.velocity.y);
            //thisRigid.AddRelativeForce(Vector2.right * runForce * (Time.deltaTime + 1));
            //this.thisRigid.MovePosition(new Vector2(thisRigid.position.x + runForce, thisRigid.position.y));
        }
    }

    //Toca a animação de pular
    void Jump() {
        //if(thisAnimator.GetBool("grabWall"))
        //    grabWallToFalse();


        //////////////////////////////////////////////////////////////////

        ///////////////////////objectWithRender.SetActive(true);
        ///////////////////////boneTemp.GetComponentInParent<Transform>().gameObject.SetActive(true);
        ///////////////////////boneTemp.gameObject.SetActive(true);
       
        ///////////////////////originalPositionOfBall = ballTrans.position;
        ///////////////////////ballTrans.SetParent(boneTemp.GetChild(0));
        ///////////////////////ballTrans.localPosition = ballPositionWhenGrabed;
        ///////////////////////originalBoneParent.SetParent(tempParentOfBone4);
        //print(boneTemp.GetComponentInChildren<Transform>().name);

       
        


        //////////////////////////////////////////////////////////////////



        thisRigid.bodyType = RigidbodyType2D.Dynamic;
        
        //Se o player não estiver pulando e nem estiver caindo, nem atirando de nenhuma maneira, então pode pular
        if (/*thisAnimator.GetBool("grabWall") == false &&*/ jumpOrigin == Vector2.zero && thisRigid.velocity.y == 0 && thisAnimator.GetBool("jump") == false && thisAnimator.GetBool("fall") == false && thisAnimator.GetBool("superShooting") == false && thisAnimator.GetBool("shoot") == false && thisAnimator.GetBool("superShootEnd") == false) {


            //if (thisAnimator.GetBool("grabWall")==false)
            //    ballOriginalLocalRotation = ballTrans.rotation;


            bonesReposition();


            jumpOrigin = this.transform.position;
            //print("jump");
            thisRigid.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            thisAnimator.SetBool("jump", true);
            thisAnimator.SetBool("grabWall", false);
        }
    }
     
    //Toca a animação de cair
    void Fall() {
        
        thisAnimator.SetBool("fall", true);
        grabWallToFalse();
    }

    //Toca as animações do tiro comum
    void Fire() {
        if(thisRigid.velocity.y == 0) {
            thisAnimator.SetBool("shoot", true);
            ballAnimator.SetBool("shoot", true);
        }        
    }

    //Toca as animações do super tiro
    void SuperFire(){
        if (thisAnimator.GetBool("grabWall") == false && thisRigid.velocity.y == 0 && thisAnimator.GetBool("superShooting") == false) {
            thisAnimator.SetBool("superShooting", true);
            ballAnimator.SetBool("shoot", false);
            ballAnimator.SetBool("superShootTrigger", true);
        } 
            
    }

    void camReposition(Transform transform, Animator animator) {

        if (animator.GetBool("grabWall") && transform.localScale.x > 0 && transform.localScale.x != scaleOrigin.x) {
            vCamTransposer.m_SoftZoneWidth = Mathf.Lerp(vCamTransposer.m_SoftZoneWidth, 0.3f, 20f * Time.deltaTime);
            vCamTransposer.m_SoftZoneHeight = 0.8f;
            vCamTransposer.m_ScreenX = Mathf.Lerp(vCamTransposer.m_SoftZoneWidth, 0.00000000000000000000005f, 10.0f * Time.deltaTime);
            vCamTransposer.m_ScreenY = 0.47f;
        } else if (animator.GetBool("grabWall") && transform.localScale.x < 0 && transform.localScale.x != scaleOrigin.x) {
            vCamTransposer.m_SoftZoneWidth = 0.7f;
            vCamTransposer.m_SoftZoneHeight = 0.8f;
            vCamTransposer.m_ScreenX = Mathf.Lerp(vCamTransposer.m_SoftZoneWidth, 0.9f, 5.0f * Time.deltaTime);
            vCamTransposer.m_ScreenY = 0.47f;

        } else if (animator.GetBool("run") && transform.localScale.x > 0) {

            vCamTransposer.m_SoftZoneWidth = 1.0f;
            vCamTransposer.m_SoftZoneWidth = Mathf.Lerp(vCamTransposer.m_SoftZoneWidth, 0.2f, 1.0f * Time.deltaTime);
            vCamTransposer.m_SoftZoneHeight = 0.8f;
            vCamTransposer.m_ScreenX = 0.25f;

        } else if (animator.GetBool("run") && transform.localScale.x < 0) {

            vCamTransposer.m_SoftZoneWidth = 1.0f;
            vCamTransposer.m_SoftZoneWidth = Mathf.Lerp(vCamTransposer.m_SoftZoneWidth, 0.2f, 1.0f * Time.deltaTime);
            vCamTransposer.m_SoftZoneHeight = 0.8f;
            vCamTransposer.m_ScreenX = 0.75f;

        } else {

            //vCamTransposer.m_SoftZoneWidth = 0.5f;
            //vCamTransposer.m_ScreenX = 0.5f;

        }
            
            
        
    }

    void bonesReposition() {

        originalBoneParent.SetParent(bone0);
        ballTrans.SetParent(originalBoneParent.GetChild(0));
        ballTrans.localPosition = ballOriginalPosition;
        ballTrans.localRotation = ballOriginalLocalRotation;

        if (ballTrans.localScale.x > 0 && thisTrans.localScale.x < 0)
            ballTrans.localScale = new Vector3(-1, 1, 1);
        else if (ballTrans.localScale.x < 0 && thisTrans.localScale.x > 0)
            ballTrans.localScale = new Vector3(1, 1, 1);


        boneTemp.gameObject.SetActive(false);
        objectWithRender.SetActive(false);
    }

    //Toca a animação de recuo do super tiro
    public static void EndOfSuperFire() {
        thisAnimator.SetBool("superShootEnd", true);
        shootToFalse();
    } 

    public void StopRigidMovement() {
        // thisRigid.MovePosition(thisRigid.position);
        thisRigid.velocity = Vector2.zero;
    }


    public void ShootInstantiate() {
        Instantiate(ShootPrefab, ballTrans.position, new Quaternion(0, 0, 0, 0));
        
    } 


    //UI Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public void FlipToLeft() {
        if(thisTrans.localScale.x > 0)
            thisTrans.localScale = new Vector3(thisTrans.localScale.x * -1, thisTrans.localScale.y, thisTrans.localScale.z);
    }

    public void FlipToRight() {
        if (thisTrans.localScale.x < 0)
            thisTrans.localScale = new Vector3(thisTrans.localScale.x * -1, thisTrans.localScale.y, thisTrans.localScale.z);
    }



    //False Setters
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    //Seta o parâmetro "jump" como falso neste Animator
    public static void jumpToFalse() {
        thisAnimator.SetBool("jump", false);

    }

    //Seta os parâmetros "superShooting" e "superShootEnd" (neste Animator) e
    //"superShootTrigger" (Animator da EnergySphere) como falso
    void superShootEndToFalse() {
        thisAnimator.SetBool("superShootEnd", false);
        thisAnimator.SetBool("superShooting", false);
        ballAnimator.SetBool("superShootTrigger", false);
        shouldCount = false;
    }

    public void superShootingToFalse() {
        thisAnimator.SetBool("superShooting", false);
        ballAnimator.SetBool("superShootTrigger", false);
    }

    //Seta o parâmetro "shoot" como falso neste Animator
    public static void shootToFalse() {
        thisAnimator.SetBool("shoot", false);
    }

    public void shootToFalse2() {
        thisAnimator.SetBool("shoot", false);

    }

    //Seta o parâmetro "fall" como falso neste Animator
    public static void fallToFalse() {
        //print("Fall to false ");
        thisAnimator.SetBool("fall", false);
        jumpOrigin = Vector2.zero;

    }

    //Seta o parâmetro "run" como falso neste Animator
    void runToFalse() {
        thisAnimator.SetBool("run", false);
    }

    void grabWallToFalse() {
        thisAnimator.SetBool("grabWall", false);
    }

    //True setters
    //////////////////////////////////////////////////////////////////////////////////////////////

    public void runToTrue() {
        print("Run true");
        thisAnimator.SetBool("run", true);
    }
}