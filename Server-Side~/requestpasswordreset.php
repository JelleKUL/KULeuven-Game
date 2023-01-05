<?php
// REQUESTPASSWORDRESET.PHP
// INPUT: email via _POST
// FUNCTIONALITY: Tries to find an account with the specified email and then creates a one-time url link for RESETPASSWORD.HTML
// OUTPUT: "success" if all went according to plan or an error message containing "error"

// And some helper functions
require('helper.php');

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

// Connect to server and select databse.
$link = try_mysql_connect($databaseHostname, $databaseUsername, $databasePassword, $databaseDbName, $databasePort);

$from = $emailAccount;

// Get the id
$email=$_POST['email'];

// To protect MySQL injection (more detail about MySQL injection)
$email = stripslashes($email);
$email = mysqli_real_escape_string($link, $email);
$email = str_replace('%40', '@', $email );
$email = str_replace('%2b', '+', $email );

// Check If the specified username already exists,
$query="SELECT id, username, email FROM accounts WHERE email = '$email'";
$result= try_mysql_query($link, $query);

// If it doesn't 
if( mysqli_num_rows($result) ==0){
die( "MySQL error: Could not find the specified email - " . $email);} 

$temp = mysqli_fetch_array($result);

$id = $temp['id'];  
$username = $temp['username']; 

// Generate a random hash and sha1 it
$hash = sha1(uniqid(rand(), true));

// First check if it exists
$query="SELECT accountid FROM passwordreset WHERE accountid = '$id'";
$result= try_mysql_query($link, $query);

// If it does, 
if( mysqli_num_rows($result) != 0){

	// Update it with the new hash
	$query="UPDATE passwordreset SET hash = '$hash' WHERE accountid = '$id'";
        
        try_mysql_query($link, $query);
	
} 
// If it doesn't
else {

	// Insert it
	$query="INSERT INTO passwordreset VALUES ('$id', '$hash')";
        
	try_mysql_query($link, $query);	

}

$to       = $temp['email'];

// Re-Insert slashes
$resetLink    = $phpScriptsLocation . '/resetpassword.html?id=' . $id . '&hash=' . $hash;

$subject = 'Password Reset'; // Give the email a subject

$message = '
 
Hello ' . $username . ',

to reset your password, click on the following link:
'
	. $resetLink . '
 

'; // Our message above

$headers = 'From: ' . $emailAccount . "\r\n" .
	'Reply-To: ' . $emailAccount  . "\r\n" .
	'X-Mailer: PHP/' . phpversion();


$to = str_replace('%40', '@', $to );

// Send our email   
if ( !mail($to, $subject, $message, $headers) ){
	
	// Remove it from confirmation table 
	$query="DELETE FROM passwordreset WHERE accountid = '$id' and hash = '$hash'";
	try_mysql_query($link, $query); 
	
	$errorMessage  = "error: Could not send the following email:\n";
	$errorMessage .= "\nFROM: "    . $emailAccount;
	$errorMessage .= "\nTO: "      . $to;
	$errorMessage .= "\nSUBJECT: " . $subject;
	$errorMessage .= "\nMESSAGE: " . $message;
	$errorMessage .= "\nHEADERS: " . $headers;
	$errorMessage .= "\nAlso deleted the password reset request from 'passwordreset' - the password reset link is now broken.";
	$errorMessage .= "\nTo change that functionality edit REQUESTPASSWORDRESET.PHP at line 111.";
	die($errorMessage);
}

// Successful sent email
$successMessage  = "success - sent the following email\n";
$successMessage .= "\nFROM: "    . $emailAccount;
$successMessage .= "\nTO: "      . $to;
$successMessage .= "\nSUBJECT: " . $subject;
$successMessage .= "\nMESSAGE: " . $message;
$successMessage .= "\nHEADERS: " . $headers;
echo $successMessage;
exit();


?>