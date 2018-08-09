using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Mostra as informações do algoritmo 
 * genético na tela.
 */
public class UIAGInfo : MonoBehaviour 
{
	private AlgoritmoGenetico algGen;

	public Text geracaoAtualText;
	public Text qtdIndividuosVivosText;

	// Use this for initialization
	void Start () {
		algGen = FindObjectOfType<AlgoritmoGenetico>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		int genAtual           = algGen.geracaoAtual;
		int populacao          = algGen.tamanhoPopulacao;
		int qtdIndividuosVivos = populacao - algGen.qtdIndividuosMortos;

		geracaoAtualText.text       = "Gen " + algGen.geracaoAtual;
		qtdIndividuosVivosText.text = qtdIndividuosVivos + "/" + populacao;
	}
}
