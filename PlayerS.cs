using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using Leap;  // For Leap Motion classes


public class PlayerS : MonoBehaviour
{
   private float moveSpeed = 50f;
   private float jumpForce = 20f;
   private int jumpCount = 0;
   private int maxJumps = 2;
   private bool canJump = true;  // Add debounce for jumping
   private bool inSignZone = false;


   private Rigidbody2D rb;
   private LeapProvider leapProvider;
   private HandPoseDetector poseDetector;


   void Start()
   {
       rb = GetComponent<Rigidbody2D>();
       leapProvider = FindObjectOfType<LeapServiceProvider>();
    //    poseDetector = gameObject.AddComponent<HandPoseDetector>();
    //    SetupASignPose();
   }

    // void SetupASignPose()
    // {
    //     var aSignPose = ScriptableObject.CreateInstance<HandPoseScriptableObject>();
    //     aSignPose.name = "ASign";

    //     // Define rules for "A" sign
    //     var rules = new HandPoseDetector.PoseRule[]
    //     {
    //         // Thumb should be extended
    //         new HandPoseDetector.PoseRule{
    //             finger = Finger.FingerType.Thumb,
    //             direction = HandPoseDetector.RuleDirection.Extended
    //         },
    //         // All other fingers should be closed
    //         new HandPoseDetector.PoseRule{
    //             finger = Finger.FingerType.Index,
    //             direction = HandPoseDetector.RuleDirection.Closed
    //         },
    //         new HandPoseDetector.PoseRule{
    //             finger = Finger.FingerType.Middle,
    //             direction = HandPoseDetector.RuleDirection.Closed
    //         },
    //         new HandPoseDetector.PoseRule{
    //             finger = Finger.FingerType.Ring,
    //             direction = HandPoseDetector.RuleDirection.Closed
    //         },
    //         new HandPoseDetector.PoseRule{
    //             finger = Finger.FingerType.Pinky,
    //             direction = HandPoseDetector.RuleDirection.Closed
    //         }
    //     };

    //     poseDetector.poses.Add(aSignPose);
    // }


   void OnCollisionEnter2D(Collision2D collision)
   {
    if (collision.gameObject.CompareTag("Ground")) {
        jumpCount = 0;  
    }
       
   }
   void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SignZone"))
        {
            inSignZone = true;
            Debug.Log("Entered sign detection zone!");
        }
    }
     void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SignZone"))
        {
            inSignZone = false;
            Debug.Log("Left sign detection zone!");
        }
    }


   void Update()
   {
     
       Frame frame = leapProvider.CurrentFrame;
       if (frame.Hands.Count > 0)
       {
           Hand hand = frame.Hands[0];
           float moveDirection = hand.PalmPosition.x;
           rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
          
           // Only jump when pinch starts and we have jumps left
           if (hand.PinchStrength > 0.8f && jumpCount < maxJumps && canJump)
           {
               rb.velocity = new Vector2(rb.velocity.x, 0); // Zero out y velocity
               rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
             
               jumpCount++;
               canJump = false;  // Prevent multiple jumps from one pinch
           }
           else if (hand.PinchStrength < 0.3f)
           {
               canJump = true;  // Allow another jump when pinch is released
           }
           if (inSignZone)
            {
                CheckForASign(hand);
            }
       }
   }

   void CheckForASign(Hand hand)
    {
   

        // Method 2: Direct checking (backup method)
        if (hand.GrabStrength > 0.8f && 
            hand.Thumb.IsExtended &&
            !hand.Index.IsExtended &&
            !hand.Middle.IsExtended &&
            !hand.Ring.IsExtended &&
            !hand.Pinky.IsExtended)
        {
            OnASignDetected();
        }
    }

    void OnASignDetected() {
        Debug.Log("'A' sign detected! Opening door...");
    }
}
