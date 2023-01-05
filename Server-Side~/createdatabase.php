<?php

// CREADEDATABASE.PHP
// INPUT: Serialized AdditionalFields
//        Preferences {AskUserForEmail, RequireEmailActivation, EnablePasswordRecovery, DropExistingTables}
// FUNCTIONALITY: Initializes the given database based on the prefs. The following tables are created:
//        'Accounts' {id, username, password, {AdditionalFields}, custominfo}
//             * Also adds an 'email' field if $askUserForEmail is true
//             * Also adds an 'isactive' field if $requireEmailActivation is true
//        'Confirm' {accountid, hash} - if $requireEmailActivation is true
//       https://cp1.atspace.me/beta/# 'PasswordReset' {accountid, hash} - if $enablePasswordRecovery is true
// OUTPUT: "success" if all went according to plan - an error message containing "error" otherwise


// And some helper functions
require('helper.php');

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

// Connect to server - EMPTY 4th argument (database name) since it might not exist yet
$link = try_mysql_connect($databaseHostname, $databaseUsername, $databasePassword, "", $databasePort);

$query = "CREATE DATABASE IF NOT EXISTS `$databaseDbName`";
try_mysql_query($link, $query);
mysqli_select_db($link, "$databaseDbName");



// Preferences sent from form
$requireEmailActivation = strtolower($_POST['requireEmailActivation']);
$enablePasswordRecovery = strtolower($_POST['enablePasswordRecovery']);
$overrideExistingTables = strtolower($_POST['overrideExistingTables']);


// To protect MySQL injection (more detail about MySQL injection)
$tableName = stripslashes($tableName);
$tableName = mysqli_real_escape_string($link, $tableName);



// Read the number of fields
$fields = explode( $fieldsSeparator, $_POST['fields'] );

$subqueryFields = "";

// For each field
foreach ($fields as $field){
	
	// Split the expression into
	$values = explode( $fieldNameValueSeparator, $field );
	
	// Protect vs MySQL injection
	$numValues = count($values);
	for ($j = 0; $j < $numValues; $j++ ){
		
		$values[$j] = stripslashes($values[$j]);
		$values[$j] = mysqli_real_escape_string($link, $values[$j]);        
	}
	
	// Store the itemid, owned and equipped values seperately
	$fieldName   = $values[0];
	$fieldType   = $values[1];
	$fieldMustBeUnique   = $values[2];
	$fieldIsRequired   = $values[3];
	$fieldComments   = $values[4];
	
	$subqueryField = "
  `$fieldName` ";
	$subqueryField .= $fieldType;
	
	if ($fieldComments != ""){
		
		$subqueryField .= " COMMENT " . "'$fieldComments'" ;
		
	}
	if ($fieldMustBeUnique == "true"){
		$subqueryField .= " UNIQUE";
	}       
	
	if ($fieldIsRequired == "true"){
		$subqueryField .= " NOT NULL";
	}
	
	$subqueryField .= ",";
	$subqueryFields .= $subqueryField;
	
}


if ($overrideExistingTables == "true") {
	try_mysql_query($link, "DROP TABLE IF EXISTS `accounts`");
}

$query = 
	"
CREATE TABLE `accounts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,"
	.$subqueryFields
	."
  
  PRIMARY KEY (`id`)
  
) ENGINE=MyISAM DEFAULT CHARSET=latin1 COMMENT='Created by AccountSystem' AUTO_INCREMENT=1 ;";

try_mysql_query($link, $query);

if ($requireEmailActivation == "true") {

	if ($overrideExistingTables == "true") {
		try_mysql_query($link, "DROP TABLE IF EXISTS `confirm`");
	}
	
	$query = 
		"
        CREATE TABLE `confirm` (
          `accountid` int(11) NOT NULL UNIQUE,
          `hash` varchar(255) NOT NULL,
          
          PRIMARY KEY (`accountid`),
          
          CONSTRAINT FOREIGN KEY ( `accountid` )
          REFERENCES `accounts`( `id` )
          ON DELETE CASCADE ON UPDATE CASCADE
          
        ) ENGINE=MyISAM DEFAULT CHARSET=latin1 COMMENT='Created by AccountSystem'  ;";
	
	try_mysql_query($link, $query);

	
}
if ($enablePasswordRecovery == "true") {

	if ($overrideExistingTables == "true") {
		try_mysql_query($link, "DROP TABLE IF EXISTS `passwordreset`");
	}
	
	$query = 
		"
        CREATE TABLE `passwordreset` (
          `accountid` int(11) NOT NULL UNIQUE,
          `hash` varchar(255) NOT NULL,
          
          PRIMARY KEY (`accountid`),
          
          CONSTRAINT FOREIGN KEY ( `accountid` )
          REFERENCES `accounts`( `id` )
          ON DELETE CASCADE ON UPDATE CASCADE
          
        ) ENGINE=MyISAM DEFAULT CHARSET=latin1 COMMENT='Created by AccountSystem'  ;";
	
	try_mysql_query($link, $query);

}

echo "success";

exit();

?>