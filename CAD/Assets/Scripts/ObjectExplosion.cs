using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectExplosion : MonoBehaviour {

    private List<Dictionary<string, GameObject>> partLists;

    private List<Transform> initialPositions;

    public int depthLevel = 0;

	// Use this for initialization
	void Start () {

        partLists = new List<Dictionary<string, GameObject>>();

        partLists.Add(CreatePartList(depthLevel));

        initialPositions = new List<Transform>();

        StartingPositions(partLists[0]);
    }
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.F))
            Explosion();
	}

    void StartingPositions(Dictionary<string, GameObject> partList) {

        foreach(GameObject part in partList.Values)
            initialPositions.Add(part.transform);
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

    void Explosion() {

        Vector3 center = this.transform.position;

        float radius = 5.0f;

        foreach(GameObject part in partLists[0].Values) {

            float angle = 360 / initialPositions.Count * Mathf.Deg2Rad;

            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);

            Vector3 newPosition = new Vector3(x, y, 0);

            part.transform.position = newPosition;
        }
    }
}
