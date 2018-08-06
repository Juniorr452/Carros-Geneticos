using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cromossomo
{
	public BitArray[] genes;

	private const int qtdBits = 8;
	
	public Cromossomo(int qtdGenes, float[][] limitesInfSup)
	{
		genes = new BitArray[qtdGenes];

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

			genes[i] = new BitArray(new byte[] { baux });
		}
	}

	// TODO:
	public byte[] decodificar()
	{
		byte[] bytes = new byte[genes.Length];

		for(int i = 0; i < genes.Length; i++)
			bytes[i] = ConverterBitArrayParaByte(genes[i]);

		return bytes;
	}

	// https://stackoverflow.com/questions/45759398/byte-to-bitarray-and-back-to-byte
	private byte ConverterBitArrayParaByte(BitArray bits)
	{
		var bytes = new byte[1];
    	bits.CopyTo(bytes, 0);
    	return bytes[0];
	}

	private void imprimirBitArray(BitArray bits)
	{
		String s = "";

		// Tem que percorrer ao contrário porque ele
		// armazena os bits menos significativos 
		// nos primeiros índices.
		// https://stackoverflow.com/questions/9066831/bitarray-returns-bits-the-wrong-way-around
		for(int i = bits.Length - 1; i >= 0; i--)
		{
			bool bit = bits[i];

			if(bit) s += "1";
			else    s += "0";
		}

		Debug.Log("bits: " + s);
	}
}