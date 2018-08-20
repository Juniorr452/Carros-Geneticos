using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensoresCarro : MonoBehaviour 
{
	//
	// ─── PARÂMETROS DOS SENSORES ────────────────────────────────────────────────────
	//

	private       int     qtdSensores     = 3;
	public static float   tamanhoRaycast  = 50f;
	public        bool    desenharRaycast = true;

	/**
	 * Esse Vector3 será somado com a posição
	 * do objeto para os sensores não ficarem
	 * muito perto do chão.
	 */
	public Vector3 offsetRaycast = new Vector3(0, .5f, 0);

	/**
	 * Máscara para o sensor detectar apenas
	 * as paredes.
	 */
	public LayerMask layerMaskSensor;

	//
	// ─── SENSORES DE DISTÂNCIA ENTRE O CARRO E AS PAREDES ───────────────────────────
	//
		
	private Vector3[] direcaoSensores;
	public  float[]   distanciaSensores;

	// ────────────────────────────────────────────────────────────────────────────────

	// Start is called on the frame when a script is enabled just before
	// any of the Update methods is called the first time.
	void Start()
	{
		distanciaSensores = new float[qtdSensores];
		direcaoSensores   = new Vector3[qtdSensores];
	}
	
	// Update is called once per frame
	void Update () 
	{
		AtualizarDirecaoSensores();
		AtualizarDistanciaSensores();
	}

	/**
	 * Dispara os sensores até uma determinada distância
	 * e verifica se atingiu algo.
	 */
    private void AtualizarDistanciaSensores()
    {
        for(int i = 0; i < direcaoSensores.Length; i++)
		{
			RaycastHit hit;
			Vector3 direcao = direcaoSensores[i];

			Vector3 posInicialRaycast = transform.position + offsetRaycast;

			distanciaSensores[i] = tamanhoRaycast;
			if (Physics.Raycast(posInicialRaycast, direcao, out hit, tamanhoRaycast, layerMaskSensor))
			{	
				distanciaSensores[i] = hit.distance;

				if(desenharRaycast)
					Debug.DrawLine(posInicialRaycast, hit.point, Color.red, 0);
			}
		}
    }

	/**
	 * Atualiza a direção dos sensores de acordo com
	 * a orientação atual do objeto.
	 */
    private void AtualizarDirecaoSensores()
    {
		// Diagonal Esquerda
		direcaoSensores[0] = Quaternion.AngleAxis(-45, transform.up) * transform.forward;
		// Frente
		direcaoSensores[1] = transform.forward;
		// Diagonal Direita
		direcaoSensores[2] = Quaternion.AngleAxis(45, transform.up) * transform.forward;
    }
}
