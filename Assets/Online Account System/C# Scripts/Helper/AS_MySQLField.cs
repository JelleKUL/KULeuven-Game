using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// MySQL field.
/// </summary>

[Serializable]
public class AS_MySQLField
{
    public AS_MySQLField() { }
    public AS_MySQLField(string _name, AS_MySQLFieldType _type, bool _mustBeUnique, bool _isRequired, string _comment = "")
    { name = _name; type = _type; mustBeUnique = _mustBeUnique; isRequired = _isRequired; comment = _comment; }
    public AS_MySQLField(string _name, string _stringValue, AS_MySQLFieldType _type, bool _mustBeUnique, bool _isRequired, string _comment = "")
    { name = _name; stringValue = _stringValue; type = _type; mustBeUnique = _mustBeUnique; isRequired = _isRequired; comment = _comment; }
	public AS_MySQLField(AS_MySQLField original)
	{ this.name = original.name; this.stringValue = original.stringValue; this.type = original.type; 
		this.mustBeUnique = original.mustBeUnique; this.isRequired = original.isRequired; this.comment = original.comment; }


    public string name = "";
    public string stringValue = "";
    public AS_MySQLFieldType type = AS_MySQLFieldType.Varchar;
    public bool mustBeUnique = false;
    public bool isRequired = false;
    public string comment = "";


}

public static class AS_MySQLFieldHelper
{
	
	public static bool CheckMySQLFields (this List<AS_MySQLField> fields, ref string errorMessage)
	{
		
		foreach (AS_MySQLField field in fields)
		{
			// These are not given by the user during registration
			if (field.name.ToLower() == "id" || field.name.ToLower() == "isactive" || field.name.ToLower() == "custominfo")
				continue;
			
			// Check for ommited fields
			if (field.isRequired && field.stringValue == "")
			{
				errorMessage = field.name.UppercaseFirst() + " can not be empty!";
				return false;
			}
			
			if (field.stringValue != "")
			{
				switch (field.type)
				{
					
					// INTEGER
				case AS_MySQLFieldType.Int:
					int n;
					if (!int.TryParse(field.stringValue, out n))
					{	
						errorMessage = field.name.UppercaseFirst() + " must be a natural number (1, 2, 5, 10, 1439, .. )!";
						return false;
					}
					break;
					
					// FLOAT
				case AS_MySQLFieldType.Float:
					float f;
					if (!float.TryParse(field.stringValue, out f))
					{	
						errorMessage = field.name.UppercaseFirst() + " must be a real number (1.2345, 3.14, .. )!";
						return false;
					}
					break;
					
					// DOUBLE
				case AS_MySQLFieldType.Double:
					double d;
					if (!double.TryParse(field.stringValue, out d))
					{	
						errorMessage = field.name.UppercaseFirst() + " must be a real number in scientific format: 1.2345E+123 !";
						return false;
					}
					break;
					
					// BOOL
				case AS_MySQLFieldType.Bool:
					int b;
					if (!int.TryParse(field.stringValue, out b))
					{	
						errorMessage = field.name.UppercaseFirst() + " must be either 0 or 1";
						return false;
					}
					if (b != 0 && b != 1)
					{
						errorMessage = field.name.UppercaseFirst() + " must be either 0 or 1";
						return false;
					}
					break;
					
				}// End switch
				
			}// End if (stringValue !="")
			
		}// End foreach(field);

		return true;
	}

    // THESE MUST BE THE SAME WITH AS_Helper and any PHP scripts that use them
    const string fieldsSeparator = "$#@(_fields_separator_*&%^";
    const string fieldNameValueSeparator = "$#@(_field_name_value_separator_*&%^";

    /// <summary>
    /// Adds my SQL fields.
    /// </summary>
    /// <returns>"" if there were no errors, otherwise the error message</returns>
    /// <param name="form">Form.</param>
    /// <param name="formFieldBaseName">Form field base name.</param>
    /// <param name="fields">Fields.</param>
    public static string AddMySQLFields(this WWWForm form, string formFieldsName, AS_MySQLField[] fields)
    {

        string mySQLString = fields.FormatAsMySQLString();

        if (mySQLString.ToLower().Contains("error"))
            return mySQLString;

        form.AddField(formFieldsName, mySQLString);
        Log(LogType.Log, mySQLString);

        return "";
    }


    private static string FormatAsMySQLString(this AS_MySQLField[] fields)
    {
        string mySQLString = "";

        if (fields == null)
        { 
            Log( LogType.Error, "Null argument passed");
            return "Error: Didn't pass any fields";
        }

        for (int i = 0; i < fields.Length; i++)
        {

            string tempMySQLString = fields[i].FormatAsMySQLString();

            if (tempMySQLString.ToLower().Contains("error"))
                return tempMySQLString;

            mySQLString += tempMySQLString;

            if (i < fields.Length - 1)
                mySQLString += fieldsSeparator;

        }

        return mySQLString;
    }

    private static string FormatAsMySQLString(this AS_MySQLField field)
    {

        string mySQLString = "";

        if (field.name == "")
        {
            Log( LogType.Error, "Field doesn't have a name");
            return "Error: Field doesn't have a name!";
        }

        // Name ~ Type ~ Unique ~ Required
        mySQLString = field.name;
        mySQLString += fieldNameValueSeparator;
        mySQLString += field.type.GetMySQLType();
        if (field.type == AS_MySQLFieldType.Varchar)
            mySQLString += "(255)";
        mySQLString += fieldNameValueSeparator;
        mySQLString += field.mustBeUnique ? "true" : "false";
        mySQLString += fieldNameValueSeparator;
        mySQLString += field.isRequired ? "true" : "false";
        mySQLString += fieldNameValueSeparator;
        mySQLString += field.comment;

        Log(LogType.Log, "Added field " + field.name + " (of Type: " + field.type.ToString() + ")");

        return mySQLString;
    }


    /// <summary>
    /// Static string Dictionary example
    /// </summary>
    static Dictionary<AS_MySQLFieldType, string> mySQLTypes = new Dictionary<AS_MySQLFieldType, string>
	{
		{AS_MySQLFieldType.Bool, 	"BOOL"},
		{AS_MySQLFieldType.Int, 	"INT"},
		{AS_MySQLFieldType.Float, 	"FLOAT"},
		{AS_MySQLFieldType.Double, 	"DOUBLE"},
		{AS_MySQLFieldType.Varchar, "VARCHAR"},
		{AS_MySQLFieldType.LongText,"LONGTEXT"}
		
	};

    /// <summary>
    /// Access the Dictionary from external sources
    /// </summary>
    public static string GetMySQLType(this AS_MySQLFieldType enumType)
    {
        // Try to get the result in the static Dictionary
        string result;
        if (mySQLTypes.TryGetValue(enumType, out result))
        {
            return result;
        }
        else
        {
            return null;
        }
    }

    public static AS_MySQLFieldType GetEnumType(this string mySQLType)
    {
        // Try to get the result in the static Dictionary

        foreach (AS_MySQLFieldType enumType in (AS_MySQLFieldType[])Enum.GetValues(typeof(AS_MySQLFieldType)))
            if (mySQLType.ToLower().Contains(enumType.ToString().ToLower()))
                return enumType;

        return AS_MySQLFieldType.UNSPECIFIED;

    }
    static void Log(LogType logType, string msg) { AS_Methods.Log(MethodBase.GetCurrentMethod().DeclaringType.Name, logType, msg); }
}


public enum AS_MySQLFieldType
{
    // 
    UNSPECIFIED,

    // Numeric:
    Bool, Int, Float, Double,

    // String:
    Varchar, LongText
}


