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
	private int individuosMortos;

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

		for(int i = 0; i < tamanhoPopulacao; i++){
			// TODO: Inicializar o gene aleatoriamente
			populacao.Add(InstanciarIndividuo());
		}
	}

	public void GerarProximaGeracao()
	{
		// TODO:
		
		Selecao();
		CrossOver();
		Mutacao(fatorMutacao);
		ReposicionarCarros();

		geracaoAtual++;
	}

    private void ReposicionarCarros()
    {
        foreach(Individuo individuo in populacao)
			individuo.Reposicionar(posicaoSpawn);
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
}