// muse-io --device Muse-9302 --preset 14 --50hz --osc osc.udp://localhost:5000

using UnityEngine;
using System;
using System.Collections;

public class MuseIO : MonoBehaviour {

	public OscIn oscIn;
	public int MusePort = 5000;

	/********************************************************************
	 *  http://developer.choosemuse.com/research-tools/available-data
	 ********************************************************************/

	public int[] HorseShoe = new int[4] { 0, 0, 0, 0 };
	public int[] SignalStatus = new int[4] { 0, 0, 0, 0 };

	public float[] Accselerometer = new float[3] { 0f, 0f, 0f };
	public bool Forehead = false;
	public bool Blink = false;
	public bool Jaw = false;

	public float[] EEG = new float[4] { 0f, 0f, 0f, 0f };
	public float[] EEGQuantization = new float[4] { 0f, 0f, 0f, 0f };
	public float[] LowAbsolute = new float[4] { 0f, 0f, 0f, 0f };
	public float[][] FastFourier = new float[4][];

	public string[] SpectrumMap = new string[5] { "Delta","Theta","Alpha","Beta","Gamma" };
	public float[][] Absolute = new float[5][];
	public float[][] Relative = new float[5][];
	public float[][] Score = new float[5][];


	/********************************************************************
	 *  Unity Engine
	 ********************************************************************/

	void Start() {

		if( !oscIn ) oscIn = gameObject.AddComponent<OscIn>();
		oscIn.Open( MusePort );
	
		oscIn.Map("/muse/elements/horseshoe", OnHorseShoe);
		oscIn.Map("/muse/elements/is_good",   OnSignalStatus);

		oscIn.Map("/muse/acc",                        OnAccselerometer);
		oscIn.Map("/muse/elements/touching_forehead", OnForehead);
		oscIn.Map("/muse/elements/blink",             OnBlink);
		oscIn.Map("/muse/elements/jaw_clench",        OnJaw);

		oscIn.Map("/muse/eeg", 				           OnEEG);
		oscIn.Map("/muse/eeg/quantization",            OnEEGQuantization);
		oscIn.Map("/muse/elements/low_freqs_absolute", OnLowAbsolute);

		oscIn.Map("/muse/elements/raw_fft0", OnFastFourier0);
		oscIn.Map("/muse/elements/raw_fft1", OnFastFourier1);
		oscIn.Map("/muse/elements/raw_fft2", OnFastFourier2);
		oscIn.Map("/muse/elements/raw_fft3", OnFastFourier3);

		oscIn.Map("/muse/elements/delta_absolute", OnDeltaAbsolute);
		oscIn.Map("/muse/elements/theta_absolute", OnThetaAbsolute);
		oscIn.Map("/muse/elements/alpha_absolute", OnAlphaAbsolute);
		oscIn.Map("/muse/elements/beta_absolute",  OnBetaAbsolute);
		oscIn.Map("/muse/elements/gamma_absolute", OnGammaAbsolute);

		oscIn.Map("/muse/elements/delta_relative", OnDeltaRelative);
		oscIn.Map("/muse/elements/theta_relative", OnThetaRelative);
		oscIn.Map("/muse/elements/alpha_relative", OnAlphaRelative);
		oscIn.Map("/muse/elements/beta_relative",  OnBetaRelative);
		oscIn.Map("/muse/elements/gamma_relative", OnGammaRelative);

		oscIn.Map("/muse/elements/delta_session_score", OnDeltaScore);
		oscIn.Map("/muse/elements/theta_session_score", OnThetaScore);
		oscIn.Map("/muse/elements/alpha_session_score", OnAlphaScore);
		oscIn.Map("/muse/elements/beta_session_score",  OnBetaScore);
		oscIn.Map("/muse/elements/gamma_session_score", OnGammaScore);


		for (int i = 0; i < 4; i++) {
			FastFourier [i] = new float[129];
			for (int j = 0; j < 129; j++) {
				FastFourier [i] [j] = 0f;
			}
		}
			
		for (int i = 0; i < 5; i++) {
			Absolute [i] = new float[4];
			Relative [i] = new float[4];
			Score [i] = new float[4];
			for (int j = 0; j < 4; j++) {
				Absolute [i] [j] = 0f;
				Relative [i] [j] = 0f;
				Score [i] [j] = 0f;
			}
		}
	}


