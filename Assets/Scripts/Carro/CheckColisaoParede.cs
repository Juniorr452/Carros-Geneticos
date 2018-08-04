using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckColisaoParede : MonoBehaviour 
{
	public Individuo individuo;

	/// <summary>
	/// OnCollisionEnter is called when this collider/rigidbody has begun
	/// touching another rigidbody/collider.
	/// </summary>
	/// <param name="other">The Collision data associated with this collision.</param>
	void OnCollisionEnter(Collision other)
	{
		if(other.collider.tag == "Parede")
			individuo.Morrer();
	}
}
