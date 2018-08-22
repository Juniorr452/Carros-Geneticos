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
	public Cromossomo[] cromossomos;

	public int pontuacaoSelecaoRoleta = 0;

	public bool morto = false;

	//
	// ─── SENSORES E CONTROLADOR ─────────────────────────────────────────────────────
	//

	/**
	 * Para controlar o carro (fazer virar, acelerar...).
	 */
	private CarController controladorCarro;

	private CalculadorPontuacao calculadorPontuacao;
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

	/**
	 * Intervalo de checagem pra ver se o carro está
	 * andando ou não para matar.
	 */
	private const float intervaloCheckMovendo = 1f;

	// ────────────────────────────────────────────────────────────────────────────────

	// Use this for initialization
	void Start () 
	{
		controladorCarro    = GetComponent<CarController>();
		calculadorPontuacao = GetComponent<CalculadorPontuacao>();
		sensoresCarro       = GetComponent<SensoresCarro>();

		InvokeRepeating("VerificarSeEstaMovendo", intervaloCheckMovendo * 3, intervaloCheckMovendo);
	}

	/**
	 * Função chamada a cada X segundos para
	 * verificar se o carro está ou não se movendo.
	 * 
	 * Se estiver parado, ela mata o indivíduo.
	 */
	void VerificarSeEstaMovendo()
	{
		if(controladorCarro.CurrentSpeed <= 2)
			Morrer();
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
	 * @param cromossomos - Os novos cromossomos do indivíduo.
	 */
	public void Setar(String nome, Cromossomo[] cromossomos)
	{
		this.nome        = nome;
		this.cromossomos = cromossomos;
	}

	/**
	 * Processa os valores dos genes de acordo com
	 * os sensores e move o carro baseado neles.
	 * 
	 * TODO: Pensar melhor numa função de mover
	 * pro carro.
	 * 
	 */
    private void Mover()
    {
        float horizontal = 0f;
		float vertical   = 0f;

		int qtdCromossomos = cromossomos.Length;
		int qtdGenes       = cromossomos[0].genes.Length;

		//
		// ─── PEGAR OS SENSORES ───────────────────────────────────────────
		//

		float[] sensores = new float[qtdGenes];

		for(int i = 0; i < (qtdGenes - 1) / 2; i++)
			sensores[i] = sensoresCarro.distanciaSensores[i];

		//
		// ─── PEGAR OS GENES ──────────────────────────────────────────────
		//

		int[][] genes = new int[cromossomos.Length][];

		for(int i = 0; i < cromossomos.Length; i++)
			genes[i] = cromossomos[i].decodificar();

		int[] geneLimite = genes[0];
		int[] genePeso   = genes[1];
		int[] geneHV     = genes[2];

		//
		// ─── COMPUTAR OS VALORES HORIZONTAIS E VERTICAIS ─────────────────
		//

		for(int i = 0, j = 3; i < (qtdGenes - 1) / 2; i++, j++)
		{
			float v = 0;

			if(sensores[i] <= geneLimite[i])
				v = formula(sensores[i], geneLimite[i], genePeso[i]);
			else if(sensores[i] > geneLimite[j])
				v = formula(sensores[i], geneLimite[j], genePeso[j]);

			if(geneHV[i] < 25) 
				horizontal += v;
			else               
				vertical   += v;
		}

		// Computar velocidade
		int indexVel = qtdGenes - 1;
		if(controladorCarro.CurrentSpeed < geneLimite[indexVel])
			horizontal -= formula(sensores[indexVel], geneLimite[indexVel], genePeso[indexVel]);

		/*if(sensorEsquerda <= genes[0])
			horizontal += (genes[0] - sensorEsquerda) / genes[0] * 2;

		if(genes[1] <= sensorDigEsquerda)
		{
			float v = (genes[1] - sensorDigEsquerda) / limiteSensor;
			horizontal += v;
			vertical   -= v;
		}

		if(genes[2] <= sensorFrente)
			vertical -= (genes[2] - sensorFrente) / limiteSensor * 2;
		
		if(genes[3] <= sensorDigDireita)
		{
			float v = (genes[3] - sensorDigDireita) / limiteSensor;
			horizontal -= v;
			vertical   -= v;
		}

		if(genes[4] <= sensorDireita)
			horizontal -= (genes[4] - sensorDireita) / limiteSensor * 2;

		if(genes[5] <= velocidade)
			vertical += (genes[5] - velocidade) / limiteVelocidade;*/

		//
		// ─── APLICAR NO CARRO ────────────────────────────────────────────
		//

		// TODO: Não deixar o carro dar ré.
		controladorCarro.Move(horizontal, vertical, vertical, 0f);
		//controladorCarro.Move(steering, accel, footbrake, handbrake);
    }

	private float formula(float sensor, float limite, float peso){
		return (limite - sensor) / limite * (peso - 1);
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

		// Resetar pontuação do carro e pontuação da roleta.
		calculadorPontuacao.pontuacao = 0;
		this.pontuacaoSelecaoRoleta = 0;
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

	public float calcularFitness(){
		return calculadorPontuacao.pontuacao;
	}

	//
	// ─── COMPARADOR ─────────────────────────────────────────────────────────────────
	//

	public static int OrdenarPelaPontuacao(Individuo i1, Individuo i2) 
	{
		float p1 = i1.calculadorPontuacao.pontuacao;
		float p2 = i2.calculadorPontuacao.pontuacao;

    	return p2.CompareTo(p1);
 	}

	public static int OrdenarPelaPontuacaoSelecao(Individuo i1, Individuo i2) 
	{
		float d1 = i1.pontuacaoSelecaoRoleta;
		float d2 = i2.pontuacaoSelecaoRoleta;

    	return d2.CompareTo(d1);
 	}

 	// ────────────────────────────────────────────────────────────────────────────────
}