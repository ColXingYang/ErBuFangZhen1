using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    Collider m_Collider;
    Vector3 m_Center;
    Vector3 m_Size, m_Min, m_Max;


    private BoxCollider _boxCollider; 
	// Use this for initialization
	void Start () {
        _boxCollider = GetComponent<BoxCollider>();
        // Vector3 pos = transform.position + _boxCollider.


        //Fetch the Collider from the GameObject
        m_Collider = GetComponent<Collider>();
        //Fetch the center of the Collider volume
        m_Center = m_Collider.bounds.center;
        //Fetch the size of the Collider volume
        m_Size = m_Collider.bounds.size;
        //Fetch the minimum and maximum bounds of the Collider volume
        m_Min = m_Collider.bounds.min;
        m_Max = m_Collider.bounds.max;
        //Output this data into the console
        OutputData();

        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject s1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s.transform.position = m_Max;
        s1.transform.position = m_Min;
    }

    void OutputData()
    {
        //Output to the console the center and size of the Collider volume
        Debug.Log("Collider Center : " + m_Center);
        Debug.Log("Collider Size : " + m_Size);
        Debug.Log("Collider bound Minimum : " + m_Min);
        Debug.Log("Collider bound Maximum : " + m_Max);
    }

    // Update is called once per frame
    void Update () {
       
		
	}
}
