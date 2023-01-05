<?php


// GETREGISTRATIONFORM.PHP
// INPUT: -
// FUNCTIONALITY: Tries to download all field names from the 'accounts' table.
// OUTPUT: An echo containing fields separated by $fieldsSeparator.
//     * Each field is in the format:    $fieldName $fieldNameValueSeparator $fieldValue - for consistency with GETACCOUNTINFO.PHP
// NOTES: Custom separators are required in order to allow the $fieldName 
// variable to consist of any character the developer wants

// And some helper functions
require('helper.php');

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

// Connect to server and select databse.
$link = try_mysql_connect($databaseHostname, $databaseUsername, $databasePassword, $databaseDbName, $databasePort);


// Get the field names      
$result = try_mysql_query($link, "SHOW COLUMNS FROM accounts");

if (mysqli_num_rows($result) == 0) {
	die( "MySQL error: Could not find any fields in accounts table");
}

while ($row = mysqli_fetch_assoc($result)) {

	$name = $row['Field'];
	$type = $row['Type'];
	$null = $row['Null'];
	$key = $row['Key'];
	$default = $row['Default'];
	$extra = $row['Extra'];
	
	//echo "name: " . $name . "type: " . $type . "null: " . $null . "key: " . $key . "default: " . $default . "extra: " . $extra;
	
	$echo  = $name;
	
	// Append value (null)
	$echo .=  $fieldNameValueSeparator . "";
	
	// Append Type
	if (strpos($type, "tinyint") !== FALSE)
	$type = "BOOL";
	
	$echo .= $fieldNameValueSeparator . $type;
	
	// Append Must Be Unique (if it isn't a key, it doesn't have to be unique)
	if ($key == "")
	$echo .= $fieldNameValueSeparator . "false";
	else
		$echo .= $fieldNameValueSeparator . "true";
	
	// Append IsRequired (if it can't be null it is required
	if ($null == "NO")
	$echo .= $fieldNameValueSeparator . "true";
	else 
		$echo .= $fieldNameValueSeparator . "false";
	
	
	
	$echo .= $fieldsSeparator ;
	
	echo $echo;
}

exit();

?>