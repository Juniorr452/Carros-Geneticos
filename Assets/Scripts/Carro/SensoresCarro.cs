using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensoresCarro : MonoBehaviour 
{
	// Sensores de distância entre o carro e as paredes
	public int qtdSensores = 5;
	private Vector3[] direcaoSensores;
	public  float[]   distanciaSensores;

	public Vector3 offsetRaycast = new Vector3(0, .5f, 0);

	public float tamanhoRaycast  = 75f;
	public bool  desenharRaycast = true;

	// Distância percorrida
	public  float   distanciaPercorrida = 0;
	private Vector3 posicaoAnterior;

	// Start is called on the frame when a script is enabled just before
	// any of the Update methods is called the first time.
	void Start()
	{
		distanciaSensores = new float[qtdSensores];
		direcaoSensores   = new Vector3[qtdSensores];

		posicaoAnterior = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		AtualizarDirecaoSensores();
		AtualizarDistanciaSensores();
		AtualizarDistanciaPercorrida();
	}

    private void AtualizarDistanciaSensores()
    {
        for(int i = 0; i < direcaoSensores.Length; i++)
		{
			RaycastHit hit;
			Vector3 direcao = direcaoSensores[i];

			Vector3 posInicialRaycast = transform.position + offsetRaycast;

			distanciaSensores[i] = tamanhoRaycast;
			if (Physics.Raycast(posInicialRaycast, direcao, out hit, tamanhoRaycast))
			{
				//TODO: Verificar se o alvo atingido é uma parede
				distanciaSensores[i] = hit.distance;

				if(desenharRaycast)
					Debug.DrawLine(posInicialRaycast, hit.point, Color.red, 0);
			}
		}
    }

    private void AtualizarDirecaoSensores()
    {
        // Esquerda
		direcaoSensores[0] = -transform.right;
		// Diagonal Esquerda
		direcaoSensores[1] = Quaternion.AngleAxis(-45, transform.up) * transform.forward;
		// Frente
		direcaoSensores[2] = transform.forward;
		// Diagonal Direita
		direcaoSensores[3] = Quaternion.AngleAxis(45, transform.up) * transform.forward;
		// Direita
		direcaoSensores[4] = transform.right;

		/*direcaoSensores = new Vector3[]{
			-transform.right,
			Quaternion.AngleAxis(-45, transform.up) * transform.forward,
			transform.forward,
			Quaternion.AngleAxis(45, transform.up) * transform.forward,
			transform.right
		};*/
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
}
