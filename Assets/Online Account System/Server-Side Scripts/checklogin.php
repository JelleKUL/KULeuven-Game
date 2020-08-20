<?php

// CHECKLOGIN.PHP
// INPUT: (username, password) via _POST
// FUNCTIONALITY: Tries to login with the provided username and password. 
// NOTES: If a "isactive" column is found for the requested account we check if it's set to true
// OUTPUT: "-2" if there was no match, "-1" if the account is inactive - otherwise returns the user's ID

// And some helper functions
require('helper.php'); 

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

// Connect to server and select databse.
$link = try_mysql_connect($databaseHostname, $databaseUsername, $databasePassword, $databaseDbName, $databasePort);

mysqli_set_charset($link, 'utf8');

// Preferences sent from form
$requireEmailActivation=$_POST['requireEmailActivation'];

// username and password sent from form
$username=$_POST['username'];
$password=$_POST['password'];


// To protect MySQL injection (more detail about MySQL injection)
$username = stripslashes($username);
$password = stripslashes($password);
$username = mysqli_real_escape_string($link, $username);
$password = mysqli_real_escape_string($link, $password);

$query="SELECT * FROM accounts WHERE username= '$username' and password='$password'";
$result=try_mysql_query($link, $query);

// Mysql_num_row is counting table row
$count=mysqli_num_rows($result);

// If result matched $username and $password, table row must be 1 row
if($count==1){
	
	$temp = mysqli_fetch_array($result);  

	// If the account is not activated and it should be
	if (isset($temp['isactive']) &  !$temp['isactive'])  {    
		die ("-1");
	}    
	// If it is, send back the id
	else {        
		echo $temp['id'];  
		exit();        
	}
}
// If we didn't match
else 
	die ("-2");
        

?>