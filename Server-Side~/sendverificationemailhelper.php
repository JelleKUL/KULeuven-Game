<?php

// RESENDVERIFICATIONEMAIL.PHP
// INPUT: Serialized ACCOUNTINFO via _POST
// FUNCTIONALITY: Tries to register a new user.
// OUTPUT: A success message or an error message that contains "error"

// And some helper functions
require('helper.php');

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

// Connect to server and select databse.
$link = try_mysql_connect($databaseHostname, $databaseUsername, $databasePassword, $databaseDbName, $databasePort);

$from = $emailAccount;

$email = "mr.kon.vas@gmail.com";// $_POST['email'];
$email = mysqli_escape_string ($link, $email );

// Fetch the user's id
$query="SELECT id FROM accounts WHERE email='$email'";
$result=try_mysql_query($link, $query);   

$temp = mysqli_fetch_array($result);
$id = $temp['id'];    

// Generate random 32 character hash
$hash = mysqli_escape_string ($link, md5 ( rand (0, 1000) ) );


// Remove it from confirmation table 
$query="DELETE FROM confirm WHERE accountid = '$id'";
try_mysql_query($link, $query); 

// Store it into confirmation table
$query = "INSERT INTO confirm VALUES ('$id', '$hash')"; 
$result = try_mysql_query($link, $query); 

// Re-Insert slashes
$activationLink = $phpScriptsLocation.'/verify.php?id='.$id.'&hash='.$hash;

$subject = 'Account Verification'; // Give the email a subject
$message = '
Hello ' . $username . ',

thanks for signing up!

Click on the following link to activate your account:
'.$activationLink.'
'; // Our message above including the link

$headers = 'From: ' . $from . "\r\n" .
        'Reply-To: ' . $from  . "\r\n" .
        'X-Mailer: PHP/' . phpversion();

$to = str_replace('%40', '@', $email );

// Send our email   
if ( !mail($to, $subject, $message, $headers) ){
        
        // Remove it from confirmation table 
        $query="DELETE FROM confirm WHERE accountid = '$id'";
        try_mysql_query($link, $query); 
        
        $errorMessage  = "error: Could not send the following email:\n";
        $errorMessage .= "\nFROM: "    . $from;
        $errorMessage .= "\nTO: "      . $to;
        $errorMessage .= "\nSUBJECT: " . $subject;
        $errorMessage .= "\nMESSAGE: " . $message;
        $errorMessage .= "\nHEADERS: " . $headers;
        $errorMessage .= "\nAlso deleted the account and password reset request from 'passwordreset' - the password reset link is now broken.";
        $errorMessage .= "\nTo change that functionality edit REQUESTPASSWORDRESET.PHP at line 111.";
        die($errorMessage);
}

// Successful sent email
$successMessage  = "success - sent the following email\n";
$successMessage .= "\nFROM: "    . $from;
$successMessage .= "\nTO: "      . $to;
$successMessage .= "\nSUBJECT: " . $subject;
$successMessage .= "\nMESSAGE: " . $message;
$successMessage .= "\nHEADERS: " . $headers;
echo $successMessage;

exit();


?>