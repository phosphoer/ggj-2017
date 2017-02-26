using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainSpout : MonoBehaviour {

	public float FlowSpeed = 2f;
	public Vector2 FlowDirection = new Vector2(1f,0f);
	public float EndWobble = .5f;
	public float WobbleSpeed = 1f;
	public Vector3 WobbleMask = new Vector3(1f,0f,1f);
	public GameObject SplashFX;
	[Range(0f,1f)]
	public float SplashSpinSpeed = .5f;

	Material mat;
	LineRenderer line;
	Vector2 offset = new Vector2(0f,0f);

	Vector3 wobble = Vector3.zero;
	Vector3 wobbleAnchor = Vector3.zero;
	Vector3 wobbleDest = Vector3.zero;

	float splashTimer = 0f;
	Vector3 splashOffset = Vector3.zero;
	Vector3 splashBaseRot = Vector3.zero;

	// Use this for initialization
	void Awake () {
		line = gameObject.GetComponent<LineRenderer>();
		mat = line.material;

		wobbleAnchor = line.GetPosition(line.numPositions-1);
		wobbleDest = wobbleAnchor;
		wobble = wobbleDest;

		if(SplashFX){
			splashOffset = SplashFX.transform.localPosition - wobbleAnchor;
			splashBaseRot = SplashFX.transform.localEulerAngles;
		}
	}
	
	// Update is called once per frame
	void Update () {

		//Texture Scroll
		offset = Vector2.MoveTowards(offset,FlowDirection,Time.deltaTime*FlowSpeed);
		if(offset == FlowDirection) offset = Vector2.zero;
		mat.SetTextureOffset("_MainTex",offset);


		//Line Render Wobble
		wobble = Vector3.Slerp(wobble,wobbleDest,Time.deltaTime*WobbleSpeed);

		if(Vector3.Distance(wobble,wobbleDest)<=EndWobble*.25){
			wobbleDest = wobbleAnchor + new Vector3(
				WobbleMask.x*Random.Range(-EndWobble,EndWobble),
				WobbleMask.y*Random.Range(-EndWobble,EndWobble),
				WobbleMask.z*Random.Range(-EndWobble,EndWobble)
			);
		}

		line.SetPosition(line.numPositions-1,wobble);

		//SplashFX
		if(SplashFX){
			SplashFX.transform.localPosition = wobble + splashOffset;
			splashTimer += Time.deltaTime;
			if(splashTimer>=1-SplashSpinSpeed){
				splashTimer = 0f;
				Vector3 randRot = new Vector3(0f,Random.Range(0f,360f),0f);
				SplashFX.transform.rotation = Quaternion.Euler(splashBaseRot + randRot);
			}
		}
	}
}
