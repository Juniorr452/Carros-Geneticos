﻿using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(SensoresCarro))]

public class UICarro : MonoBehaviour 
{ 
	public Text nomeCarroText;
	public Text infoCarroText;
	public Text sensoresCarroText;
	public Text hvText;
	
	private Individuo           individuo;
	private CarController       carController;
	private SensoresCarro       sensoresCarro;
	private CalculadorPontuacao calculadorPontuacao;

	// Use this for initialization
	void Start () 
	{
		individuo          = GetComponent<Individuo>();
		nomeCarroText.text = individuo.nome;

		calculadorPontuacao = GetComponent<CalculadorPontuacao>();
		carController       = GetComponent<CarController>();
		sensoresCarro       = GetComponent<SensoresCarro>();
	}	
	
	// Update is called once per frame
	void Update () 
	{
		StringBuilder sbSensoresCarro = new StringBuilder();
		StringBuilder sbInfoCarro     = new StringBuilder();
		StringBuilder sbHV            = new StringBuilder();

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

		sbInfoCarro.Append("Pontuação: ");
		sbInfoCarro.Append(calculadorPontuacao.pontuacao.ToString("0.0"));

		// Output pra mover o carro
		sbHV.Append("\nh: ");
		sbHV.AppendLine(individuo.horizontal.ToString("0.0"));
		sbHV.Append("v: ");
		sbHV.AppendLine(individuo.vertical.ToString("0.0"));

		// Atualizar a UI
		sensoresCarroText.text = sbSensoresCarro.ToString();
		infoCarroText.text     = sbInfoCarro.ToString();
		hvText.text              = sbHV.ToString();
	}
}
