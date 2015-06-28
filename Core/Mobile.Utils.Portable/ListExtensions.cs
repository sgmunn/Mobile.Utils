//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ListExtensions.cs" company="(c) Greg Munn">
//    (c) 2015 (c) Greg Munn  All Rights Reserved
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace System
{
	public static class ListExtensions
	{
		public static void AddRange<T> (this IList<T> list, IList<T> otherList)
		{
			if (otherList != null) {
				foreach (var item in otherList) {
					list.Add (item);
				}
			}
		}

		public static void ForEach<T> (this IEnumerable<T> list, Action<T> action)
		{
			if (action != null) {
				foreach (var item in list) {
					action (item);
				}
			}
		}
	}
}