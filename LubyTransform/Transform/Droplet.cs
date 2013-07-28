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
	 * Degree of this packet.
	 */
		public int Degree { get; set; }

		/**
	 * The encoded content of this packet.
	 */
		private byte[] _data;

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
		public Droplet (int degree, int blockSize){
			_neighbours = new List<int>();
			Degree = degree;
			BlockSize = blockSize;
			_data = new byte[blockSize];
		}

		/**
	 * Creates a new instance of <code>Block</code>, equal to another instance.
	 * @param b Instance of <code>Block</code>, from which the values of the new instance will be taken.
	 */
		public Droplet (Droplet b){
			_neighbours = b.Neighbours;
			Degree = b.Degree;
			BlockSize = b.BlockSize;
			_data = b.Data;
		}

		/**
	 * Returns (a copy of) the list of connections this packet has with the source packets.
	 * @return A copy of the list of connections this packet has with the source packets.
	 */
		public List<int> Neighbours
		{
			get
			{
				List<int> ret = new List<int> (_neighbours);

				return ret;
			}
		}

		/**
	 * Returns the encoded content of this packet.
	 * @return The encoded content of this packet.
	 */
		public byte[] Data
		{
			get
			{
				return _data;
			}
			set
			{
				for(int i=0; i<_data.Length; i++)
				{
					_data[i] = value[i];
				}
			}
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


		/**
	 * Returns a deep copy of this packet.
	 * @return A deep copy of this packet.
	 */
		public object Clone ()
		{
			return (new Droplet(this));
		}

		public override bool Equals (object o)
		{
			Droplet d = (Droplet)o;
			if (Degree != d.Degree)
			{
				return false;
			}

			for (int neighbourIndex=0; neighbourIndex<Neighbours.Count; neighbourIndex++)
			{
				if (Neighbours.Contains (d.Neighbours[neighbourIndex]) == false)
				{
					return false;
				}
			}

			return true;
		}

	}
}

