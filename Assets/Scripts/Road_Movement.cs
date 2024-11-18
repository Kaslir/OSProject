using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class Road_Movement : MonoBehaviour

{

    public Renderer meshRenderer;

    public float speed = 1f;



    // Update is called once per frame

    void Update()

    {

        meshRenderer.material.mainTextureOffset += new Vector2(0, speed * Time.deltaTime);

    }

}