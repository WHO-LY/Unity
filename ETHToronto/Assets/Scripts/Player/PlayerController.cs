using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

public enum GameState{FreeRoam, Dialog}


public class PlayerController : NetworkBehaviour
{

    public event Action OnEncountered;

    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;

    private bool isMoving;
    private Vector2 input;

    private Animator animator;
    GameState state;


    private void Awake(){
        animator = GetComponent<Animator>();
    }
    private void Start(){
        DialogManager.Instance.OnShowDialog += () => {
            state = GameState.Dialog;
        };
        DialogManager.Instance.OnCloseDialog += () => {
            if (state== GameState.Dialog) state = GameState.FreeRoam;
        };


    }

    // Update is called once per frame
    public void Update()
    {   
        if (!IsOwner) return;

        if (state == GameState.FreeRoam){
            if (Input.GetKeyDown(KeyCode.Z)){
                Interact();
            }
            if (!isMoving){
                input.x = Input.GetAxisRaw("Horizontal");
                input.y = Input.GetAxisRaw("Vertical");

                if (input.x != 0) input.y = 0;

                if (input != Vector2.zero){
                    animator.SetFloat("moveX", input.x);
                    animator.SetFloat("moveY", input.y);

                    var targetPos = transform.position;
                    targetPos.x += input.x;
                    targetPos.y += input.y;
                    if (IsWalkable(targetPos)){
                        StartCoroutine(Move(targetPos)); 
                    }
                }
            }
        }
        else if (state == GameState.Dialog){
            DialogManager.Instance.HandleUpdate();
        }
        animator.SetBool("isMoving", isMoving);

        
    }

    void Interact(){
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);

        if(collider != null){
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    IEnumerator Move(Vector3 targetPos){
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon){
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
    }

    private bool IsWalkable(Vector3 targetPos){
        if(Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer | interactableLayer) != null){
            return false;
        } return true;
    }
}
