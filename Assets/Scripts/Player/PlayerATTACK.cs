﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerATTACK : PlayerFSMState {
    public float delayTime = 1f;
    public float waitTime = 0;
    public override void BeginState()
    {
        base.BeginState();
        waitTime = 0;
    }
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("ATTACK");
        
        if (Vector3.Distance(transform.position, manager.target.position)
            > manager.stat.attackR)
        {
            manager.SetState(PlayerState.CHASE);
            return;
        }
        GameLib.JJRotate(transform, manager.target.position, manager.stat);
        Vector3 diff = manager.target.position - transform.position;
        diff.y = 0.0f;
        if (diff.magnitude > manager.stat.attackR)
        {   waitTime+= Time.deltaTime;
            if(waitTime > delayTime) {
                manager.SetState(PlayerState.CHASE);
            }
        }
    }
}
