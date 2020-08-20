<?php 

// VERIFY.PHP
// INPUT: (id, hash) via _GET
// FUNCTIONALITY: Verifies that the user with the provided ID has an inactive account and activates it.
// OUTPUT: "success" if all went according to plan - otherwise an error message containing "error"

// And some helper functions
require('helper.php');

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

// Connect to server and select databse.
$link = try_mysql_connect($databaseHostname, $databaseUsername, $databasePassword, $databaseDbName, $databasePort);


if(!isset($_GET['id']) || empty($_GET['id']) OR (!isset($_GET['hash']) || empty($_GET['hash']))){
        die( '<div class="statusmsg">Error: Invalid approach, please use the link that has been send to your email.</div>' );
}


// Id and hash sent from email
$id   = $_GET['id'];
$hash = $_GET['hash'];

// To protect MySQL injection (more detail about MySQL injection)
$id   = stripslashes($id);
$hash = stripslashes($hash);

$id   = mysqli_real_escape_string($link, $id);
$hash = mysqli_real_escape_string($link, $hash);

// Check If there is a match for the userid - hash combination
$query  = "SELECT * FROM confirm WHERE accountid='$id' AND hash='$hash'";
$result = try_mysql_query($link, $query) ;

// If we didn't find a match
if(mysqli_num_rows($result) == 0 ){

        die( '<div class="statusmsg">Error: The url is either invalid or you already have activated your account.</div>' );
     
}
// Else if there is
else {

        // Mark the user active
        $query = "UPDATE accounts SET isactive=TRUE WHERE id='$id'";
        try_mysql_query($link, $query) ;
        
        // Remove the key
        $query = "DELETE from confirm WHERE accountid='$id'";    
        try_mysql_query($link, $query) ;
        
        echo '<div class="statusmsg">Success: Your account has been activated, you can now login</div>';
}

exit();

?>