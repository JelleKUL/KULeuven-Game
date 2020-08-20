<?php


// GETSIMILARACCOUNTS.PHP
// INPUT: fieldNameToCheck, fieldValueToMatch, fieldNameToReturn
// FUNCTIONALITY: Tries to download the 'fieldNameToReturn' from all accounts whose 'fieldNameToCheck' matches 'fieldValueToMatch'
// OUTPUT: An echo containing the matched accounts' 'fieldNameToReturn', separated by $fieldsSeparator

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

$fieldNameToCheck = $_POST['fieldNameToCheck'];
$fieldValueToMatch = $_POST['fieldValueToMatch'];
$fieldNameToReturn = $_POST['fieldNameToReturn'];

// _____TEST_____
// $fieldNameToCheck = 'age';
// $fieldValueToMatch = '24';
// $fieldNameToReturn = 'username';


// To protect MySQL injection (more detail about MySQL injection)
$fieldNameToCheck = stripslashes($fieldNameToCheck);
$fieldNameToCheck = mysqli_real_escape_string($link, $fieldNameToCheck);

$fieldValueToMatch = stripslashes($fieldValueToMatch);
$fieldValueToMatch = mysqli_real_escape_string($link, $fieldValueToMatch);

$fieldNameToReturn = stripslashes($fieldNameToReturn);
$fieldNameToReturn = mysqli_real_escape_string($link, $fieldNameToReturn);

$query = "SELECT $fieldNameToReturn FROM accounts WHERE $fieldNameToCheck = '$fieldValueToMatch'";
$result = try_mysql_query($link, $query); 

if ($result) {

    /* fetch associative array */
    while ($row = mysqli_fetch_assoc($result)) {
        echo $row[$fieldNameToReturn] . $fieldsSeparator;
    }

    /* free result set */
    mysqli_free_result($result);
}


exit();

?>