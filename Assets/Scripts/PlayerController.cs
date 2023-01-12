using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

[RequireComponent(typeof(Rigidbody2D),typeof(TouchingDirection),typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    
    public float jumpImpulse = 10f;
    public float walkSpeed = 5f;
    public float airWalkSpeed = 3f;
    public float runSpeed = 8f;
    Vector2 moveInput;
    TouchingDirection touchingDirections;
    Damageable damageable;
    public float CurrentMoveSpeed 
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    if (touchingDirections.IsGrounded)
                    {
                        if (IsRunning)
                        {
                            return runSpeed;
                        }
                        else
                        {
                            return walkSpeed;
                        }
                    }

                    else
                    {
                        return airWalkSpeed;
                    }
                }
                else
                {
                    return 0;//idle speed is zero
                }

            }else
            {
                return 0; // movement lock
            }
        }
    }

    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving
    { 
        get 
        { 
            return _isMoving;
        } 
        private set 
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }
    [SerializeField]
    private bool _isRunning = false;
    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.inRunning, value);
        }
    }
    
    public bool _isFacingRight = true;
    public bool IsFacingRight {
        get
        {
            return _isFacingRight;
        }
        private set 
        {
            if(_isFacingRight !=value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        } 
    }
    
    public bool IsAlive 
    {
        get
        { 
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }

    Animator animator;
    Rigidbody2D rb;
    
    public bool CanMove
    { 
        get 
        { 
            return animator.GetBool(AnimationStrings.canMove);
        } 
    }

  

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirection>();
        damageable = GetComponent<Damageable>();
    }

   
    private void FixedUpdate()
    {
        if (!damageable.LockVelocity)
          rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

        animator.SetFloat(AnimationStrings.yVelocity,rb.velocity.y);
        
    }
    /*public void MouseDir()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (Input.GetMouseButtonDown(0))
        {         
            if (mousePos.x > 0 && !IsFacingRight  )
            {
                IsFacingRight = true;
                
            }
            else if (mousePos.x < 0 && IsFacingRight)
            {
                IsFacingRight = false;
                
            }

        }
    }*/
    public void FacingNull()
    {
        IsFacingRight = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);

        }else
        {
            IsMoving=false;
        }
        
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true; // face right
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;// face left
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }else if (context.canceled)
        {
            IsRunning = false;
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
     
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started )
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
            
        }

    }
    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.rangedAttackTrigger);

        }

    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
   

}