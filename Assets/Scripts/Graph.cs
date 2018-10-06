﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {
    
    public float granularidade = 1;//distância entre vértices
    public int nVertex = 10;//número de vértices no lado
    [Range(0,90)] public float maxSlope = 30;//slope máximo do vértice para ele ser andável
    [Range(0, 10)] public float maxBound = 5;
    public GameObject No;
    public GameObject[] Nos;//vetor de nos
    public Material red, green;

    void Start()
    {   //criar vértices
        Nos = new GameObject[nVertex * nVertex];
        for (int i = 0; i < nVertex; i++)
        {
            for (int j = 0; j < nVertex; j++)
            {
                RaycastHit hit;
                Vector3 posNodo = this.transform.position + new Vector3(i * granularidade, 0, j * granularidade);
                if (Physics.Raycast(posNodo, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
                {
                    GameObject newNo = Instantiate(No, hit.point, Quaternion.identity);
                    newNo.name = "v_" + (i * nVertex + j);

                    if (hit.collider.gameObject.layer == 23)//se colidir com a layer No Walk
                        newNo.GetComponent<Node>().active = false;

                    if(!isSlopeValid(hit))                  //desliga vértices muito inclinados
                        newNo.GetComponent<Node>().active = false;

                    if(isNearWall(newNo,hit))               //desliga vértices próximos de paredes
                        newNo.GetComponent<Node>().active = false;

                    Nos[i * nVertex + j] = newNo;
                }
            }
        }
    }

    void Update()
    {   //desenhar linhas
        if (Nos == null)//se o vetor não tiver sido preenchido ainda
            return;

        for (int i = 0; i < nVertex; i++)
        {
            for (int j = 0; j < nVertex; j++)
            {
                if(!Nos[j*nVertex+i].GetComponent<Node>().active) //se o nó estiver desativo
                    Nos[j * nVertex + i].GetComponent<MeshRenderer>().material = red;//fica vermelho
                else
                    Nos[j * nVertex + i].GetComponent<MeshRenderer>().material = green;

                if (i + 1 < nVertex)
                {
                    if (Nos[(j * nVertex) + (i + 1)].GetComponent<Node>().active && Nos[(j * nVertex) + (i)].GetComponent<Node>().active)
                        Debug.DrawLine(Nos[(j * nVertex) + i].transform.position, Nos[(j * nVertex) + (i + 1)].transform.position, Color.green);
                }
                if (j + 1 < nVertex)
                    if ( Nos[((j + 1) * nVertex) + (i)].GetComponent<Node>().active && Nos[(j * nVertex) + (i)].GetComponent<Node>().active)
                        Debug.DrawLine(Nos[(j * nVertex) + i].transform.position, Nos[((j + 1) * nVertex) + (i)].transform.position, Color.green);
                if (i + 1 < nVertex && j + 1 < nVertex)
                {
                    if (Nos[((j + 1) * nVertex) + (i+1)].GetComponent<Node>().active && Nos[(j * nVertex) + (i)].GetComponent<Node>().active)
                        Debug.DrawLine(Nos[(j * nVertex) + i].transform.position, Nos[((j + 1) * nVertex) + (i + 1)].transform.position, Color.green);
                    if (Nos[((j + 1) * nVertex) + (i)].GetComponent<Node>().active && Nos[(j * nVertex) + (i+1)].GetComponent<Node>().active)
                        Debug.DrawLine(Nos[((j + 1) * nVertex) + i].transform.position, Nos[((j) * nVertex) + (i + 1)].transform.position, Color.green);
                    }
                }              
            }
    }

    /// <summary>
    /// Checks if the slope is valid
    /// </summary>
    /// <returns></returns>
    bool isSlopeValid (RaycastHit h)
    {
        float slope = Vector3.Angle(h.collider.gameObject.transform.TransformDirection(Vector3.up), h.normal);//calculates angle between hitPoint and hitNormal
        if (slope > 90)
            slope = 180 - slope;
        if (slope > maxSlope)//returns false if slope is bigger than maxSlope
            return false;
        return true;
    }

    /// <summary>
    /// Checks if vertex is near a blocking wall
    /// </summary>
    /// <param name="n"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    bool isNearWall(GameObject n, RaycastHit h)
    {
        if (Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.forward, out h, maxBound) ||
            Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.back, out h, maxBound) ||
            Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.right, out h, maxBound) ||
            Physics.Raycast(n.transform.position + new Vector3(0, -maxBound, 0), Vector3.left, out h, maxBound))
            if (h.transform.gameObject.layer == 23)
                return true;

            return false;
    }
}