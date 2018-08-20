using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Individuo))]
public class CheckChegada : MonoBehaviour 
{
	public Individuo individuo;
	public bool passouDoMeioDaPista = false;

	void OnStart(){
		individuo = GetComponent<Individuo>();
	}

	/// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter(Collider other)
	{
		string tag = other.tag;

		switch(tag)
		{
			case "MeioPista":
				passouDoMeioDaPista = true;
				break;

			case "Chegada":
				if(passouDoMeioDaPista)
					individuo.Morrer();
				break;
		}
	}
}
