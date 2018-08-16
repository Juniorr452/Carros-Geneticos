using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CalculadorPontuacao : MonoBehaviour 
{
	public float pontuacao = 0f;

	private SensoresCarro sensoresCarro;
	private CarController carController;

	// Use this for initialization
	void Start () {
		sensoresCarro = GetComponent<SensoresCarro>();
		carController = GetComponent<CarController>();
	}
	
	/**
	 * Calcular a pontuação do carro dependendo se ele
	 * está mais no centro da pista e de acordo com a velocidade.
	 */
	void Update () 
	{
		float sensorEsquerda = sensoresCarro.distanciaSensores[0];
		float sensorDireita  = sensoresCarro.distanciaSensores[2];

		float diferencaSensores = Mathf.Abs(sensorEsquerda - sensorDireita);
		float velocidade        = carController.SignCurrentSpeed;

		if (diferencaSensores < 1) diferencaSensores = 1;

		pontuacao += velocidade * 0.005f / diferencaSensores;
	}
}
