using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanciaPercorrida : MonoBehaviour 
{
	[SerializeField]
	private float   distanciaPercorrida = 0;
	private Vector3 posicaoAnterior;

	// Use this for initialization
	void Start () {
		posicaoAnterior = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		AtualizarDistanciaPercorrida();
	}

	// Calcula o quanto o carro andou de um ponto a outro e soma
	// com a distância percorrida.
	private void AtualizarDistanciaPercorrida()
    {
		Vector3 posicaoAtual = transform.position;

		float distanciaCalculada = (posicaoAtual - posicaoAnterior).magnitude;

		posicaoAnterior = posicaoAtual;

		// Pra impedir que pequenas variações no movimento
		// não sejam levadas em consideração.
		if(distanciaCalculada > 0.01f)
        	distanciaPercorrida += distanciaCalculada;
    }

	public void ResetarDistancia(Vector3 posicao)
	{
		distanciaPercorrida = 0f;
		posicaoAnterior     = posicao;
	}

	public float getDistanciaPercorrida(){
		return distanciaPercorrida;
	}
}