using UnityEngine;

[RequireComponent(typeof(OscOut))]
public class SonicPi : MonoBehaviour
{
	public OscOut oscOut;
	public int SonicPort = 4557;
	public string SonicHost = "127.0.0.1";

	void Start()
	{
		if( !oscOut ) oscOut = gameObject.AddComponent<OscOut>();
		oscOut.Open( SonicPort, SonicHost );
	}


	public void StopAll()
	{
		oscOut.Send("/stop-all-jobs", "SONIC_PI_CLI", "SONIC_PI_CLI");
	}


	public void RunCode(string code)
	{
		oscOut.Send("/run-code", "SONIC_PI_CLI", code);
	}


	void OnDisable()
	{
		StopAll ();
	}
}
