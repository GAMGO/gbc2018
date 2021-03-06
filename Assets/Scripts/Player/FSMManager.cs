﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerState
{
    IDLE = 0,
    RUN,
    CHASE,
    ATTACK,
    DEAD
}
public class FSMManager: MonoBehaviour,IFSMManager {

    public PlayerState currentState;
    public PlayerState startState;
    public Transform marker;
    public Animator ani;
    public CharacterStat stat;
    public CharacterController cc;
    public int layerMask;
    public Transform target;

    Dictionary<PlayerState, PlayerFSMState> states = new Dictionary<PlayerState, PlayerFSMState>();

    private void Awake()
    {
        layerMask = (1 << 9) + (1 << 10);
        marker = GameObject.FindGameObjectWithTag("Marker").transform;
        ani = GetComponentInChildren<Animator>();
        stat = GetComponent<CharacterStat>();
        states.Add(PlayerState.IDLE,GetComponent<PlayerIDLE>());
        states.Add(PlayerState.RUN,GetComponent<PlayerRUN>());
        states.Add(PlayerState.CHASE,GetComponent<PlayerCHASE>());
        states.Add(PlayerState.ATTACK,GetComponent<PlayerATTACK>());
        states.Add(PlayerState.DEAD,GetComponent<PlayerDEAD>());
        cc = GetComponent < CharacterController >();
    }
    public void SetState(PlayerState newState)
    {
        if (currentState == PlayerState.DEAD)
        {
            return;
        }
        //too long to type all States So, we should make it simple.
        //Initialization All the States with 'foreach' code.
        foreach (PlayerFSMState fsm in states.Values)
        {
            fsm.enabled = false;
        }
        //Start newState.
        states[newState].enabled = true;
        states[newState].BeginState();
        currentState = newState;
        ani.SetInteger("CurrentState",(int)currentState);
    }
    void Start () {
        SetState(startState);
        marker.gameObject.SetActive(false);
    }
    void Update()
    {
        if (currentState == PlayerState.DEAD)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000, layerMask))
            {
                marker.gameObject.SetActive(true);
                if (hit.transform.gameObject.layer == 9)
                {
                    marker.position = hit.point;
                    /*Initialization All the States in new veriable and change PlayerStates to Suitable State.*/
                    SetState(PlayerState.RUN);
                    target = null;
                    foreach (Renderer ren in marker.GetComponentsInChildren<Renderer>())
                    {
                        ren.material.color = Color.white;
                    }
                    marker.parent = null;
                    marker.localScale = Vector3.one * 0.3f;
                }
                else if (hit.transform.gameObject.layer == 10)
                {
                    target = hit.transform;
                    SetState(PlayerState.CHASE);

                    marker.parent = target;
                    marker.localPosition = Vector3.zero;
                    marker.localScale = Vector3.one;
                    foreach (Renderer ren in marker.GetComponentsInChildren<Renderer>())
                    {
                        ren.material.color = Color.red;
                    }
                }

            }//else if(){}
        }
    }
    public void AttackCheck()
    {
        if(target == null)
        {
            return;
        }
        SlimeStat targetStat = target.GetComponent<SlimeStat>();
        targetStat.ApplyDamage(stat);
    }

    public void SetDead()
    {

        SetState(PlayerState.DEAD);
        cc.enabled = false;
        marker.gameObject.SetActive(false);
    }

    public void NotifyDead()
    {
        SetState(PlayerState.IDLE);
    }
}