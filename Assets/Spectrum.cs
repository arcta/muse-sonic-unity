using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class Spectrum : MonoBehaviour {
	public float rate = 1.618f;
	public float div = 8.9f;
	public float amp = 5f;
	public int sample = 5;

	public GameObject dotPrefab;
	public SonicPi synth;

	private GameObject[] dots;
	private Text[] info;

	private string[] label = new string[5] { "Delta","Theta","Alpha","Beta","Gamma" };

	private float timer;
	private float pacer;


	void Start () {
		GameObject canvas = GameObject.Find("Info");
		GameObject text;
		GameObject dot;
		Color c;

		timer = 0.6f * Mathf.PI;
		pacer = 0f;

		dots = new GameObject[5];
		info = new Text[5];

		for (int i = 0; i < 5; i++) {
			c = new Color (0.99f - i * 0.22f, 0.3f, i * 0.22f, 1f);

			text = new GameObject("Text");
			text.transform.SetParent(canvas.transform);

			RectTransform rect = text.AddComponent<RectTransform>();
			rect.anchoredPosition = new Vector3((i + 1) * 100, 0, 0);

			Text txt = text.AddComponent<Text>();
			txt.text = label[i];
			txt.color = c;
			info[i] = txt;

			dot = (GameObject)Instantiate (
				dotPrefab,
				i * Vector3.right,
				Quaternion.identity);

			dot.GetComponent<MeshRenderer> ().material.color = c;
			dot.GetComponent<TrailRenderer> ().material.color = c;
			dots[i] = dot;
		}
	}


	float channelsAvg(float[] vals, int[] stat) {
		float norm = 0f;
		float val = 0f;
		for (int j = 0; j < 4; ++j) {
			if (stat [j] > 0) { //SignalStatus
			//if (stat [j] < 3) { //HorseShoe
				val += vals [j];
				norm += 1f;
			}
		}

		if (norm == 0)
			return 0f;

		return val/norm;
	}


	public void Series (float[][] vals, int[] stat, Camera camera) {
		//https://github.com/samaaron/sonic-pi/blob/master/etc/doc/cheatsheets/samples.md
		string[] samples = new string[5] { "ambi_haunted_hum","ambi_drone","perc_bell","elec_twang","elec_tick" };
		string c;
		string text = "";

		timer += Time.deltaTime * 0.1f;
		if (pacer >= 5f * rate) {
			pacer = 0f;
			synth.StopAll ();
		}
		pacer += 1;
		camera.transform.Rotate (0, -Time.deltaTime * 18 / Mathf.PI, 0);

		for (int i = 0; i < 5; ++i) {
			float R = 7f + i;
			float val = channelsAvg(vals[i], stat);
			int j = sample;
			c = "#" + ((9 - 2 * i) * 11).ToString() + "30" + (19 * i + 10).ToString() + "ff";

			if (pacer == Mathf.Round(i * rate)) {
				if (sample > 4)
					j = i;
				string code = "sample :" + samples[j] + ", rate: " + (div * val).ToString ();
				synth.RunCode (code);
			}

			float d = timer + i * 0.05f;
			dots[i].transform.position = new Vector3 (R * Mathf.Cos(d), amp * val, R * Mathf.Sin(d));
			info[i].text = label[i] + ": " + val.ToString("F2");
			text = "<color=" + c + ">" + label[i] + "</color>: " + val.ToString("F2") + "   " + text;
		}

		GameObject test = GameObject.Find("Test");
		test.GetComponent<Text>().text = text;
		test.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, 0);
	}
}

