﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SecureFrameworkDemo.Framework {

	public static class HelpfulExtensions {

		/// <summary>
		/// Returns TRUE if the string contains the specified target, using a case-insensitive 
		/// Contains() operation.
		/// </summary>
		public static bool ContainsIgnoringCase(this string val, string target) {
			if (val == null)
				return false;

			return val.IndexOf(target, StringComparison.CurrentCultureIgnoreCase) >= 0;
		}

		public static T GetAttribute<T>(this ICustomAttributeProvider member) where T : class {
			return ((typeof(T) == typeof(object))
				? (T[])member.GetCustomAttributes(false)
				: (T[])member.GetCustomAttributes(typeof(T), false))
				.FirstOrDefault();
		}

		/// <summary>
		/// Returns TRUE if the target value is found in the specified list.
		/// </summary>
		public static bool IsIn<T>(this T val, IList<T> list) {
			if (list == null || list.Count == 0)
				return false;

			return list.Contains(val);
		}

		/// <summary>
		/// Returns TRUE if the target value is found in the specified list.
		/// </summary>
		public static bool IsIn<T>(this T val, params T[] list) {
			return val.IsIn(list.ToEmptyIfNull().ToList());
		}

		/// <summary>
		/// Returns TRUE if the string instance is null or refers to an empty string.
		/// </summary>
		public static bool IsNullOrEmpty(this string val) {
			return (val == null || val == String.Empty);
		}

		/// <summary>
		/// Returns TRUE if the string instance is neither null or an empty string.
		/// </summary>
		public static bool IsNotNullOrEmpty(this string val) {
			return val.IsNullOrEmpty() == false;
		}

		/// <summary>
		/// Concatenates all items in the list into a string, delimited by the specified
		/// delimiter.
		/// </summary>
		public static string Join(this IEnumerable list, string delimiter) {
			if (list == null)
				return String.Empty;

			var temp = new StringBuilder();

			foreach (object o in list) {
				if (o == null)
					continue;

				temp.Append(delimiter).Append(o.ToString());
			}

			// trim the leading delim and return
			string joinedString = temp.ToString();

			return (joinedString.Length == 0)
				? ""
				: joinedString.Substring(delimiter.Length);
		}

		/// <summary>
		/// Returns TRUE if the string is a match for the specified regular expression. 
		/// </summary>
		/// <param name="regex">May be specified in .NET style [just the pattern], or in Perl style like "/foo/i"</param>
		public static bool MatchesRegex(this string val, string regex) {
			return val.MatchesRegex(regex, RegexOptions.None);
		}

		/// <summary>
		/// Returns TRUE if the string is a match for the specified regular expression. 
		/// </summary>
		/// <param name="regex">May be specified in .NET style [just the pattern], or in Perl style like "/foo/i"</param>
		public static bool MatchesRegex(this string val, string regex, RegexOptions options) {
			MatchCollection ignored;

			return val.MatchesRegex(regex, options, out ignored);
		}

		/// <summary>
		/// Returns TRUE if the string is a match for the specified regular expression. Any matches
		/// are returned using an out parameter.
		/// </summary>
		/// <param name="regex">May be specified in .NET style [just the pattern], or in Perl style like "/foo/i"</param>
		public static bool MatchesRegex(this string val, string regex, out MatchCollection matches) {
			return val.MatchesRegex(regex, RegexOptions.None, out matches);
		}

		/// <summary>
		/// Returns TRUE if the string is a match for the specified regular expression. Any matches
		/// are returned using an out parameter.
		/// </summary>
		/// <param name="regex">May be specified in .NET style [just the pattern], or in Perl style like "/foo/i"</param>
		public static bool MatchesRegex(this string val, string regex, RegexOptions options, out MatchCollection matches) {
			if (val == null) {
				matches = null;
				return false;
			}

			var parsedRegex = RegexParser.Parse(regex, options);

			matches = parsedRegex.Matches(val);

			return matches.Count > 0;
		}

		/// <summary>
		/// Returns true if the string is a case-insensitive match for the specified regular 
		/// expression. 
		/// </summary>
		public static bool MatchesRegexIgnoringCase(this string val, string regex) {
			return Regex.IsMatch(val, regex, RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// Splits a string into an array using the specified string as the delimiter. This is 
		/// syntactic sugar for String.Split(), but allows the delimiter to be passed as a string
		/// instance rather than a character array.
		/// </summary>
		public static string[] Split(this string val, string singleCharacterDelimiter) {
			Contract.Requires(singleCharacterDelimiter.Length == 1, "The delimiter must be a single character.");

			if (String.IsNullOrEmpty(val))
				return new string[0];

			return val.Split(singleCharacterDelimiter.ToCharArray());
		}

		/// <summary>
		/// Syntactic sugar for Html.AttributeEncode(). Converts null input to an empty string, to allow for method chaining.
		/// </summary>
		public static string ToAttributeEncoded(this string val) {
			return HttpUtility.HtmlAttributeEncode(val ?? "");
		}

		/// <summary>
		/// Parses the string as a boolean, matching commonly used representations ("1", "Y", "no", etc)
		/// to their appropriate boolean p_value. Throws an exception if the string does not match
		/// any expected representation.
		/// </summary>
		public static bool ToBoolean(this string val) {
			Contract.Requires(val.IsNotNullOrEmpty(), "The string to parse cannot be null or empty.");

			bool result;

			if (Boolean.TryParse(val, out result))
				return result;
			else {
				switch (val.ToUpper()) {
					case "T":
					case "TRUE":
					case "Y":
					case "YES":
					case "1":
						return true;

					case "F":
					case "FALSE":
					case "N":
					case "NO":
					case "0":
						return false;
				}

				throw new FormatException("Can't parse '" + val + "' as a boolean.");
			}
		}

		/// <summary>
		/// Parses the string as a boolean, matching commonly used representations ("1", "Y", "no", etc)
		/// to their appropriate boolean p_value. 
		/// </summary>
		public static bool ToBoolean(this string val, bool defaultVal) {
			if (val.IsNullOrEmpty()) {
				return defaultVal;
			}

			try {
				return val.ToBoolean();
			}
			catch (FormatException) {
				return defaultVal;
			}
		}

		/// <summary>
		/// Returns the original sequence or a new, empty list.
		/// </summary>
		public static IList<T> ToEmptyIfNull<T>(this IList<T> list) {
			return (list == null)
				? new List<T>()
				: list;
		}

		/// <summary>
		/// Converts a string into an instance of the specified Enum. The p_value is compared
		/// first against the enum's ToString() p_value, then against its StringConstant 
		/// attribute, and finally against it's Description attribute. If no matches are found,
		/// an ArgumentException is thrown.
		/// </summary>
		public static T ToEnum<T>(this string val, bool ignoreCase = true) where T : struct {
			return (T)Enum.Parse(typeof(T), val, ignoreCase: ignoreCase);
		}

		/// <summary>
		/// Converts a string into an instance of the specified Enum. The p_value is compared
		/// first against the enum's ToString() p_value, then against its StringConstant 
		/// attribute, and finally against it's Description attribute. If no matches are found,
		/// the specified default is returned.
		/// </summary>
		public static T ToEnum<T>(this string val, T ifParseFails) where T : struct {
			try {
				return val.ToEnum<T>();
			}
			catch (ArgumentException) {
				return ifParseFails;
			}
		}

		/// <summary>
		/// Converts a comma-delimited list of strings into an Enum list
		/// </summary>
		public static List<T> ToEnumList<T>(this string val) where T : struct {
			var list = new List<T>();

			if (val == null)
				return list;

			foreach (string s in val.Split(',')) {
				var trimmed = s.Trim();

				if (trimmed.IsNullOrEmpty())
					continue;

				list.Add(trimmed.ToEnum<T>());
			}

			return list;
		}

		/// <summary>
		/// Converts a list of strings into a list of enum instances. Each item in the
		/// list is converted using ToEnum(), and any values that cannot be converted
		/// are silently ignored.
		/// </summary>
		public static List<T> ToEnumList<T>(this IEnumerable<string> strings) where T : struct {
			var list = new List<T>();

			if (strings == null)
				return list;

			foreach (string s in strings) {
				if (s.IsNullOrEmpty())
					continue;

				list.Add(s.ToEnum<T>());
			}

			return list;
		}

		public static Guid ToGuid(this string p_value) {
			return new Guid(p_value);
		}

		public static Guid ToGuid(this string p_value, Guid p_defaultVal) {
			try {
				return p_value.ToGuid();
			}
			catch (FormatException) {
				return p_defaultVal;
			}
		}

		public static string ToStringNullSafe(this object val, string defaultVal) {
			if (val == null)
				return defaultVal;

			var stringRepresentation = val.ToString();

			return (stringRepresentation == String.Empty)
				? defaultVal
				: stringRepresentation;
		}
	}
}