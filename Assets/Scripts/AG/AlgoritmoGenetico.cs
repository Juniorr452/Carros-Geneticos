using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class AlgoritmoGenetico : MonoBehaviour 
{
	[SerializeField]
	private int geracaoAtual = 1;
	private float fatorMutacao = 0.05f;

	List<Individuo> populacao;

	[Range(1, 1000)]
	public int tamanhoPopulacao = 10;

	[SerializeField]
	private int individuosGerados = 0;

	[SerializeField]
	private int individuosMortos;

	// Limites superior e inferior para os genes
	// 5 primeiros são os sensores e o último é da velocidade do carro
	private const int qtdGenes = 6;
	[SerializeField]
	private float[][] limitesInfSupCromo = new float[qtdGenes][]
	{
		new float[] {0, SensoresCarro.tamanhoRaycast}, // Sensor Parede Esquerda
		new float[] {0, SensoresCarro.tamanhoRaycast}, // Sensor Parede Diagonal Esquerda
		new float[] {0, SensoresCarro.tamanhoRaycast}, // Sensor Parede Frente
		new float[] {0, SensoresCarro.tamanhoRaycast}, // Sensor Parede Diagonal Direita
		new float[] {0, SensoresCarro.tamanhoRaycast}, // Sensor Parede Direita
		new float[] {0, 200} // Velocidade do Carro
	};

	// Define onde os indivíduos serão spawnados
	public Transform posicaoSpawn;

	// Prefabs do carro e da câmera
	public Transform carroPrefab;
	public CinemachineVirtualCamera cameraPrefab;

	// Use this for initialization
	void Start () {
		inicializarPopulacao();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Jump"))
			GerarProximaGeracao();
	}

	void inicializarPopulacao()
	{
		individuosMortos = 0;
		populacao = new List<Individuo>(tamanhoPopulacao);

		for(int i = 0; i < tamanhoPopulacao; i++, individuosGerados++)
		{
			Individuo carro = InstanciarIndividuo();

			carro.nome       = "Individuo_" + individuosGerados;
			carro.cromossomo = new Cromossomo(qtdGenes, limitesInfSupCromo);

			populacao.Add(carro);
		}
	}

	public void GerarProximaGeracao()
	{
		// TODO:
		
		Selecao();
		CrossOver();
		Mutacao(fatorMutacao);
		RespawnarCarros();

		geracaoAtual++;
	}

    private void RespawnarCarros()
    {
        foreach(Individuo individuo in populacao)
		{
			individuo.Reposicionar(posicaoSpawn);
			individuo.gameObject.SetActive(true);
			individuo.morto = false;
		}

		individuosMortos = 0;
    }

    // TODO: Seleção
    void Selecao()
	{

	}

	// TODO: CrossOver
	void CrossOver()
	{

	}

	// TODO: Mutação
	void Mutacao(float fatorMutacao)
	{

	}

	// TODO: Parar
	void Parar()
	{

	}

	Individuo InstanciarIndividuo()
	{
		Transform carro = Instantiate(carroPrefab, 
			posicaoSpawn.position, 
			posicaoSpawn.rotation);

		CinemachineVirtualCamera camera = Instantiate(cameraPrefab, 
			posicaoSpawn.position, 
			posicaoSpawn.rotation);

		camera.m_Follow = carro;
		camera.m_LookAt = carro;

		return carro.GetComponent<Individuo>();
	}

	public void MatarIndividuo(Individuo individuo)
	{
        individuo.gameObject.SetActive(false);
		individuo.morto = true;

		individuosMortos++;

		if(individuosMortos == tamanhoPopulacao)
			GerarProximaGeracao();
	}
}