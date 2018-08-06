using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cromossomo
{
	public byte[] genes;

	private const int qtdBits = 8;
	
	public Cromossomo(int qtdGenes, float[][] limitesInfSup)
	{
		genes = new byte[qtdGenes];

		codificar(limitesInfSup, qtdBits);
	}

	private void codificar(float[][] limitesInfSup, int qtdBits)
	{
		for(int i = 0; i < genes.Length; i++)
		{
			float inf = limitesInfSup[i][0];
			float sup = limitesInfSup[i][1];

			float random = UnityEngine.Random.Range(inf, sup);

			float aux = ((random - inf) / (sup - inf)) * (Mathf.Pow(2, qtdBits) - 1);

			byte baux = Convert.ToByte(random);

			genes[i] = baux;
		}
	}

	public byte[] decodificar()
	{
		return genes;
	}
}