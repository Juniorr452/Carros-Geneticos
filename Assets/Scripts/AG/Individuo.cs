using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

/**
 * A partir das informações dos cromossomos, o indivíduo irá
 * decidir como vai atuar sobre o carro (Acelerar, frear, mover pro lado...)
 * 
 * A ideia seria ler a velocidade do carro e os 
 * parâmetros dos sensores e decidir o que fazer 
 * de acordo com as informações dos cromossomos.
 */

[RequireComponent(typeof(CarController))]
public class Individuo : MonoBehaviour 
{
	//
	// ─── INFORMAÇÕES BÁSICAS ────────────────────────────────────────────────────────
	//

	public string nome;
	public Cromossomo cromossomo;

	public bool morto = false;

	//
	// ─── SENSORES E CONTROLADOR ─────────────────────────────────────────────────────
	//

	/**
	 * Para controlar o carro (fazer virar, acelerar...).
	 */
	private CarController controladorCarro;

	private DistanciaPercorrida distanciaCarro;
	private SensoresCarro       sensoresCarro;

	//
	// ─── OUTROS ─────────────────────────────────────────────────────────────────────
	//

	/**
	 * Camera do cinemachine para acompanhar o carro.
	 */
	public CinemachineVirtualCamera cameraCinemachine;

	/**
	 * Renderer para alterar a cor do material do carro.
	 */
	public Renderer carroRenderer;

	// ────────────────────────────────────────────────────────────────────────────────

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

	/**
	 * Usado para quando for gerar um 
	 * novo indivíduo pelo crossover.
	 * 
	 * @param nome - O nome do novo indivíduo.
	 * @param cromossomo - O novo cromossomo do indivíduo.
	 */
	public void Setar(String nome, Cromossomo cromossomo)
	{
		this.nome = nome;
		this.cromossomo = cromossomo;
	}

	/**
	 * Processa os valores dos genes de acordo com
	 * os sensores e move o carro baseado neles.
	 */
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

	/**
	 * Reseta a física, posição e rotação do
	 * objeto para um ponto.
	 * 
	 * @param posicao - O ponto de reposicionamento do objeto.
	 */
    public void Reposicionar(Transform posicao)
    {
		Rigidbody rb       = controladorCarro.GetComponent<Rigidbody>();
		rb.velocity        = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		transform.position = posicao.position;
		transform.rotation = posicao.rotation;

		distanciaCarro.ResetarDistancia(posicao.position);
    }

	/**
	 * Abaixa a prioridade da câmera e fala pro
	 * algoritmo genético para morrer.
	 */
    public void Morrer()
	{
		if(!morto)
		{
			cameraCinemachine.Priority = 0;

			AlgoritmoGenetico alg = FindObjectOfType<AlgoritmoGenetico>();
			alg.MatarIndividuo(this);
		}
    }

	//
	// ─── COMPARADOR ─────────────────────────────────────────────────────────────────
	//

	public static int OrdenarPelaDistanciaPercorrida(Individuo i1, Individuo i2) 
	{
		float d1 = i1.distanciaCarro.getDistanciaPercorrida();
		float d2 = i2.distanciaCarro.getDistanciaPercorrida();

    	return d2.CompareTo(d1);
 	}

 	// ────────────────────────────────────────────────────────────────────────────────
}