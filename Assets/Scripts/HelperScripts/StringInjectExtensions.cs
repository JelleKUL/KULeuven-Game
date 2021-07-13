using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using UnityEngine;
using System.Linq;


public static class StringInjectExtension
{
	public static string ParseVariables(this string str, object obj)
	{
		while (str.Contains("{"))
		{

			if (!str.Contains("}")) break;

			string substring = str.Substring(str.IndexOf("{") + 1, str.IndexOf("}") - str.IndexOf("{") - 1);
			str = str.Replace("{" + substring + "}", ParseVariablesValues(substring, obj));

			//Debug.Log("New string: " + str);
		}

		return str;
	}

	static string ParseVariablesValues(string subStr, object obj )
	{

		string val = "<b> ERROR: Invalid Variable: '" + subStr + "' </b>";
		int index = -1;
		bool scaleUp = false;

        if (subStr.Contains("*"))
        {
			//Debug.Log("Element contains a * scaling the output by the worldScaleFactor");
			subStr = subStr.Remove(subStr.IndexOf("*"), 1);
			scaleUp = true;
        }

		if (subStr.Contains("[") && subStr.Contains("]"))
		{
			string substring = subStr.Substring(0, subStr.IndexOf("["));
			int.TryParse(subStr.Substring(subStr.IndexOf("[") + 1, subStr.IndexOf("]") - subStr.IndexOf("[") - 1), out index);
			subStr = substring;
		}

		if (obj.GetType().GetField(subStr) != null)
        {
			if (index >= 0)
			{
				IEnumerable collection = (IEnumerable)obj.GetType().GetField(subStr).GetValue(obj);

				if (collection != null)
				{
					int count = 0;
					foreach (var thing in collection)
					{
						if (count == index)
						{
							if (scaleUp) val = (GameManager.RoundFloat((float)thing * GameManager.worldScale, 3)).ToString();
							else val = (GameManager.RoundFloat((float)thing, 3)).ToString();
							break;
						}
						count++;
					}
				}
				else
				{
					val = "<b> ERROR: Invalid Collection: '" + subStr + "' </b>";
				}
			}
			else
			{
				object thing = obj.GetType().GetField(subStr).GetValue(obj);

				if(float.TryParse(thing.ToString(), out float outPut))
                {
					if (scaleUp) val = (GameManager.RoundFloat(outPut * GameManager.worldScale, 3)).ToString();
					else val = (GameManager.RoundFloat(outPut, 3)).ToString();
				}
                else
                {
					val = thing.ToString();
                }
				
			}
		}
        else
        {
			Debug.LogWarning(obj.ToString() + ": " + val);
        }

		return val;
	}


	/// <summary>
	/// Extension method that replaces keys in a string with the values of matching object properties.
	/// <remarks>Uses <see cref="String.Format()"/> internally; custom formats should match those used for that method.</remarks>
	/// </summary>
	/// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
	/// <param name="injectionObject">The object whose properties should be injected in the string</param>
	/// <returns>A version of the formatString string with keys replaced by (formatted) key values.</returns>
	public static string Inject(this string formatString, object injectionObject)
	{
		return formatString.Inject(GetPropertyHash(injectionObject));
	}

	/// <summary>
	/// Extension method that replaces keys in a string with the values of matching dictionary entries.
	/// <remarks>Uses <see cref="String.Format()"/> internally; custom formats should match those used for that method.</remarks>
	/// </summary>
	/// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
	/// <param name="dictionary">An <see cref="IDictionary"/> with keys and values to inject into the string</param>
	/// <returns>A version of the formatString string with dictionary keys replaced by (formatted) key values.</returns>
	public static string Inject(this string formatString, IDictionary dictionary)
	{
		return formatString.Inject(new Hashtable(dictionary));
	}

	/// <summary>
	/// Extension method that replaces keys in a string with the values of matching hashtable entries.
	/// <remarks>Uses <see cref="String.Format()"/> internally; custom formats should match those used for that method.</remarks>
	/// </summary>
	/// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
	/// <param name="attributes">A <see cref="Hashtable"/> with keys and values to inject into the string</param>
	/// <returns>A version of the formatString string with hastable keys replaced by (formatted) key values.</returns>
	public static string Inject(this string formatString, Hashtable attributes)
	{
		string result = formatString;
		if (attributes == null || formatString == null)
			return result;

		foreach (string attributeKey in attributes.Keys)
		{
			result = result.InjectSingleValue(attributeKey, attributes[attributeKey]);
		}
		return result;
	}

	/// <summary>
	/// Replaces all instances of a 'key' (e.g. {foo} or {foo:SomeFormat}) in a string with an optionally formatted value, and returns the result.
	/// </summary>
	/// <param name="formatString">The string containing the key; unformatted ({foo}), or formatted ({foo:SomeFormat})</param>
	/// <param name="key">The key name (foo)</param>
	/// <param name="replacementValue">The replacement value; if null is replaced with an empty string</param>
	/// <returns>The input string with any instances of the key replaced with the replacement value</returns>
	public static string InjectSingleValue(this string formatString, string key, object replacementValue)
	{
		string result = formatString;
		//regex replacement of key with value, where the generic key format is:
		//Regex foo = new Regex("{(foo)(?:}|(?::(.[^}]*)}))");
		Regex attributeRegex = new Regex("{(" + key + ")(?:}|(?::(.[^}]*)}))");  //for key = foo, matches {foo} and {foo:SomeFormat}

		//loop through matches, since each key may be used more than once (and with a different format string)
		foreach (Match m in attributeRegex.Matches(formatString))
		{
			string replacement = m.ToString();
			if (m.Groups[2].Length > 0) //matched {foo:SomeFormat}
			{
				//do a double string.Format - first to build the proper format string, and then to format the replacement value
				string attributeFormatString = string.Format(CultureInfo.InvariantCulture, "{{0:{0}}}", m.Groups[2]);
				replacement = string.Format(CultureInfo.CurrentCulture, attributeFormatString, replacementValue);
			}
			else //matched {foo}
			{
				replacement = (replacementValue ?? string.Empty).ToString();
			}
			//perform replacements, one match at a time
			result = result.Replace(m.ToString(), replacement);  //attributeRegex.Replace(result, replacement, 1);
		}
		return result;

	}


	/// <summary>
	/// Creates a HashTable based on current object state.
	/// <remarks>Copied from the MVCToolkit HtmlExtensionUtility class</remarks>
	/// </summary>
	/// <param name="properties">The object from which to get the properties</param>
	/// <returns>A <see cref="Hashtable"/> containing the object instance's property names and their values</returns>
	private static Hashtable GetPropertyHash(object properties)
	{
		Hashtable values = null;
		if (properties != null)
		{
			values = new Hashtable();
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(properties);
			foreach (PropertyDescriptor prop in props)
			{

				values.Add(prop.Name, prop.GetValue(properties));

			}
		}
		return values;
	}

}

