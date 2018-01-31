using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectExplosion : MonoBehaviour {

    private List<Dictionary<string, GameObject>> partLists;

    private List<Vector3> initialPositions;

    public int depthLevel = 0;

	// Use this for initialization
	void Start () {

        partLists = new List<Dictionary<string, GameObject>>();

        partLists.Add(CreatePartList(depthLevel));

        initialPositions = new List<Vector3>();

        SaveStartingPositions(partLists[0]);
    }
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.F))
            Explosion(2.0f, partLists[0]);

        if(Input.GetKeyDown(KeyCode.G))
            CircleExplosion(2.0f, partLists[0]);

        if(Input.GetKeyDown(KeyCode.R))
            ReverseExplosion(partLists[0]);
	}

    void SaveStartingPositions(Dictionary<string, GameObject> partList) {

        foreach(GameObject part in partList.Values)
            initialPositions.Add(part.transform.position);
    }

    Dictionary<string, GameObject> CreatePartList(int depthLevel) {

        Dictionary<string, GameObject>  partList = new Dictionary<string, GameObject>();

        for(int i = 0; i < transform.childCount; i++) {

            Transform child = transform.GetChild(i);

            partList.Add(child.name + i, child.gameObject);

            // Adding Grand children if Depth Level > 0
            // Add Recursiveness here for the new depth levels (maybe change the function itself)
            if(depthLevel > 0 && child.childCount > 1)
                for(int j = 0; j < child.childCount; j++)
                    partList.Add(child.GetChild(j).name + i.ToString() + j.ToString(), child.GetChild(j).gameObject);
        }

        return partList;
    }

   /// <summary>
   /// Sphere Explosion
   /// </summary>
   /// <param name="radius"></param>
   /// <param name="partList"></param>
    void Explosion(float radius, Dictionary<string, GameObject> partList) {

        Vector3 center = transform.position;

        int numberOfParts = partList.Values.Count;

        int counter = 0;

        foreach(GameObject part in partList.Values) {

            float x = center.x + radius * Mathf.Cos(2 * Mathf.PI * counter / numberOfParts) * Mathf.Sin(Mathf.PI * counter / numberOfParts);
            float y = center.y + radius * Mathf.Sin(2 * Mathf.PI * counter / numberOfParts) * Mathf.Sin(Mathf.PI * counter / numberOfParts);
            float z = center.z + radius * Mathf.Sin(Mathf.PI * counter / numberOfParts);

        /*
        x=origin.x+radius*cos(rotation.y)*cos(rotation.x)
        y=origin.y+radius*sin(rotation.x)
        z=origin.z+radius*sin(rotation.y)*cos(rotation.x)
        */

            part.transform.position = new Vector3(x, y, z);

            counter++;
        }
    }

    /// <summary>
    /// Circle Explosion
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="partList"></param>
    void CircleExplosion(float radius, Dictionary<string, GameObject> partList) {

        Vector3 center = transform.position;

        int numberOfParts = partList.Values.Count;

        int counter = 0;

        foreach(GameObject part in partList.Values) {

            float x = center.x + radius * Mathf.Cos(2 * Mathf.PI * counter / numberOfParts);
            float y = center.y + radius * Mathf.Sin(2 * Mathf.PI * counter / numberOfParts);

            part.transform.position = new Vector3(x, y, 0.0f);

            counter++;
        }
    }

    void ReverseExplosion(Dictionary<string, GameObject> partList) {

        int counter = 0;

        foreach(GameObject part in partList.Values) {

            part.transform.position = initialPositions[counter];

            counter++;
        }            
    }
}
