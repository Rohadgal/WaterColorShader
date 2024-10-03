using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveTowards : MonoBehaviour
{
    public GameObject target;

    Vector3 _targetPos;

    void Start(){
        _targetPos = target.transform.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, Time.deltaTime * 5f);
    }

    private void OnTriggerEnter(Collider other){
        Renderer renderer = other.GetComponent<Renderer>();
        //int i = renderer.material.shader.FindPropertyIndex("_Color1");
        if (other.gameObject.tag == "Player") {
            renderer.material.SetColor("_Color2", Color.blue);
            Debug.Log("Collision");
            return;
        }
        renderer.material.SetColor("_Color2", Color.yellow);
        Debug.Log("Collision");
    }
}
