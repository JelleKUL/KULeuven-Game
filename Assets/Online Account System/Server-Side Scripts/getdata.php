<html>
 <head>
   <title>TopografieData</title>
 </head>
 <body>
<h2>Player Data:</h2>
<?php

// getadata.PHP
// INPUT:  view on webpage
// FUNCTIONALITY: shows all the player info 
// OUTPUT: a view of the player data

    // And some helper functions
require('helper.php');

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

// Connect to server and select databse.
$link = try_mysql_connect($databaseHostname, $databaseUsername, $databasePassword, $databaseDbName, $databasePort);

// ++++++++++++++ FIELD NAMES / TYPES +++++++++
$columns = try_mysql_query($link, "SHOW COLUMNS FROM accounts");


if (mysqli_num_rows($columns) == 0) {
	die( "MySQL error: Could not find any fields in accounts table");
}



$query = "SELECT * FROM accounts";
$result = mysqli_query($link, $query);

$amount = mysqli_num_rows($result);


if($amount > 0){

    while($row = mysqli_fetch_assoc($result)){
        $myXMLData = mb_convert_encoding($row["custominfo"], 'UTF-16', 'UTF-8');    
        $xml=simplexml_load_string($myXMLData) or die("Error: Cannot create object");
        
        echo "<h3>";
        echo $row["username"];
        echo "</h3> <h4>";
        echo $row["studentnr"];
        echo "</h4> <p>TotalScore: ";
        echo $xml->totalScore . "<br>";
        echo "Campaign 1 level: ";
        echo $xml->levelCamp1 . "<br>";
        $i = 0;
        foreach($xml->scoreCamp1->children() as $scoreCamp) { 
            echo "level " . $i . ": ";
            echo $scoreCamp . ", ";
            $i++; 
        }
        echo "<br>";
        echo "Campaign 2 level: ";
        echo $xml->levelCamp2 . "<br>";
        $i = 0;
        foreach($xml->scoreCamp2->children() as $scoreCamp) { 
            echo "level " . $i . ": ";
            echo $scoreCamp . ", ";
            $i++; 
        }
        echo "</p><hr>";
        
    }
    
}
else{
    echo("MySQL_ERROR 7: No entries in database");
}

exit();

?>
 </body>
</html>