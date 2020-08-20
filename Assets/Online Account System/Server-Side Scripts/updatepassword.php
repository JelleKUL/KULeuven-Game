<?php

// UPDATEPASSWORD.PHP
// INPUT: (id, newpassword, hash) via _POST
// FUNCTIONALITY: Verifies that there was indeed a password reset request and then 
// proceeds with updating the password in the 'accounts' table where the id matches the given id
// OUTPUT: "success" if all went according to plan - otherwise an error message containing "error"

// And some helper functions
require('helper.php');

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

// Connect to server and select databse.
$link = try_mysql_connect($databaseHostname, $databaseUsername, $databasePassword, $databaseDbName, $databasePort);

// username and password sent from form
$id = $_POST['id'];
$newpassword = $_POST['newpassword'];
$hash = $_POST['hash'];

// To protect MySQL injection (more detail about MySQL injection)
$id = stripslashes($id);
$newpassword = stripslashes($newpassword);
$hash = stripslashes($hash);

$id = mysqli_real_escape_string($link, $id);
$newpassword = mysqli_real_escape_string($link, $newpassword);
$hash = mysqli_real_escape_string($link, $hash);

// Check If the specified id exists
$query="SELECT * FROM accounts WHERE id = '$id'";
$result = try_mysql_query($link, $query);

// If it doesn't
if( mysqli_num_rows($result) ==0){

	//die( "MySQL error: Could not find the account with id " . $id );
	die( "The password reset you requested has some invalid info.\n\nPlease follow the link from your email, or request another password reset." );
} 

$temp = mysqli_fetch_array($result);
$username = $temp['username'];

// Check If he requested a password reset specified id exists
$query="SELECT * FROM passwordreset WHERE accountid = '$id'";

$result = try_mysql_query($link, $query);

// If he didn't
if( mysqli_num_rows($result) ==0){

	//die( "MySQL error: Account with id " . $id . " has not requested a password reset." );
	die( "$username, we can not find a password reset request for your account.\n\nPlease try again." );
} 

$temp = mysqli_fetch_array($result);


// If the hashes do not match
if ($hash != $temp['hash']){
	//die( "MySQL error: Invalid id / hash combination!" );
	die( "The password reset you requested has some invalid info.\n\nPlease follow the link from your email, or request another password reset." );
} 

// Create the query
$hashedPassword = hash('sha512', $newpassword);

$query = "UPDATE accounts SET password = '$hashedPassword' WHERE id = '$id'";

// Attempt to upload the info
try_mysql_query($link, $query);

// Create the query
$query = "DELETE FROM passwordreset WHERE accountid = '$id'";

// Attempt to upload the info
$result = try_mysql_query($link, $query);

echo "$username, your password has been updated successfully!";

exit();



?>