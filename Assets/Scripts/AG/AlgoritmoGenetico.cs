using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class AlgoritmoGenetico : MonoBehaviour 
{
	//
	// ─── PARÂMETROS DO AG ───────────────────────────────────────────────────────────
	//

	[Range(3, 1000)]
	public int   tamanhoPopulacao         = 10;
	public float fatorMutacao             = 0.05f;

	[Range(2, 6)]
	public int   qtdIndividuosASelecionar = 2;

	//
	// ─── INFORMAÇÃO DOS INDIVÍDUOS ──────────────────────────────────────────────────
	//

	[SerializeField]
	private int geracaoAtual = 1;

	public  List<Individuo> populacao;

	[SerializeField]
	/**
	 * Indivíduos que passaram na fase de seleção.
	 */
	private List<Individuo> individuosSelecionados;

	[SerializeField]
	private int qtdIndividuosGerados = 0;

	[SerializeField]
	private int qtdIndividuosMortos;

	//
	// ─── GENES E LIMITES DOS VALORES ────────────────────────────────────────────────
	//

	// 5 primeiros são os sensores e o último é da velocidade do carro.
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

	//
	// ─── SPAWN, PREFABS E CORES DOS CARROS ───────────────────────────────────────────
	//

	// Define onde os indivíduos serão spawnados.
	public Transform posicaoSpawn;

	// Prefabs do carro e da câmera.
	public Transform carroPrefab;
	public CinemachineVirtualCamera cameraPrefab;

	// Cores dos carros em primeiro, segundo e outros lugares.
	public Color[] coresPosicoes = {
		new Color(.8113208f, .5610197f, 0, 1),      // Dourado
		new Color(.1585792f, .5660378f, 0, 1),      // Verde
		new Color(.764151f, .764151f, .764151f, 1), // Cinza
	};

	// ────────────────────────────────────────────────────────────────────────────────

	// Use this for initialization
	void Start () {
		inicializarPopulacao();
	}
	
	// Update is called once per frame
	void Update () {
		VerificarPosicoesCarro();
	}

	//
	// ─── FUNÇÕES BÁSICAS DO ALGORITMO GENÉTICO ──────────────────────────────────────
	//

	/**
	 * Cria e configura os indivíduos da primeira geração.
	 */
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
		Selecao();
		CrossOver();
		Mutacao(fatorMutacao);
		Elitismo();
		RespawnarCarros();

		geracaoAtual++;
		qtdIndividuosMortos = 0;
	}

	/**
	 * TODO: Fazer seleção com o 
	 * pseudocódigo da aula de seleção. 
	 */
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

	/**
	 * TODO: Fazer crossover com o 
	 * pseudocódigo da aula de crossover.
	 */
	void CrossOver()
	{
		// Selecionando só 2 indivíduos por enquanto...
		Cromossomo cromossomo1 = individuosSelecionados[0].cromossomo;
		Cromossomo cromossomo2 = individuosSelecionados[1].cromossomo;

		for(int i = 0; i < populacao.Count; i++)
		{
			Individuo individuo = populacao[i];
			BitArray[] genesCrossover = new BitArray[qtdGenes];

			Debug.Log(individuo.nome + ": " + individuo.cromossomo.ConverterBitArrayParaByte(individuo.cromossomo.genes[0]));

			for(int j = 0; j < qtdGenes; j++)
			{
				BitArray gene1 = cromossomo1.genes[j];
				BitArray gene2 = cromossomo2.genes[j];

				BitArray geneCrossover = new BitArray(8);

				for(int k = 0; k < gene1.Count; k++)
				{
					bool bitEscolhido;

					if(UnityEngine.Random.Range(0, 1) == 0)
						bitEscolhido = gene1[k];
					else
						bitEscolhido = gene2[k];

					geneCrossover[k] = bitEscolhido;
				}

				genesCrossover[j] = geneCrossover;
			}

			qtdIndividuosGerados++;
			Cromossomo cromossomoCrossover = new Cromossomo(genesCrossover);
			individuo.Setar("Individuo_" + qtdIndividuosGerados, cromossomoCrossover);

			Debug.Log(individuo.nome + ": " + individuo.cromossomo.ConverterBitArrayParaByte(individuo.cromossomo.genes[0]));
		}
	}

	/**
	 * TODO: Fazer o algoritmo de acordo com
	 * o pseudocódigo da aula de mutação.
	 */
	void Mutacao(float fatorMutacao)
	{
		for(int i = 0; i < populacao.Count; i++)
		{
			Individuo individuo = populacao[0];

			foreach(BitArray gene in individuo.cromossomo.genes)
				for(int j = 0; j < gene.Count; j++)
				{
					float random = UnityEngine.Random.Range(0.0f, 1.0f);

					// Se tiver gerado um valor dentro do fator de
					// mutação, faz um NOT no valor do bit.
					if(random <= fatorMutacao)
						gene[i] = !gene[i];
				}	
		}
	}

	/**
	 * Coloca os indivíduos selecionados na próxima geração.
	 */
	void Elitismo()
	{
		foreach(Individuo i in individuosSelecionados)
			populacao.Add(i);
	}

	/** 
	 * TODO: Parar 
	 * Parar a simulação quando o objetivo for atingido.
	 */
	void Parar()
	{

	}

	//
	// ─── INSTANCIAMENTO, RESPAWN E MORTE DO CARRO ───────────────────────────────────
	//

	/**
	 * Instacia um carro e câmera e configura
	 * a câmera para seguir ele.
	 */
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

	/**
	 * Reposiciona e reativa os carros.
	 */
	private void RespawnarCarros()
    {
        foreach(Individuo individuo in populacao)
		{
			individuo.Reposicionar(posicaoSpawn);
			individuo.morto = false;
			individuo.gameObject.SetActive(true);
		}
    }

	/**
	 * Desativa o gameobject, seta como morto,
	 * incrementa e verifica a quantidade de 
	 * indivíduos mortos para gerar a próxima gen.
	 * 
	 * @param individuo - O indivíduo a ser morto.
	 */
	public void MatarIndividuo(Individuo individuo)
	{
        individuo.gameObject.SetActive(false);
		individuo.morto = true;

		qtdIndividuosMortos++;

		if(qtdIndividuosMortos == tamanhoPopulacao)
			GerarProximaGeracao();
	}

	//
	// ─── POSIÇÕES DOS CARROS ────────────────────────────────────────────────────────
	//

	/**
	 * Verifica em quais posições os carros
	 * estão (Primeiro lugar, segundo lugar...)
	 * para dar uma cor específica a eles.
	 */
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
	// ────────────────────────────────────────────────────────────────────────────────
}