using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ZoneManager : MonoBehaviour
{
    public float attractionForce;

    public GameObject spawnInObj;
    private void OnTriggerStay(Collider other)
    {
        GameObject[] CO_Parent = GameObject.FindGameObjectsWithTag("CO_Parent");
        GameObject[] CO2_Parent = GameObject.FindGameObjectsWithTag("CO2_Parent");
        GameObject[] SOO_Parent = GameObject.FindGameObjectsWithTag("SOO_Parent");
        GameObject[] CHH_Parent = GameObject.FindGameObjectsWithTag("CHH_Parent");
        GameObject[] ON_Child = GameObject.FindGameObjectsWithTag("ON_Child");

        GameObject[] ON = GameObject.FindGameObjectsWithTag("ON");
        GameObject[] CHH = GameObject.FindGameObjectsWithTag("CHH");
        GameObject[] OO2 = GameObject.FindGameObjectsWithTag("OO2");
        GameObject[] HH = GameObject.FindGameObjectsWithTag("HH");
        GameObject[] CO = GameObject.FindGameObjectsWithTag("CO");
        GameObject[] SOO = GameObject.FindGameObjectsWithTag("SOO");
        GameObject[] OON = GameObject.FindGameObjectsWithTag("OON");

        float forceMagnitude = attractionForce * Time.deltaTime;
        if (other.CompareTag("OO"))
        {
            GameObject[] O = GameObject.FindGameObjectsWithTag("O");
            GameObject[] OO = GameObject.FindGameObjectsWithTag("OO");


            if (O.Length >= 2 && CO.Length == 2)
            {
                other.transform.DetachChildren();
                foreach (GameObject child in O)
                {
                    Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
                    if (childRigidbody != null)
                    {
                        childRigidbody.isKinematic = false; 
                        childRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                    }
                }

                O[0].transform.position = Vector3.MoveTowards(O[0].transform.position, CO[0].transform.position, forceMagnitude * 10);

                O[1].transform.position = Vector3.MoveTowards(O[1].transform.position, CO[1].transform.position, forceMagnitude * 10);

                foreach (GameObject child in CO)
                {
                    Destroy(child, 1f);
                }
                foreach (GameObject child in O)
                {
                    Destroy(child, 1f);
                }
                foreach (GameObject child in OO)
                {
                    Destroy(child, 1f);
                }
                foreach (GameObject child in CO_Parent)
                {
                    Destroy(child, 1f);
                }
            }
            else
            {
                Debug.LogError("�� ������� ��� ������� � ������ �� �����: " + CO.Length + " � " + O.Length) ;
            }
        }
        if (other.CompareTag("OO3")) 
        {

            GameObject[] O = GameObject.FindGameObjectsWithTag("O4");
            GameObject[] OO3 = GameObject.FindGameObjectsWithTag("OO3");

            if (O.Length >= 2 && SOO.Length == 2)
            {
                other.transform.DetachChildren();
                foreach (GameObject child in O)
                {
                    Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
                    if (childRigidbody != null)
                    {
                        childRigidbody.isKinematic = false;
                        childRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                    }
                }

                O[0].transform.position = Vector3.MoveTowards(O[0].transform.position, SOO[0].transform.position, forceMagnitude *  10);

                O[1].transform.position = Vector3.MoveTowards(O[1].transform.position, SOO[1].transform.position, forceMagnitude * 10);

                foreach (GameObject child in O)
                {
                    Destroy(child, 1f);
                }
                foreach (GameObject child in SOO)
                {
                    Destroy(child, 1f);
                }
                foreach (GameObject child in OO3)
                {
                    Destroy(child, 1f);
                }
                foreach (GameObject child in SOO_Parent)
                {
                    Destroy(child, 1f);
                }
            }
            else
            {
                Debug.LogError("�� ������� ��� ������� � ������ �� �����: " + SOO + " � " + O);
            }
        }
        if (other.CompareTag("OO2"))
        {
            GameObject[] O = GameObject.FindGameObjectsWithTag("O1");

            other.transform.DetachChildren();

            foreach (GameObject child in O)
            {
                Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
                if (childRigidbody != null)
                {
                    childRigidbody.isKinematic = false;
                    childRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                }
                Destroy(child, 1f);
            }

            O[0].transform.position = Vector3.MoveTowards(O[0].transform.position, CHH[0].transform.position, forceMagnitude * 10);

            O[1].transform.position = Vector3.MoveTowards(O[1].transform.position, HH[0].transform.position, forceMagnitude * 10);

            foreach (GameObject child in CHH)
            {
                Destroy(child, 1f);
            }
            foreach (GameObject child in OO2)
            {
                Destroy(child, 1f);
            }
            foreach (GameObject child in HH)
            {
                Destroy(child, 1f);
            }
            foreach (GameObject child in CHH_Parent)
            {
                Destroy(child, 1f);
            }

        }
        if (other.CompareTag("OON"))
        {
            GameObject[] O5 = GameObject.FindGameObjectsWithTag("O5");
            GameObject[] N2 = GameObject.FindGameObjectsWithTag("N2");

            other.transform.DetachChildren();
            foreach (GameObject child in O5)
            {
                Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
                if (childRigidbody != null)
                {
                    childRigidbody.isKinematic = false;
                    childRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                }
            }
            foreach (GameObject child in N2)
            {
                Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
                if (childRigidbody != null)
                {
                    childRigidbody.isKinematic = false;
                    childRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                }
            }

            O5[1].transform.position = Vector3.MoveTowards(O5[1].transform.position, O5[0].transform.position, forceMagnitude * 10);

            foreach(GameObject child in O5)
            {
                Destroy(child,1f);
            }
            foreach (GameObject child in N2)
            {
                Destroy(child, 1f);
            }
            foreach (GameObject child in OON)
            {
                Destroy(child, 1f);
            }
        }
        if (other.CompareTag("ON"))
        {
            GameObject[] O3 = GameObject.FindGameObjectsWithTag("O3");

            other.transform.DetachChildren();
            foreach (GameObject child in O3)
            {
                Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
                if (childRigidbody != null)
                {
                    childRigidbody.isKinematic = false;
                    childRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                }
            }
            foreach (GameObject child in ON_Child)
            {
                Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
                if (childRigidbody != null)
                {
                    childRigidbody.isKinematic = false;
                    childRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                }
            }

            O3[1].transform.position = Vector3.MoveTowards(O3[1].transform.position, O3[0].transform.position, forceMagnitude * 10);

            foreach (GameObject child in O3)
            {
                Destroy(child, 1f);
            }
            foreach (GameObject child in ON)
            {
                Destroy(child, 1f);
            }
            foreach (GameObject child in CO2_Parent)
            {
                Destroy(child, 1f);
            }
            foreach (GameObject child in ON_Child)
            {
                Destroy(child, 1f);
            }
        }
    }

    public void DeleteAllMolec()
    {
        for (var i = spawnInObj.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(spawnInObj.transform.GetChild(i).gameObject);
        }

        //foreach (Transform child in spawnInObj.transform)
        //{
        //    Destroy(child.gameObject);
        //}
    }
}
