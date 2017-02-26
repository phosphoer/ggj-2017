using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class grassData{
	MeshFilter filter;
	float[] swayWeights = new float[0];
	Vector3[] rootPos = new Vector3[0];
	bool built = false;
	GrassSway master;
	float rndmRigidity = 0f;
	Vector2 heightBounds = new Vector2(-999,999);

	public void ApplySway(Vector3 swayPow){
		if(built){
			List<Vector3> tempVerts = new List<Vector3>();
			//Debug.Log(tempVerts.Count + " / " + filter.mesh.vertexCount);
			for(int i=0; i<filter.mesh.vertexCount; i++){
				Vector3 offset = (swayPow * master.HeightSwayCurve.Evaluate(swayWeights[i])) * (1f-rndmRigidity);
				tempVerts.Add(rootPos[i] + offset);
			}
			filter.mesh.SetVertices(tempVerts);
		}
	}

	public void Build(MeshFilter grassMesh, GrassSway mstr){
		//First we need to store root positions of all the verts, so we can offset instead of set it later.
		//While wer're here, we'll also be gathering the Y transform of each to build our bounds for the next step..
		if(grassMesh.mesh.vertexCount>2 && mstr!=null){
			filter = grassMesh;
			rootPos = new Vector3[filter.mesh.vertexCount];

			for(int i=0; i<filter.mesh.vertexCount; i++){
				rootPos[i] = filter.mesh.vertices[i];
				if(rootPos[i].y>heightBounds.x) heightBounds.x = rootPos[i].y;
				if(rootPos[i].y<heightBounds.y) heightBounds.y = rootPos[i].y;
			}

			//Build weights as a % of height in this mesh; 0 == lowest point && 1 == highest point in this mesh.
			//We'll use this to sample the sway curve on the master when apply sway..
			swayWeights = new float[filter.mesh.vertexCount];
			float boundsDiff = heightBounds.x - heightBounds.y;
			for(int i=0; i<swayWeights.Length; i++){
				swayWeights[i] = Mathf.Clamp01(rootPos[i].y/boundsDiff);
			}

			master = mstr;
			rndmRigidity = Random.Range(0f,master.MaxRandRigidity);

			built = true;
		}
	}
}

public class GrassSway : MonoBehaviour {

	public Vector3 WindMask = new Vector3(1f,.25f,1f);
	public Vector3 WindMinStrength = new Vector3(-.2f,-.2f,-.2f);
	public Vector3 WindMaxStrength = new Vector3(.2f,.2f,.2f);
	public float WindOscillationTimeMin = .5f;
	public float WindOscillationTimeMax = 2f;
	public AnimationCurve HeightSwayCurve = AnimationCurve.EaseInOut(0,0,1f,1f);
	[Range(0,10)]
	public int FrameUpdateDivision = 1;
	[Range(0f,1f)]
	public float MaxRandRigidity = .5f;

	int dataDivs = 0;
	float currentWindTime = 0f;
	float windTimer = 0f;
	int frmCount = 0;
	Vector3 lastWind = Vector3.zero;
	Vector3 currentWind = Vector3.zero;
	Vector3 windDest = Vector3.zero;
	List<grassData> data = new List<grassData>();

	// Use this for initialization
	void Start () {
		BuildData();
		frmCount = FrameUpdateDivision;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateWind();
		if(frmCount>=FrameUpdateDivision+1){
			frmCount=0;
			dataDivs = Mathf.CeilToInt(data.Count/(FrameUpdateDivision+1));
		}

		for(int i=frmCount*dataDivs; i<data.Count; i++){
			data[i].ApplySway(currentWind);
		}

		frmCount++;
	}

	public void UpdateWind(){
		windTimer+=Time.deltaTime;
		if(windTimer >= currentWindTime){
			windTimer = 0f;
			windDest = new Vector3(
				Random.Range(WindMinStrength.x,WindMaxStrength.x)*WindMask.x,
				Random.Range(WindMinStrength.y,WindMaxStrength.y)*WindMask.y,
				Random.Range(WindMinStrength.z,WindMaxStrength.z)*WindMask.z
			);
			lastWind = currentWind;
			currentWindTime = Random.Range(WindOscillationTimeMin,WindOscillationTimeMax);
		}
		currentWind = Vector3.Slerp(lastWind,windDest,(windTimer/currentWindTime));

	}

	public void BuildData(){
		data.Clear();
		int dataInc = 0;
		for(int i=0; i<transform.childCount; i++){
			if(transform.GetChild(i).GetComponent<MeshFilter>()){
				data.Add(new grassData());
				data[dataInc].Build(transform.GetChild(i).GetComponent<MeshFilter>(),this);
				dataInc++;
			}
		}
	}
}
