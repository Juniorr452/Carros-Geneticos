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
	public int tamanhoPopulacao = 10;

	[Range(0, 1)]
	public float fatorMutacao = 0.05f;

	[Range(2, 6)]
	public int qtdIndividuosASelecionar = 2;

	[Range(1, 100)]
	public int qtdVezesRoleta = 10;

	//
	// ─── INFORMAÇÃO DOS INDIVÍDUOS ──────────────────────────────────────────────────
	//

	public int geracaoAtual = 1;

	public List<Individuo> populacao;

	/**
	 * Indivíduos que passaram na fase de seleção.
	 */
	[SerializeField]
	private List<Individuo> individuosSelecionados;

	[SerializeField]
	private int qtdIndividuosGerados = 0;

	public int qtdIndividuosMortos;

	//
	// ─── GENES E LIMITES DOS VALORES ────────────────────────────────────────────────
	//

	/**
	 * Cromossomo 01 - A partir de qual valor do sensor o 
	 * 	carro deve se mover.
	 * 
	 * Cromossomo 02 - Pesos dos genes do cromossomo 01.
	 * 
	 * Cromossomo 03 - Se o valor calculado a partir dos 
	 * 	cromossomos 01 e 02 deve ser aplicado na 
	 * 	horizontal ou vertical.
	 */
	private const int qtdCromossomos = 3;

	/**
	 * Quantidade de genes por cromossomo.
	 */
	private const int qtdGenes = 7;

	[SerializeField]
	private float[][][] limitesInfSupCromo = new float[qtdCromossomos][][]
	{
		new float[qtdGenes][] {
			new float[] {0, SensoresCarro.tamanhoRaycast / 2}, // Sensor Parede Diagonal Esquerda
			new float[] {0, SensoresCarro.tamanhoRaycast / 2}, // Sensor Parede Frente
			new float[] {0, SensoresCarro.tamanhoRaycast / 2}, // Sensor Parede Diagonal Direita

			new float[] {SensoresCarro.tamanhoRaycast / 2, SensoresCarro.tamanhoRaycast}, // Sensor Parede Diagonal Esquerda 2
			new float[] {SensoresCarro.tamanhoRaycast / 2, SensoresCarro.tamanhoRaycast}, // Sensor Parede Frente 2
			new float[] {SensoresCarro.tamanhoRaycast / 2, SensoresCarro.tamanhoRaycast}, // Sensor Parede Diagonal Direita 2

			new float[] {10, 63} // Velocidade do Carro
		},
		/**
		 * Esses valores serão substraídos por 31.
		 * 
		 * Não coloquei negativo pq estamos codificando
		 * em binário unsigned.
		 */
		new float[qtdGenes][]{
			new float[] {0, 63},
			new float[] {0, 63},
			new float[] {0, 63},
			new float[] {0, 63},
			new float[] {0, 63},
			new float[] {0, 63},
			new float[] {0, 63}
		},
		new float[qtdGenes][]{
			new float[] {0, 50},
			new float[] {0, 50},
			new float[] {0, 50},
			new float[] {0, 50},
			new float[] {0, 50},
			new float[] {0, 50},
			new float[] {0, 50}
		}
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

			carro.nome = "Individuo_" + qtdIndividuosGerados;

			// Inicializar cada cromossomo.
			Cromossomo[] cromossomos = new Cromossomo[qtdCromossomos];
			for(int j = 0; j < qtdCromossomos; j++)
				cromossomos[j] = new Cromossomo(qtdGenes, limitesInfSupCromo[j]);
		
			carro.cromossomos = cromossomos;

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
	 * Seleção de indivíduos utilizando o método da roleta.
	 */
    void Selecao()
	{
		List<float> porcentagens = new List<float>();
		float pAnterior = 0;

		populacao.Sort(Individuo.OrdenarPelaPontuacao);

		individuosSelecionados.Clear();

		// Calcular o fitness total.
		float fitnessTotal = calcularFitnessGeral();

		for(int i = 0; i < tamanhoPopulacao; i++)
		{
			pAnterior += populacao[i].calcularFitness() / fitnessTotal;
			porcentagens.Add(pAnterior);
		}

		// Girar a roleta.
		for(int i = 0; i < qtdVezesRoleta; i++)
		{
			float random = UnityEngine.Random.Range(0f, 1f);

			for(int j = 0; j < tamanhoPopulacao; j++)
				if(random <= porcentagens[j])
				{
					populacao[j].pontuacaoSelecaoRoleta++;
					break;
				}	
		}
			
		/**
		 * Ordenar e selecionar os indivíduos que obtiveram
		 * as maiores pontuações na roleta.
		 */
		populacao.Sort(Individuo.OrdenarPelaPontuacaoSelecao);

		for (int i = 0; i < qtdIndividuosASelecionar; i++)
		{
			Individuo ind = populacao[0];
			individuosSelecionados.Add(populacao[0]);
			populacao.RemoveAt(0);
		}
	}

	private float calcularFitnessGeral()
	{
		float fitnessSoma = 0;

		foreach(Individuo individuo in populacao)
			fitnessSoma += individuo.calcularFitness();
		
		return fitnessSoma;
	}

	/**
	 * Cruzamento de ponto único.
	 */
	void CrossOver()
	{
		// Selecionando só 2 indivíduos por enquanto...
		Cromossomo[] cromossomos1 = individuosSelecionados[0].cromossomos;
		Cromossomo[] cromossomos2 = individuosSelecionados[1].cromossomos;
		
		foreach(Individuo individuo in populacao)
		{
			Cromossomo[] cromossomosCrossover = new Cromossomo[qtdCromossomos];

			// Para cada cromossomo
			for(int i = 0; i < qtdCromossomos; i++)
			{
				BitArray[] genesCrossover = new BitArray[qtdGenes];

				// Pegar os cromossomos dos 2 indivíduos selecionados.
				Cromossomo cromossomo1 = cromossomos1[i];
				Cromossomo cromossomo2 = cromossomos2[i];

				// Para cada gene
				for(int j = 0; j < qtdGenes; j++)
				{
					int ponto = UnityEngine.Random.Range(0, Cromossomo.qtdBits);
					
					BitArray gene1 = cromossomo1.genes[j];
					BitArray gene2 = cromossomo2.genes[j];

					BitArray geneCrossover = new BitArray(6);

					// Antes e depois do ponto
					for(int k = 0; k < ponto; k++)
						geneCrossover[k] = gene1[k];

					for(int k = ponto; k < gene1.Count; k++)
						geneCrossover[k] = gene2[k];
					
					genesCrossover[j] = geneCrossover;
				}

				cromossomosCrossover[i] = new Cromossomo(genesCrossover);
			}

			qtdIndividuosGerados++;
			individuo.Setar("Individuo_" + qtdIndividuosGerados, cromossomosCrossover);
		}
	}

	/**
	 * Mutação de indivíduos com flip de bits.
	 */
	void Mutacao(float fatorMutacao)
	{
		foreach(Individuo individuo in populacao)
			foreach(Cromossomo cromossomo in individuo.cromossomos)
				foreach(BitArray gene in cromossomo.genes)
				{
					//Debug.Log("Ant: " + cromossomo.ConverterBitArrayParaString(cromossomo.genes[0]));

					for(int i = 0; i < gene.Count; i++)
					{
						float random = UnityEngine.Random.Range(0.0f, 1.0f);

						// Se tiver gerado um valor dentro do fator de
						// mutação, faz um NOT no valor do bit.
						if(random <= fatorMutacao)
							gene[i] = !gene[i];
					}

					//Debug.Log("Dps: " + cromossomo.ConverterBitArrayParaString(cromossomo.genes[0]));
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
        populacao.Sort(Individuo.OrdenarPelaPontuacao);

		for(int i = 0; i < populacao.Count; i++)
		{
			Individuo individuo = populacao[i];

			if(individuo.morto)
			{
				individuo.cameraCinemachine.Priority = -1;
				continue;
			}
				
			// Pegar as cores do primeiro, segundo e outras
			// posições, se houverem.
			int tamanhoCores = coresPosicoes.Length - 1;
			int indexCor     = i < tamanhoCores ? i : tamanhoCores;

			// Setar a cor do carro e a prioridade da
			// câmera (Para que possamos acompanhar ele)
			// de acordo com a posição.
			individuo.carroRenderer.material.color = coresPosicoes[indexCor];
			individuo.cameraCinemachine.Priority   = populacao.Count - i;
		}
    }
	// ────────────────────────────────────────────────────────────────────────────────
}