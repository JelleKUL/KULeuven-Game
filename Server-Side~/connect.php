<?php
$link = mysqli_connect("$databaseHostname", "$databaseUsername", "$databasePassword", "$databaseDbName", $databasePort + 0);

if (!$link) {
    die('Connect Error (' . mysqli_connect_errno() . ') '
            . mysqli_connect_error());
}

?>