using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class SessionData {

	float timestamp;
	float rate;
	float div;
	int sample;
	int[] status;
	float[] alpha;
	float[] beta;
	float[] gamma;
	float[] delta;
	float[] theta;


	public SessionData(float timestamp, float rate, float div, int sample, float[][] observation, int[] status) {
		this.timestamp = timestamp;
		this.rate = rate;
		this.div = div;
		this.sample = sample;
		this.status = status;
		this.delta = observation[0];
		this.theta = observation[1];
		this.alpha = observation[2];
		this.beta = observation[3];
		this.gamma = observation[4];
	}

	public static string Header() {
		return "Delta[0],Delta[1],Delta[2],Delta[3]," +
			"Theta[0],Theta[1],Theta[2],Theta[3]," +
			"Alpha[0],Alpha[1],Alpha[2],Alpha[3]," +
			"Beta[0],Beta[1],Beta[2],Beta[3]," +
			"Gamma[0],Gamma[1],Gamma[2],Gamma[3]," +
			"Signal-Status,Subject-Input,Timestamp\n";
			
	}

	public static string Input(string input) {
		return input.Replace ("\r", "").Replace ("\n", "");
	}

	public string CSV() {
		/*
		return String.Join(",", this.delta) + "," +
			String.Join(",", this.theta) + "," +
			String.Join(",", this.alpha) + "," +
			String.Join(",", this.beta) + "," +
			String.Join(",", this.gamma) + "," +
			String.Join("", this.status) + "," +
			this.input.ToString() + "," + 
			this.timestamp.ToString() + "\n";
			*/
		return "";
	}	

	public string JSON() {
		return JsonUtility.ToJson(this);
	}		
}