	void OnDisable() {
		oscIn.Unmap(OnHorseShoe);
		oscIn.Unmap(OnSignalStatus);

		oscIn.Unmap(OnAccselerometer);
		oscIn.Unmap(OnForehead);
		oscIn.Unmap(OnBlink);
		oscIn.Unmap(OnJaw);

		oscIn.Unmap(OnEEG);
		oscIn.Unmap(OnEEGQuantization);
		oscIn.Unmap(OnLowAbsolute);

		oscIn.Unmap(OnFastFourier0);
		oscIn.Unmap(OnFastFourier1);
		oscIn.Unmap(OnFastFourier2);
		oscIn.Unmap(OnFastFourier3);

		oscIn.Unmap(OnDeltaAbsolute);
		oscIn.Unmap(OnThetaAbsolute);
		oscIn.Unmap(OnAlphaAbsolute);
		oscIn.Unmap(OnBetaAbsolute);
		oscIn.Unmap(OnGammaAbsolute);

		oscIn.Unmap(OnDeltaRelative);
		oscIn.Unmap(OnThetaRelative);
		oscIn.Unmap(OnAlphaRelative);
		oscIn.Unmap(OnBetaRelative);
		oscIn.Unmap(OnGammaRelative);

		oscIn.Unmap(OnDeltaScore);
		oscIn.Unmap(OnThetaScore);
		oscIn.Unmap(OnAlphaScore);
		oscIn.Unmap(OnBetaScore);
		oscIn.Unmap(OnGammaScore);
	}


	/********************************************************************
	 *  helpers
	 ********************************************************************/

	void Test( OscMessage message ) {
		Debug.Log( message );

		int i = 0;
		foreach (object o in message.args) {
			Debug.Log (o.GetType());
			Debug.Log (i);
			i++;
		}
	}


	void ParseInt(int[] target, OscMessage message) {
		int i = 0;
		foreach (object o in message.args) {
			if (i < target.Length) {
				target [i] = System.Convert.ToInt32 (o);

			} else {
				Debug.Log ("Index OUT OF RANGE for " + target.ToString ());
			}
			i++;
		}		
	}


	void ParseFloat(float[] target, OscMessage message) {
		int i = 0;
		foreach (object o in message.args) {
			if (i < target.Length) {
				Double val = System.Convert.ToDouble (o);
				if (!Double.IsNaN (val))
					target [i] = (float)val;
				else
					target [i] = 0f;

			} else {
				Debug.Log ("Index OUT OF RANGE for " + target.ToString ());
			}
			i++;
		}		
	}


	/********************************************************************
	 *  Muse measured metrics
	 ********************************************************************/

	public void OnSignalStatus(OscMessage message) {
		ParseInt (SignalStatus, message);
	}

	public void OnHorseShoe(OscMessage message) {
		ParseInt (HorseShoe, message);
	}



	public void OnAccselerometer(OscMessage message) {
		ParseFloat (Accselerometer, message);
	}

	public void OnForehead(OscMessage message) {
		foreach (object o in message.args) {
			Forehead = System.Convert.ToInt32(o) == 1;
		}
	}

	public void OnBlink(OscMessage message) {
		foreach (object o in message.args) {
			Blink = System.Convert.ToInt32(o) == 1;
		}
	}

	public void OnJaw(OscMessage message) {
		foreach (object o in message.args) {
			Jaw = System.Convert.ToInt32(o) == 1;
		}
	}



	public void OnEEG(OscMessage message) {
		ParseFloat (EEG, message);
	}

	public void OnEEGQuantization(OscMessage message) {
		ParseFloat (EEGQuantization, message);
	}

	public void OnLowAbsolute(OscMessage message) {
		ParseFloat (LowAbsolute, message);
	}



	public void OnFastFourier0(OscMessage message) {
		ParseFloat (FastFourier[0], message);
	}

	public void OnFastFourier1(OscMessage message) {
		ParseFloat (FastFourier[1], message);
	}

	public void OnFastFourier2(OscMessage message) {
		ParseFloat (FastFourier[2], message);
	}

	public void OnFastFourier3(OscMessage message) {
		ParseFloat (FastFourier[3], message);
	}



	public void OnDeltaAbsolute(OscMessage message) {
		ParseFloat (Absolute[0], message);
	}

	public void OnThetaAbsolute(OscMessage message) {
		ParseFloat (Absolute[1], message);
	}

	public void OnAlphaAbsolute(OscMessage message) {
		ParseFloat (Absolute[2], message);
	}

	public void OnBetaAbsolute(OscMessage message) {
		ParseFloat (Absolute[3], message);
	}

	public void OnGammaAbsolute(OscMessage message) {
		ParseFloat (Absolute[4], message);
	}



	public void OnDeltaRelative(OscMessage message) {
		ParseFloat (Relative[0], message);
	}

	public void OnThetaRelative(OscMessage message) {
		ParseFloat (Relative[1], message);
	}

	public void OnAlphaRelative(OscMessage message) {
		ParseFloat (Relative[2], message);
	}

	public void OnBetaRelative(OscMessage message) {
		ParseFloat (Relative[3], message);
	}

	public void OnGammaRelative(OscMessage message) {
		ParseFloat (Relative[4], message);
	}



	public void OnDeltaScore(OscMessage message) {
		ParseFloat (Score[0], message);
	}

	public void OnThetaScore(OscMessage message) {
		ParseFloat (Score[1], message);
	}

	public void OnAlphaScore(OscMessage message) {
		ParseFloat (Score[2], message);
	}

	public void OnBetaScore(OscMessage message) {
		ParseFloat (Score[3], message);
	}

	public void OnGammaScore(OscMessage message) {
		ParseFloat (Score[4], message);
	}
}
