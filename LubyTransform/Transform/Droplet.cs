using System;
using System.Collections.Generic;

namespace LubyTransform
{
	/**
 * Object that represents an encoded packet.
 * @author JosŽ Lopes
 *
 */

	public class Droplet : ICloneable 
	{

		/**
	 * Connections to source packets.
	 */
		public List<int> _neighbours;

		/**
	 * Seed used in the soliton distribution, when calculating the degree of this packet.
	 */
		public long Seed { get; set; }

		/**
	 * Degree of this packet.
	 */
		public int Degree { get; set; }

		/**
	 * The encoded content of this packet.
	 */
		private byte[] _data = null;

		/**
	 * Size of the encoded content.
	 */
		public int BlockSize { get; set; }

		/**
	 * Creates a new instance of <code>Block</code>.
	 * @param seed Value to be used as a seed for the soliton distribution.
	 * @param degree Degree of this packet.
	 * @param blockSize Size of the encoded content on the packet.
	 */
		public Droplet (long seed, int degree, int blockSize){
			this._neighbours = new List<int>();
			this.Seed = seed;
			this.Degree = degree;
			this.BlockSize = blockSize;
			this._data = new byte[blockSize];
		}

		/**
	 * Creates a new instance of <code>Block</code>, equal to another instance.
	 * @param b Instance of <code>Block</code>, from which the values of the new instance will be taken.
	 */
		public Droplet (Droplet b){
			this._neighbours = b.GetNeighbours();
			this.Seed = b.Seed;
			this.Degree = b.Degree;
			this.BlockSize = b.BlockSize;
			this._data = b.Data();
		}

		/**
	 * Returns (a copy of) the list of connections this packet has with the source packets.
	 * @return A copy of the list of connections this packet has with the source packets.
	 */
		public List<int> GetNeighbours() 
		{
			List<int> ret = new List<int> (_neighbours);

			return ret;
		}

		/**
	 * Sets a new list of connections with the source packets.
	 * @param neighbours New list of connections with the source packets - will be deep copied.
	 */
		public void SetNeighbours(List<int> neighbours) 
		{
			this._neighbours.Clear();

			foreach (int i in neighbours)
			{
				this._neighbours.Add(i);
			}
		}

		/**
	 * Sets the encoded content of this packet.
	 * @param d Encoded packet of this packet.
	 */
		public void setData(byte[] d)
		{
			_data = new byte[BlockSize];

			for(int i=0; i<d.Length; i++)
			{
				_data[i] = d[i];
			}
		}

		/**
	 * Returns the encoded content of this packet.
	 * @return The encoded content of this packet.
	 */
		public byte[] Data()
		{
			byte[] d = new byte[this._data.Length];

			for(int i=0; i<this._data.Length; i++)
			{
				d[i] = this._data[i];
			}

			return d;
		}

		/**
	 * Adds a new element to the list of connections this packet has with the source packets.
	 * @param i New element for the list of connections this packet has with the source packets.
	 */
		public void AddNeighbour(int i)
		{
			_neighbours.Add (i);
		}

		/**
	 * Removes a connection from the list of connections this packet has with the source packets.
	 * @param i Element (Object, not index) to be removed from the list of connections this packet has with the source packets.
	 */
		public void RemoveNeighbour(int i)
		{
			_neighbours.Remove (i);
		}

		public void RemoveNeighbourXOR(int j, byte[] block)
		{

			for (int k = 0; k < BlockSize; k++)
			{
				_data[k] = (byte) (_data[k] ^  block[k]);
			}

			_neighbours.Remove (j);
		}

		/**
	 * XORs the <code>input</code> given with the encoded content of this packet.
	 * @param input Input to be XORed with the encoded content of this packet. Will be truncated if bigger than 
	 * <code>blockSize</code>, and padded with '0's if shorter.
	 * @throws NullPointerException In case <code>data</code> or <code>input</code> are <code>null</code>.
	 */
		public void Xor (byte[] input) 
		{ 

			if(_data == null || input == null) 
			{
				throw new ArgumentNullException(); 
			}

			if(input.Length >= _data.Length)
			{
				for (int i=0; i<_data.Length; i++)
				{
					_data[i] = (byte)(_data[i] ^ input[i]);
				}
			}
			else
			{
				byte[] aux = new byte[_data.Length];
				Array.Copy (input, aux, _data.Length);

				for (int i=0; i<_data.Length; i++)
				{
					_data[i] = (byte)(_data[i] ^ aux[i]);
				}
			}
		}

		/**
	 * Returns a deep copy of this packet.
	 * @return A deep copy of this packet.
	 */
		public object Clone ()
		{
			return (new Droplet(this));
		}

	}
}

