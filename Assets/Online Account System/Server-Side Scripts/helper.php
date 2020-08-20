<?php

function try_mysql_connect($hostname, $username, $password, $databaseName, $databasePort)
{
        $link = mysqli_connect("$hostname", "$username", "$password", "$databaseName", $databasePort + 0);
        if (mysqli_connect_errno($link)) {
            die('Connect Error (' . mysqli_connect_errno($link) . ') '
                    . mysqli_connect_error($link));
        }
        return $link;
}

function try_mysql_query($link, $query)
{
        $result=mysqli_query($link, $query);

        // Check for errors
        if(mysqli_errno($link)){
                die( "MySQL error " . mysqli_errno($link) . " (" . mysqli_error($link) . ")\n<br>When executing <br>\n$query\n<br>");
        }
        
        return $result;

}
?>