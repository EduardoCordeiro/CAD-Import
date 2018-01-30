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
            if(depthLevel > 0 && child.childCount > 1)
                for(int j = 0; j < child.childCount; j++)
                    partList.Add(child.GetChild(j).name + i.ToString() + j.ToString(), child.GetChild(j).gameObject);
        }

        return partList;
    }

    void createSphere() {

        Vector3 center = this.transform.position;

        float radius = 5.0f;
    }
}
