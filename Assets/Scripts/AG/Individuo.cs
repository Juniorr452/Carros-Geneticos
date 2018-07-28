using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

/* Indivíduo é a classe que, a partir das informações do gene, 
   irá decidir como vai atuar sobre o carro (Acelerar, frear, mover pro lado...) */

/* A ideia seria ler a velocidade do carro e 
   os parâmetros dos sensores e decidir o que fazer de acordo
   com as informações do gene */

[RequireComponent(typeof(CarController))]
public class Individuo : MonoBehaviour 
{
	private string nome;
	public  Gene   gene;

	private CarController controladorCarro;
	private SensoresCarro sensoresCarro;

	// Use this for initialization
	void Start () {
		controladorCarro = GetComponent<CarController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
