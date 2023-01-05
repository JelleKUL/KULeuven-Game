<?php
$phpScriptsLocation = "http://tgp-assets.atspace.eu/OnlineAccountSystem/OnlineAccountSystem"; // Location of the PHP scripts
$databaseHostname   = ""; // Host name
$databasePort       = ""; // Port number
$databaseUsername   = ""; // Mysql username
$databasePassword   = ""; // Mysql password
$databaseDbName     = ""; // Database name
$emailAccount       = ""; // The account that sends out emails
// Separators must be the same along AccountSystemHelper (C#) and [Set|Get]AccountInfo (php)
$fieldsSeparator         = "$#@(_fields_separator_*&%^"; // Separates MySQL fields in C#/PHP communication
$fieldNameValueSeparator = "$#@(_field_name_value_separator_*&%^"; // Separates MySQL field names & values in C#/PHP communication
?>