<?php

// SETUP.PHP
// INPUT: Database Credentials {Hostname, Port, Username, Password, Database Name} and - if provided Email Account
// FUNCTIONALITY: Updates the credential variables in each of the PHP files in the array $files
// OUTPUT: "success" if all went according to plan - otherwise an error message containing "error"

// Separators must be the same along AccountSystemHelper (C#) and [Set|Get]AccountInfo (php)

$fieldsSeparator         = "$#@(_fields_separator_*&%^";
$fieldNameValueSeparator = "$#@(_field_name_value_separator_*&%^";

$file = 'credentials.php';
	
//$data = file($file) or die ('error: Could not locate file: ' . $file); // reads an array of lines

$databaseHostname = $_POST['databaseHostname']; 
$databaseHostname = str_replace('%2f', '/', $databaseHostname );
$databasePort     = $_POST['databasePort']; 

$databaseUsername   = $_POST['databaseUsername']; 
$databasePassword   = $_POST['databasePassword']; 
$databaseDbName     = $_POST['databaseDbName']; 
$emailAccount       = $_POST['emailAccount']; 
$emailAccount	    = str_replace('%40', '@', $emailAccount);
$phpScriptsLocation = $_POST['phpScriptsLocation']; 
$phpScriptsLocation = str_replace('%2f', '/', $phpScriptsLocation );
$phpScriptsLocation = str_replace('%3a', ':', $phpScriptsLocation );

$linePhpScriptsLocation         = "$" . "phpScriptsLocation = " . '"' . $phpScriptsLocation . '"' . "; // Location of the PHP scripts". "\n";

$lineDatabaseHostname		= "$" . "databaseHostname   = " . '"' . $databaseHostname   . '"' . "; // Host name"      . "\n";
$lineDatabasePort		= "$" . "databasePort       = " . '"' . $databasePort       . '"' . "; // Port number"    . "\n";
$lineDatabaseUsername		= "$" . "databaseUsername   = " . '"' . $databaseUsername   . '"' . "; // Mysql username" . "\n";
$lineDatabasePassword		= "$" . "databasePassword   = " . '"' . $databasePassword   . '"' . "; // Mysql password" . "\n";
$lineDatabaseDbName	        = "$" . "databaseDbName     = " . '"' . $databaseDbName     . '"' . "; // Database name"  . "\n";
$lineEmailAccount               = "$" . "emailAccount       = " . '"' . $emailAccount       . '"' . "; // The account that sends out emails". "\n";

$lineFieldsSeparator            = "$" . "fieldsSeparator         = " . '"' . $fieldsSeparator         . '"' . "; // Separates MySQL fields in C#/PHP communication". "\n";
$lineNameValueSeparator         = "$" . "fieldNameValueSeparator = " . '"' . $fieldNameValueSeparator . '"' . "; // Separates MySQL field names & values in C#/PHP communication". "\n";

$data = "<?php\n";

$data .= $linePhpScriptsLocation;

$data .= $lineDatabaseHostname;
$data .= $lineDatabasePort;
$data .= $lineDatabaseUsername;
$data .= $lineDatabasePassword;
$data .= $lineDatabaseDbName;
$data .= $lineEmailAccount;

$data .= "// Separators must be the same along AccountSystemHelper (C#) and [Set|Get]AccountInfo (php)". "\n";
$data .= $lineFieldsSeparator;
$data .= $lineNameValueSeparator;

$data .= "?>";

echo($data);
file_put_contents($file, $data);

echo ('success');
exit();

?>