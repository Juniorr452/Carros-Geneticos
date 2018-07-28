using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class AlgoritmoGenetico : MonoBehaviour 
{
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
	void Update () {
		
	}

	void inicializarPopulacao()
	{
		individuosMortos = 0;
		populacao = new List<Individuo>(tamanhoPopulacao);

		for(int i = 0; i < tamanhoPopulacao; i++){
			populacao.Add(InstanciarIndividuo());
		}
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
			Quaternion.identity);

		CinemachineVirtualCamera camera = Instantiate(cameraPrefab, 
			posicaoSpawn.position, 
			Quaternion.identity);

		camera.m_Follow = carro;
		camera.m_LookAt = carro;

		return carro.GetComponent<Individuo>();
	}
}