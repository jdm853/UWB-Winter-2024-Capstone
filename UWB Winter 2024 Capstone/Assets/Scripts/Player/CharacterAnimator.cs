using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private const float animationDampTime = .1f;
    private float speedPercent;

    private Animator animator;
    private ThirdPersonMovementController movement;
    private CharacterController characterController;
    
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        movement = GetComponent<ThirdPersonMovementController>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        speedPercent = characterController.velocity.magnitude / movement.GetMaxMovementSpeed();
        //Debug.Log(speedPercent);
        animator.SetFloat("speedPercent", speedPercent, animationDampTime, Time.deltaTime);
    }
}
