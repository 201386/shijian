using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Playercontroler))]//角色控制器脚本
public class Player : shanghai
{
    public float moveSpeed=5;
    private Vector3 inputMove;
    private Vector3 moveVelocity;
    
    Playercontroler controller;
  protected override void Start()
    {
        base.Start();
        //base.startingHealth = 10;
        controller = GetComponent<Playercontroler>();
    }
    void Update()
    {
        #region 玩家移动
        inputMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveVelocity = moveSpeed * inputMove;
        controller.Move(moveVelocity);
        #endregion
    }
}
