using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(SensoresCarro))]

public class UICarro : MonoBehaviour 
{ 
	public Text infoCarroText;
	public Text sensoresCarroText;
	
	private CarController       carController;
	private SensoresCarro       sensoresCarro;
	private DistanciaPercorrida distanciaCarro;

	// Use this for initialization
	void Start () 
	{
		distanciaCarro = GetComponent<DistanciaPercorrida>();
		carController  = GetComponent<CarController>();
		sensoresCarro  = GetComponent<SensoresCarro>();
	}	
	
	// Update is called once per frame
	void Update () 
	{
		StringBuilder sbSensoresCarro = new StringBuilder();
		StringBuilder sbInfoCarro     = new StringBuilder();

		// Texto do sensores carro
		for(int i = 0; i < sensoresCarro.distanciaSensores.Length; i++)
		{
			float distancia = sensoresCarro.distanciaSensores[i];

			sbSensoresCarro.Append(" | ");
			sbSensoresCarro.Append(distancia.ToString("0.0"));
		}

		sbSensoresCarro.Append(" |");

		// Texto do info carro (Velocidade, distância percorrida...)
		sbInfoCarro.Append(carController.CurrentSpeed.ToString("0.0"));
		sbInfoCarro.AppendLine(" mph");

		sbInfoCarro.Append("Dist: ");
		sbInfoCarro.Append(distanciaCarro.getDistanciaPercorrida().ToString("0.0"));

		// Atualizar a UI
		sensoresCarroText.text = sbSensoresCarro.ToString();
		infoCarroText.text     = sbInfoCarro.ToString();
	}
}
