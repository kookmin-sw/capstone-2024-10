using UnityEngine;
using System.Collections;

public class AnkleGrabberExample : MonoBehaviour {
	private Animator anim;
	int IdleOne;
	int IdleAlert;
	int Sleeps;
	int AngryReaction;
	int Hit;
	int AnkleBite;
	int CrochBite;
	int Dies;
	int HushLittleBaby;
	int Run;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		IdleOne = Animator.StringToHash("IdleOne");
		IdleAlert = Animator.StringToHash("IdleAlert");
		Sleeps = Animator.StringToHash("Sleeps");
		AngryReaction = Animator.StringToHash("AngryReaction");
		Hit = Animator.StringToHash("Hit");
		AnkleBite = Animator.StringToHash("AnkleBite");
		CrochBite = Animator.StringToHash("CrochBite");
		Dies = Animator.StringToHash("Dies");
		HushLittleBaby = Animator.StringToHash("HushLittleBaby");
		Hit = Animator.StringToHash("Hit");
		Run = Animator.StringToHash("Run");

	
	}

	// Update is called once per frame
	void Update () {
		     if    (Input.GetKeyDown(KeyCode.Y)) {
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("IdleOne")) {  
				anim.SetBool (IdleAlert, true); 
				anim.SetBool (IdleOne, false);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyUp(KeyCode.Y)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("IdleAlert")) {            //y to alert
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, true);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyDown(KeyCode.T)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("IdleOne")) {
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, false);                                      
				anim.SetBool (Sleeps, true);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyUp(KeyCode.T)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Sleeps")) {          //T to Sleeps
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, true);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
		    }
		} else if (Input.GetKeyDown(KeyCode.E)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("IdleOne")) {
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, false);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, true);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);;
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyUp(KeyCode.E)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("AngryReaction")) {            //E to angry react
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, true);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyDown(KeyCode.U)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("IdleOne")) {
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, false);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, true);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyUp(KeyCode.U)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Hit")) {                //u to hit
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, true);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyDown(KeyCode.Q)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("IdleOne")) {
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, false);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, true);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyUp(KeyCode.Q)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("AnkleBite")) {              //Q to ankle bite
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, true);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyDown(KeyCode.W)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("IdleOne")) {
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, false);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, true); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyUp(KeyCode.W)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("CrochBite")) {          //W to croch bite
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, true);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyDown(KeyCode.O)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("IdleOne")) {                        
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, false);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, true);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyUp(KeyCode.O)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Dies")) {                        //O to simple die
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, true);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
		}
		} else if (Input.GetKeyDown(KeyCode.I)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("IdleOne")) {                        
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, false);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, true);
				anim.SetBool (Run, false);  
			}
		} else if (Input.GetKeyUp(KeyCode.I)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("HushLittleBaby")) {                        //I to hush
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, true);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false); 
			}
		} else if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("IdleOne")) {                        
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, false);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, true); 
			}
		} else if (Input.GetKeyUp(KeyCode.R) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow)) {
			if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Run")) {                        //R to Run
				anim.SetBool (IdleAlert, false); 
				anim.SetBool (IdleOne, true);                                      
				anim.SetBool (Sleeps, false);
				anim.SetBool (AngryReaction, false);
				anim.SetBool (Hit, false);  
				anim.SetBool (AnkleBite, false);  
				anim.SetBool (CrochBite, false); 
				anim.SetBool (Dies, false);
				anim.SetBool (HushLittleBaby, false);
				anim.SetBool (Run, false);  
			}

	     } 
	}
}