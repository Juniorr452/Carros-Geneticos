using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

/* Indivíduo é a classe que, a partir das informações dos cromossomos, 
   irá decidir como vai atuar sobre o carro (Acelerar, frear, mover pro lado...) */

/* A ideia seria ler a velocidade do carro e 
   os parâmetros dos sensores e decidir o que fazer de acordo
   com as informações dos cromossomos */

[RequireComponent(typeof(CarController))]
public class Individuo : MonoBehaviour 
{
	private string nome;
	public  Cromossomo cromossomo;

	private CarController controladorCarro;
	private SensoresCarro sensoresCarro;

	// Use this for initialization
	void Start () {
		controladorCarro = GetComponent<CarController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Reposicionar(Transform posicao)
    {
		Rigidbody rb       = controladorCarro.GetComponent<Rigidbody>();
		rb.velocity        = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		transform.position = posicao.position;
		transform.rotation = posicao.rotation;
    }

	public void novoIndividuo(String nome, Cromossomo cromossomo)
	{
		this.nome = nome;
		this.cromossomo = cromossomo;
		sensoresCarro.distanciaPercorrida = 0f;
	}
}