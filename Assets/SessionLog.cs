using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SessionLog : MonoBehaviour {

	public string savePath = "session-data/";
	public int bufferSize = 100;
	public int maxCount = 10000;
	public bool up; //start recording
	public SessionData data;

	private List<SessionData> dataBuffer;
	private int counter;
	private int index;
	private string dataFile;
	private string dataPath;


	void Start () {
		foreach (string file in System.IO.Directory.GetFiles(savePath))
			File.Delete (file);

		counter = 0;
		index = 1;
		up = false;
	}


	void Update () {
		if (counter > 0) {
			if (counter % bufferSize == 0 || counter % maxCount == 0) {
				Write ();
				dataBuffer = new List<SessionData>();
			}

			if (counter % maxCount == 0) {
				index += 1;
				StartRecording ();	
			}
		}
	}


	void OnApplicationQuit() {
		if (up) StopRecording ();
	}


	void Write() {
		foreach (SessionData data in dataBuffer) {
			File.AppendAllText (dataPath, data.CSV ());
		}
	}


	public void StartRecording() {
		dataBuffer = new List<SessionData>();
		dataFile = index.ToString () + ".csv";
		dataPath = savePath + dataFile;
		File.WriteAllText (dataPath, SessionData.Header ());
	}


	public void StopRecording() {
		Write ();
		index += 1;
	}


	public void Add(SessionData data) {
		dataBuffer.Add (data);	
		counter += 1;
	}
}
	