using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{

	//duration
	//intensity
	//damping
	public static CameraShake cameraShake = null;

	public float duration = 1;
	public float speed = 1f;
	public float magnitude = 1f;

	public bool test = false;
	public AnimationCurve dampingCurve;

	private Transform thisTransform = null;
	Vector3 startingPosition;

	void Awake()
	{
		cameraShake = this;
		thisTransform = GetComponent<Transform>();
		startingPosition = transform.localPosition;
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (test)
		{
			test = false;
			startShake();
		}
	}

	//calling shake again when its still shaking = camera moves
	public void startShake()
	{
		StopAllCoroutines();
		StartCoroutine(shake());
	}


	IEnumerator shake()
	{
		//Vector3 startingPosition = transform.position;
		float elapsed = 0;
		float randomStart = Random.Range(-1000, 1000f);

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;

			float percentDone = elapsed / duration;
			//float damper = 1 - Mathf.Clamp(2 * percentDone - 1, 0, 1);
			float damper = dampingCurve.Evaluate(percentDone);
			float sample = randomStart + speed * percentDone;

			float x = Mathf.PerlinNoise(sample, 0) * 2f - 1f;
			float y = Mathf.PerlinNoise(0, sample) * 2f - 1f;

			x *= (magnitude * damper);
			y *= (magnitude * damper);

			transform.localPosition = new Vector3(startingPosition.x + x, startingPosition.y + y, transform.localPosition.z);

			yield return null;
		}

		transform.localPosition = startingPosition;

	}
}
