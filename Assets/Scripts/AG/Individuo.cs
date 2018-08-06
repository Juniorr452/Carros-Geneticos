using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
	public string nome;
	public Cromossomo cromossomo;

	public bool morto = false;

	private DistanciaPercorrida distanciaCarro;
	private CarController       controladorCarro;
	private SensoresCarro       sensoresCarro;

	public CinemachineVirtualCamera cameraCinemachine;
	public Renderer carroRenderer;

	// Use this for initialization
	void Start () 
	{
		controladorCarro = GetComponent<CarController>();
		distanciaCarro   = GetComponent<DistanciaPercorrida>();
		sensoresCarro    = GetComponent<SensoresCarro>();
	}
	
	// Update is called once per frame
	void Update () {
		Mover();
	}

	public void Setar(String nome, Cromossomo cromossomo)
	{
		this.nome = nome;
		this.cromossomo = cromossomo;
	}

    private void Mover()
    {
        float horizontal = 0f;
		float vertical   = 1f;

		float sensorEsquerda    = sensoresCarro.distanciaSensores[0];
		float sensorDigEsquerda = sensoresCarro.distanciaSensores[1];
		float sensorFrente      = sensoresCarro.distanciaSensores[2];
		float sensorDigDireita  = sensoresCarro.distanciaSensores[3];
		float sensorDireita     = sensoresCarro.distanciaSensores[4];
		float velocidade        = controladorCarro.CurrentSpeed;

		byte[] genes = cromossomo.decodificar();

		if(genes[0] <= sensorEsquerda)
			horizontal -= .5f;

		if(genes[1] <= sensorDigEsquerda)
		{
			horizontal -= .25f;
			vertical   -= .25f;
		}

		if(genes[2] <= sensorFrente)
			vertical   += .25f;
		
		if(genes[3] <= sensorDigDireita)
		{
			horizontal += .25f;
			vertical   -= .25f;
		}

		if(genes[4] <= sensorDireita)
			horizontal += .5f;

		if(genes[5] <= velocidade)
			vertical += .2f;

		controladorCarro.Move(horizontal, vertical, 1f, 0f);
		//controladorCarro.Move(steering, accel, footbrake, handbrake);
    }

    public void Reposicionar(Transform posicao)
    {
		Rigidbody rb       = controladorCarro.GetComponent<Rigidbody>();
		rb.velocity        = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		transform.position = posicao.position;
		transform.rotation = posicao.rotation;

		distanciaCarro.ResetarDistancia(posicao.position);
    }

	/*public void novoIndividuo(String nome, Cromossomo cromossomo)
	{
		this.nome = nome;
		this.cromossomo = cromossomo;
		distanciaCarro.ResetarDistancia();
	}*/

    public void Morrer()
	{
		if(!morto)
		{
			cameraCinemachine.Priority = 0;

			AlgoritmoGenetico alg = FindObjectOfType<AlgoritmoGenetico>();
			alg.MatarIndividuo(this);
		}
    }

	public static int OrdenarPelaDistanciaPercorrida(Individuo i1, Individuo i2) 
	{
		float d1 = i1.distanciaCarro.getDistanciaPercorrida();
		float d2 = i2.distanciaCarro.getDistanciaPercorrida();

    	return d2.CompareTo(d1);
 	}
}