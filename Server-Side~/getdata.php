<?php

// getadata.PHP
// INPUT:  view on webpage
// FUNCTIONALITY: shows all the player info 
// OUTPUT: a view of the player data

    // And some helper functions
require('helper.php');

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

include('CSVDownload.php');

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

    $dataArray = array();

    $newArray = array();
    array_push($newArray, "Date");
    array_push($newArray, "studentnr");
    array_push($newArray, "username");
    array_push($newArray, "totalScore");
    
    array_push($dataArray, $newArray);

    while($row = mysqli_fetch_assoc($result)){
        $myXMLData = mb_convert_encoding($row["custominfo"], 'UTF-16', 'UTF-8');    
        $xml=simplexml_load_string($myXMLData) or die("Error: Cannot create object");
        
        $newArray = array();

        array_push($newArray, $row["creationdate"]);
        array_push($newArray, $row["studentnr"]);
        array_push($newArray, $row["username"]);
        array_push($newArray, $xml->totalScore);
        array_push($newArray, "Campaign 1:");
        foreach($xml->scoreCamp1->children() as $scoreCamp) { 
            array_push($newArray, $scoreCamp);
        }
        array_push($newArray, "Campaign 2:");
        foreach($xml->scoreCamp2->children() as $scoreCamp) { 
            array_push($newArray, $scoreCamp); 
        }
        array_push($newArray, "NewData:");
        foreach($xml->chapters->children() as $chapterInfo) { 
            array_push($newArray, $chapterInfo->UID);
            foreach($chapterInfo->scores->children() as $score) { 
                array_push($newArray, $score);
            }
        }

        array_push($dataArray, $newArray);
    }
    //print_r($dataArray);

    array_to_csv_download($dataArray, "data.csv");
    
/*
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
*/
    
}
else{
    echo("MySQL_ERROR 7: No entries in database");
}

exit();



?>