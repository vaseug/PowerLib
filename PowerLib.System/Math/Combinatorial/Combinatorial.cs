using System;
using System.Collections.Generic;
using PowerLib.System.Collections;


namespace PowerLib.System.Math.Combinatorial
{
	/// <summary>
	/// Provides a set of static methods (extensions) to generate combinatorial sequences.
	/// </summary>
	public static class Combinatorial
	{
		/// <summary>
		///	Initialize first permutation.
		/// </summary>
		/// <param name="a">Array of permutation integers to initialize.</param>
		/// <returns>true if length of <paramref name="a"/> greater than 0, otherwise false.</returns>
		public static bool PermutationFirst(int[] a)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			//
			for (int i = 0; i < a.Length; i++)
				a[i] = i;
			return a.Length > 0;
		}

		/// <summary>
		/// Generate next permutation.
		/// </summary>
		/// <param name="a">Array of permutaion integers.</param>
		/// <returns>true if next permutation is generated, otherwise end is achieved.</returns>
		public static bool PermutationNext(int[] a)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			//
			for (int i = a.Length - 2; i >= 0; i--)
				if (a[i] < a[i + 1])
					for (int j = a.Length - 1; ; j--)
						if (a[j] > a[i])
						{
							int t = a[i];
							a[i] = a[j];
							a[j] = t;
							for (++i, j = a.Length - 1; i < j; i++, j--)
							{
								t = a[i];
								a[i] = a[j];
								a[j] = t;
							}
							return true;
						}
			return false;
		}

		/// <summary>
		/// Initialize first combination.
		/// </summary>
		/// <param name="a">Array of combination integers to initialize.</param>
		/// <returns>true if length of <paramref name="a"/> greater than 0, otherwise false.</returns>
		public static bool CombinationFirst(int[] a)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			//
			for (int i = 0; i < a.Length; i++)
				a[i] = i;
			return a.Length > 0;
		}

		/// <summary>
		/// Generate next combination.
		/// </summary>
		/// <param name="a">Array of combination integers.</param>
		/// <param name="n">Total elements number.</param>
		/// <returns>true if next combination is generated, otherwise end is achieved.</returns>
		public static bool CombinationNext(int[] a, int n)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			if (n < a.Length)
				throw new ArgumentOutOfRangeException("n");
			//
			int m = a.Length;
			for (int i = m - 1; i >= 0; i--)
			{
				if (a[i] < n + i - m)
				{
					a[i]++;
					while (++i < m)
						a[i] = a[i - 1] + 1;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Initialize first arrangement.
		/// </summary>
		/// <param name="a">Array of arrangement integers.</param>
		/// <returns>true if length of <paramref name="a"/> greater than 0, otherwise false.</returns>
		public static bool ArrangementFirst(int[] a)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			//
			for (int i = 0; i < a.Length; i++)
				a[i] = i;
			return a.Length > 0;
		}

		/// <summary>
		/// Generate next arrangement.
		/// </summary>
		/// <param name="a">Array of arrangement integers.</param>
		/// <param name="n">Total elements number.</param>
		/// <returns>true if next arrangement is generated, otherwise end is achieved.</returns>
		public static bool ArrangementNext(int[] a, int n)
		{
			if (PermutationNext(a))
				return true;
			for (int i = 0, j = a.Length - 1; i < j; i++, j--)
			{
				int t = a[i];
				a[i] = a[j];
				a[j] = t;
			}
			return CombinationNext(a, n);
		}

		/// <summary>
		/// Initialize first arrangement with repeates.
		/// </summary>
		/// <param name="a">Array of arrangement integers.</param>
		/// <returns>true if length of <paramref name="a"/> greater than 0, otherwise false.</returns>
		public static bool ArrangementWithRepeatsFirst(int[] a)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			//
			for (int i = 0; i < a.Length; i++)
				a[i] = 0;
			return a.Length > 0;
		}

		/// <summary>
		/// Generate next arrangement with repeates.
		/// </summary>
		/// <param name="a">Array of arrangement integers.</param>
		/// <param name="n">Total elements number.</param>
		/// <returns>true if next arrangement with repeates is generated, otherwise end is achieved.</returns>
		public static bool ArrangementWithRepeatsNext(int[] a, int n)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			//
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] < n - 1)
				{
					a[i]++;
					while (--i >= 0)
						a[i] = 0;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Initialize first subset.
		/// </summary>
		/// <param name="a">Array of subset boolean indicators of set element presence.</param>
		/// <returns>true if length of <paramref name="a"/> greater than 0, otherwise false.</returns>
		public static bool SubsetFirst(bool[] a)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			//
			for (int i = 0; i < a.Length; i++)
				a[i] = false;
			return a.Length > 0;
		}

		/// <summary>
		/// Generate next subset.
		/// </summary>
		/// <param name="a">Array of subset boolean indicators of set element presence.</param>
		/// <returns>true if next subset is generated, otherwise end is achieved.</returns>
		public static bool SubsetNext(bool[] a)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			//
			for (int i = 0; i < a.Length; i++)
				if (!a[i])
				{
					a[i] = true;
					while (--i >= 0)
						a[i] = false;
					return true;
				}
			return false;
		}

		/// <summary>
		/// Return inversion number of permutation specified by <paramref name="a"/>
		/// </summary>
		/// <param name="a">Array of permutation nnumbers.</param>
		/// <returns>Number of inversions.</returns>
		public static int PermutationInversion(this int[] a)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			//
			int invs = 0;
			int[] p = (int[])a.Clone();
			for (int i = 0; i < p.Length; i++)
			{
				int j = i;
				while (p[j] != i && j < p.Length)
					if (p[j++] > i)
						invs++;
				if (j < p.Length)
					p.Move(j, i);
			}
			return invs;
		}
	}
}
