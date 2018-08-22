using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Cromossomo possui a codificação dos genes em binário.
 */
public class Cromossomo
{
	public BitArray[] genes;

	/**
	 * Quantidade de bits para cada gene.
	 */
	private const int qtdBits = 6;

	//
	// ─── CONSTRUTORES ───────────────────────────────────────────────────────────────
	//

	public Cromossomo(BitArray[] genesCodificados){
		this.genes = genesCodificados;
	}
	
	public Cromossomo(int qtdGenes, float[][] limitesInfSup)
	{
		genes = new BitArray[qtdGenes];

		codificar(limitesInfSup, qtdBits);
	}

	//
	// ─── CODIFICAÇÃO E DECODIFICAÇÃO ────────────────────────────────────────────────
	//

	/**
	 * Baseado no pseudocódigo fornecido
	 * na aula de codificação.
	 */
	private void codificar(float[][] limitesInfSup, int qtdBits)
	{
		for(int i = 0; i < genes.Length; i++)
		{
			float inf = limitesInfSup[i][0];
			float sup = limitesInfSup[i][1];

			float random = UnityEngine.Random.Range(inf, sup);

			float aux = ((random - inf) / (sup - inf)) * (Mathf.Pow(2, qtdBits) - 1);

			genes[i] = ConverterFloatParaBitArray(random, qtdBits);
		}
	}

	/**
	 * Baseado no pseudocódigo fornecido
	 * na aula de codificação.
	 */
	public int[] decodificar()
	{
		int[] genesInt = new int[genes.Length];

		for(int i = 0; i < genes.Length; i++)
			genesInt[i] = ConverterBitArrayParaInt(genes[i]);

		return genesInt;
	}

	//
	// ─── CONVERSORES ────────────────────────────────────────────────────────────────
	//

	/**
	 * https://stackoverflow.com/questions/45759398/byte-to-bitarray-and-back-to-byte
	 */
	public int ConverterBitArrayParaInt(BitArray bits)
	{
		/**
		 * New int[1] pq estamos usando apenas 8 bits
		 * para codificar nossos genes.
		 */
		int[] valoresEmInt = new int[1];
		bits.CopyTo(valoresEmInt, 0);
		return valoresEmInt[0];
	}

	public String ConverterBitArrayParaString(BitArray bits)
	{
		String s = "";

		/**
		 * Tem que percorrer ao contrário porque ele
		 * armazena os bits menos significativos
		 * nos primeiros índices.
		 * 
		 * https://stackoverflow.com/questions/9066831/bitarray-returns-bits-the-wrong-way-around
		 */
		for(int i = bits.Length - 1; i >= 0; i--)
		{
			bool bit = bits[i];

			if(bit) s += "1";
			else    s += "0";
		}

		return s;
	}

	private BitArray ConverterFloatParaBitArray(float f, int qtdBits)
	{
		byte aux = Convert.ToByte(f);

		BitArray baux = new BitArray(new byte[]{ aux });
		BitArray bits = new BitArray(qtdBits);

		for(int i = 0; i < qtdBits; i++)
			bits[i] = baux[i];

		return bits;
	}
	// ────────────────────────────────────────────────────────────────────────────────
}