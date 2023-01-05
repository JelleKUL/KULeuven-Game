<?php


// GETACCOUNTINFO.PHP
// INPUT: id via _POST
// FUNCTIONALITY: Tries to download all fields from the 'accounts' table where the id matches the given id. 
// OUTPUT: An echo containing fields separated by $fieldsSeparator.
//     * Each field is in the format:    $fieldName $fieldNameValueSeparator $fieldValue
// NOTES: Custom separators are required in order to allow the $fieldName 
// and $fieldValue variables to consist of any character the developer wants

// And some helper functions
require('helper.php');

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

// Connect to server and select databse.
$link = try_mysql_connect($databaseHostname, $databaseUsername, $databasePassword, $databaseDbName, $databasePort);


// ++++++++++++++ FIELD NAMES / TYPES +++++++++

// Get the field names      
$columns = try_mysql_query($link, "SHOW COLUMNS FROM accounts");

if (mysqli_num_rows($columns) == 0) {
	die( "MySQL error: Could not find any fields in accounts table");
}


// ++++++++++++++ ACCOUNT SPECIFIC +++++++++

$id=$_POST['id'];

// To protect MySQL injection (more detail about MySQL injection)
$id = stripslashes($id);
$id = mysqli_real_escape_string($link, $id);

$query = "SELECT * FROM accounts WHERE id = $id";
$result = try_mysql_query($link, $query); 

$row = mysqli_fetch_assoc($result);

while ($column = mysqli_fetch_assoc($columns)) {

	$name = $column['Field'];
	$type = $column['Type'];
	$null = $column['Null'];
	$key = $column['Key'];        
	//echo "name: " . $name . "type: " . $type . "null: " . $null . "key: " . $key . "default: " . $default . "extra: " . $extra;
	
	$echo  = $name;
	
	// Append value
	
	$echo .=  $fieldNameValueSeparator . $row[$name];
	
	// Append Type
	if (strpos($type, "tinyint") !== FALSE)
	$type = "BOOL";
	
	$echo .= $fieldNameValueSeparator . $type;
	
	// Append Must Be Unique (if it isn't a key, it doesn't have to be unique)
	if ($key == "")
	$echo .= $fieldNameValueSeparator . "false";
	else
		$echo .= $fieldNameValueSeparator . "true";
	
	// Append Is Required (if it can't be null it is Required
	if ($null == "NO")
	$echo .= $fieldNameValueSeparator . "true";
	else 
		$echo .= $fieldNameValueSeparator . "false";
	
	
	
	$echo .= $fieldsSeparator ;
	
	echo $echo;
}





exit();

?>