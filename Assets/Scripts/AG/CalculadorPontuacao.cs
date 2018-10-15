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
		float diferencaSensores = getDiferencaSensores();
		float velocidade        = carController.SignCurrentSpeed;

		if (diferencaSensores < 1) diferencaSensores = 1;

		pontuacao += velocidade * 0.005f / diferencaSensores;
	}

	float getDiferencaSensores()
	{
		float sensorEsquerda = getRayDistance(-transform.right);
		float sensorDireita  = getRayDistance(transform.right);

		return Mathf.Abs(sensorEsquerda - sensorDireita); 
	}

	float getRayDistance(Vector3 direcao)
	{
		RaycastHit hit;
		Physics.Raycast(sensoresCarro.offsetRaycast + transform.position, direcao, out hit, 100, sensoresCarro.layerMaskSensor);
		Debug.DrawLine(sensoresCarro.offsetRaycast + transform.position, hit.point, Color.green, 0);

		return hit.distance;
	}
}
