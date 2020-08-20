<?php

// SETACCOUNTINFO.PHP
// INPUT: (id, info) via _POST
//      * info is a string containing field names and values in the following format:
//      fieldName0 $feildNameValueSeparator fieldValue0 $fieldValueSeparator 
//      fieldName1 $feildNameValueSeparator fieldValue1 $fieldValueSeparator
//      etc..
// FUNCTIONALITY: Updates each (field, value) pair in the 'accounts' table where the id matches the given id
// OUTPUT: "success" if all went according to plan - otherwise an error message containing "error"

// And some helper functions
require('helper.php');

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

// Connect to server and select databse.
$link = try_mysql_connect($databaseHostname, $databaseUsername, $databasePassword, $databaseDbName, $databasePort);

// username and password sent from form
$id=$_POST['id'];

// To protect MySQL injection (more detail about MySQL injection)
$id = stripslashes($id);
$id = mysqli_real_escape_string($link, $id);
$info = $_POST['info'];

// Check If the specified id exists
$query="SELECT * FROM accounts WHERE id = '$id'";
$result=try_mysql_query($link, $query);

// If it doesn't
if( mysqli_num_rows($result) ==0){
die( "MySQL error: Could not find the account with id " . $id );} 

$fields = explode($fieldsSeparator, $info);

foreach ($fields as $field) {

	$words = explode($fieldNameValueSeparator, $field);
	
	$fieldName  = $words[0];
	$fieldValue = $words[1];
	
	$fieldName = stripslashes($fieldName);
	$fieldValue = stripslashes($fieldValue);
	$fieldName  = mysqli_real_escape_string($link, $fieldName);
	$fieldValue = mysqli_real_escape_string($link, $fieldValue);
	
	if ($fieldName == "")
	continue;

	// Attempt to upload the info
	$query = "UPDATE accounts SET $fieldName = '$fieldValue' WHERE id = '$id'";        
	
        try_mysql_query($link, $query);

}

// All good
echo "success";

exit();



?>