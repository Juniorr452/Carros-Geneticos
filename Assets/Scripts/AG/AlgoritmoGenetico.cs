using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class AlgoritmoGenetico : MonoBehaviour 
{
	[Range(3, 1000)]
	public int   tamanhoPopulacao         = 10;
	public float fatorMutacao             = 0.05f;
	[Range(2, 6)]
	public int   qtdIndividuosASelecionar = 2;

	[SerializeField]
	private int geracaoAtual = 1;

	public  List<Individuo> populacao;
	[SerializeField]
	private List<Individuo> individuosSelecionados;

	[SerializeField]
	private int qtdIndividuosGerados = 0;

	[SerializeField]
	private int qtdIndividuosMortos;

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
		new float[] {0, 200}                           // Velocidade do Carro
	};

	// Define onde os indivíduos serão spawnados
	public Transform posicaoSpawn;

	// Prefabs do carro e da câmera
	public Transform carroPrefab;
	public CinemachineVirtualCamera cameraPrefab;

	// Cores dos carros em primeiro, segundo e outros lugares
	public Color[] coresPosicoes = {
		new Color(.8113208f, .5610197f, 0, 1),
		new Color(.1585792f, .5660378f, 0, 1),
		new Color(.764151f, .764151f, .764151f, 1),
	};

	// Use this for initialization
	void Start () {
		inicializarPopulacao();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//if(Input.GetButtonDown("Jump"))
		//	GerarProximaGeracao();

		VerificarPosicoesCarro();
	}

    void inicializarPopulacao()
	{
		qtdIndividuosMortos = 0;

		populacao              = new List<Individuo>(tamanhoPopulacao);
		individuosSelecionados = new List<Individuo>();

		for(int i = 0; i < tamanhoPopulacao; i++, qtdIndividuosGerados++)
		{
			Individuo carro = InstanciarIndividuo();

			carro.nome       = "Individuo_" + qtdIndividuosGerados;
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

    // TODO: Seleção
    void Selecao()
	{
		populacao.Sort(Individuo.OrdenarPelaDistanciaPercorrida);

		individuosSelecionados.Clear();

		for(int i = 0; i < qtdIndividuosASelecionar; i++)
		{
			individuosSelecionados.Add(populacao[i]);
			populacao.RemoveAt(i);
		}
			
	}

	// TODO: CrossOver
	void CrossOver()
	{

	}

	// TODO: Mutação
	void Mutacao(float fatorMutacao)
	{
		for(int i = 0; i < populacao.Count; i++)
		{
			Individuo individuo = populacao[0];

			foreach(BitArray gene in individuo.cromossomo.genes)
				for(int j = 0; j < gene.Count; j++)
				{
					float random = UnityEngine.Random.Range(0, 1);

					// Se tiver gerado um valor dentro do fator de
					// mutação, faz um NOT no valor do bit.
					if(random <= fatorMutacao)
						gene[i] = !gene[i];
				}	
		}
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

		Individuo individuo         = carro.GetComponent<Individuo>();
		individuo.cameraCinemachine = camera;

		return carro.GetComponent<Individuo>();
	}

	private void RespawnarCarros()
    {
        foreach(Individuo individuo in populacao)
		{
			individuo.Reposicionar(posicaoSpawn);
			individuo.gameObject.SetActive(true);
			individuo.morto = false;
		}

		qtdIndividuosMortos = 0;

		Camera camera = FindObjectOfType<Camera>();
		camera.transform.position = posicaoSpawn.position;
		camera.transform.rotation = posicaoSpawn.rotation;
    }

	public void MatarIndividuo(Individuo individuo)
	{
        individuo.gameObject.SetActive(false);
		individuo.morto = true;

		qtdIndividuosMortos++;

		if(qtdIndividuosMortos == tamanhoPopulacao)
			GerarProximaGeracao();
	}

	private void VerificarPosicoesCarro()
    {
        populacao.Sort(Individuo.OrdenarPelaDistanciaPercorrida);

		for(int i = 0; i < populacao.Count; i++)
		{
			// Pegar as cores do primeiro, segundo e outras
			// posições, se houverem.
			int tamanhoCores = coresPosicoes.Length - 1;
			int indexCor     = i < tamanhoCores ? i : tamanhoCores;

			Individuo individuo = populacao[i];

			// Setar a cor do carro e a prioridade da
			// câmera (Para que possamos acompanhar ele)
			// de acordo com a posição.
			individuo.carroRenderer.material.color = coresPosicoes[indexCor];
			individuo.cameraCinemachine.Priority   = i == 0 && !individuo.morto ? 1 : 0;
		}
    }
}