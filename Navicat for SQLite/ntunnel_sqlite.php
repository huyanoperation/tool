<?php  //version lite1
header("Content-Type: application/octet-stream");

error_reporting(0);
set_time_limit(0);
set_magic_quotes_runtime(0);

define(FileDoesNotExist, 0);
define(FileCannotBeOpened, 1);
define(FileIsSQLite2, 2);
define(FileIsSQLite3, 3);

function GetLongBinary($num)
{
	return pack("N",$num);
}

function GetShortBinary($num)
{
	return pack("n",$num);
}

function GetDummy($count)
{
	$str = "";
	for($i=0;$i<$count;$i++)
		$str .= "\x00";
	return $str;
}

function GetBlock($val)
{
	$len = strlen($val);
	if( $len < 254 )
		return chr($len).$val;
	else
		return "\xFE".GetLongBinary($len).$val;
}

function EchoHeader($errno)
{
	$str = GetLongBinary(1111);
	$str .= GetShortBinary(1);
	$str .= GetLongBinary($errno);
	$str .= GetDummy(6);
	echo $str;
}

function EchoConnInfo()
{
	$version = sqlite_libversion();
	$str = GetBlock($version);
	$str .= GetBlock($version);
	$str .= GetBlock($version);
	echo $str;
}

function EchoConnInfo3()
{
	$version = SQLite3::version();
	$str = GetBlock($version["versionString"]);
	$str .= GetBlock($version["versionString"]);
	$str .= GetBlock($version["versionString"]);
	echo $str;
}

function EchoResultSetHeader($errno, $affectrows, $insertid, $numfields, $numrows)
{
	$str = GetLongBinary($errno);
	$str .= GetLongBinary($affectrows);
	$str .= GetLongBinary($insertid);
	$str .= GetLongBinary($numfields);
	$str .= GetLongBinary($numrows);
	$str .= GetDummy(12);
	echo $str;
}

function EchoFieldsHeader($res, $numfields)
{
	$str = "";
	for ( $i = 0; $i < $numfields; $i++ ) {
		$str .= GetBlock(sqlite_field_name($res, $i));
		$str .= GetBlock("");
		
		$type = -2;	// SQLITE_TEXT
		$length = 0;
		$flag = 0;
		
		$str .= GetLongBinary($type);
		$str .= GetLongBinary($flag);
		$str .= GetLongBinary($length);
	}
	echo $str;
}

function EchoFieldsHeader3($res, $numfields)
{
	$str = "";
	for ( $i = 0; $i < $numfields; $i++ ) {
		$str .= GetBlock($res->columnName($i));
		$str .= GetBlock("");
		
		$type = SQLITE3_NULL;
		$length = 0;
		$flag = 0;
		
		$str .= GetLongBinary($type);
		$str .= GetLongBinary($flag);
		$str .= GetLongBinary($length);
	}
	echo $str;
}

function EchoData($res, $numfields, $numrows)
{
	for ( $i = 0; $i < $numrows; $i++ ) {
		$str = "";
		$row = sqlite_fetch_array( $res, SQLITE_NUM );
		for( $j = 0; $j < $numfields; $j++ ) {
		if( is_null($row[$j]) )
			$str .= "\xFF";
		else
			$str .= GetBlock($row[$j]);
		$str .= GetLongBinary(-2);
		}
		echo $str;
	}
}

function EchoData3($res, $numfields, $numrows)
{
	while ($row = $res->fetchArray(SQLITE3_NUM)) {
		$str = "";
		for ( $j = 0; $j < $numfields; $j++ ) {
			if ( is_null($row[$j]) )
				$str .= "\xFF";
			else
				$str .= GetBlock($row[$j]);
			$str .= GetLongBinary($res->columnType($j));
		}
		echo $str;
	}
}

function SQLite2($action, $path, $queries)
{
	if (!function_exists("sqlite_open")) {
		EchoHeader(201);
		echo GetBlock("SQLite2 is not supported on the server");
		exit();
	}
	
	$errno_c = 0;
	$conn = sqlite_open($path, 0666, $error_c);
	if ($conn == FALSE) {
		$errno_c = 202;
	}
	
	EchoHeader($errno_c);
	if ($errno_c > 0) {
		echo GetBlock(sqlite_error_string($error_c));
	} elseif($action == "C") {
		EchoConnInfo();
	} elseif($action == "2") {
		sqlite_query($conn, "VACUUM");
		EchoConnInfo();
	} elseif($action == "Q") {
		for($i=0;$i<count($queries);$i++) {
			$query = $queries[$i];
			if ($query == "") continue;
			if (get_magic_quotes_gpc())
				$query = stripslashes($query);
			$res = sqlite_query($conn, $query);
			$errno = sqlite_last_error($conn);
			$affectedrows = sqlite_changes($conn);
			$insertid = sqlite_last_insert_rowid($conn);
			$numfields = sqlite_num_fields($res);
			$numrows = sqlite_num_rows($res);
			EchoResultSetHeader($errno, $affectedrows, $insertid, $numfields, $numrows);
			if ($errno != 0)
				echo GetBlock(sqlite_error_string($errno));
			else {
				if ($numfields > 0) {
					EchoFieldsHeader($res, $numfields);
					EchoData($res, $numfields, $numrows);
				} else {
					echo GetBlock("");
				}
			}
			if ($i<(count($queries)-1))
				echo "\x01";
			else
				echo "\x00";
		}
	}
	
	sqlite_close($conn);
}

function SQLite3($action, $path, $queries)
{
	if (!class_exists("Sqlite3")) {
		EchoHeader(201);
		echo GetBlock("SQLite3 is not supported on the server");
		exit();
	}
	
	$flag = SQLITE3_OPEN_READWRITE;
	if ($action == "3")
		$flag = SQLITE3_OPEN_READWRITE | SQLITE3_OPEN_CREATE;
	
	$errno_c = 0;
	$conn = new SQLite3($path, $flag);
	if ($conn == nil) {
		$errno_c = 202;
	}
	
	EchoHeader($errno_c);
	if ($errno_c > 0) {
		echo GetBlock(SQLite3::lastErrorMsg());
	} else if($action == "C") {
		EchoConnInfo3();
	}  elseif($action == "3") {
		$conn->query("VACUUM");
		EchoConnInfo3();
	} else if($action == "Q") {
		for($i=0;$i<count($queries);$i++) {
			$query = $queries[$i];
			if ($query == "") continue;
			if (get_magic_quotes_gpc())
				$query = stripslashes($query);
			$res = $conn->query($query);
			$errno = $conn->lastErrorCode();
			$affectedrows = $conn->changes();
			$insertid = $conn->lastInsertRowID();
			$numfields = 0;
			$numrows = 0;
			if (is_a($res, "SQLite3Result")) {
				$numfields = $res->numColumns();
				if ($numfields > 0) {
					while ($row = $res->fetchArray(SQLITE3_NUM)) {
						$numrows++;
					}
					$res->reset();
				}
			}
			EchoResultSetHeader($errno, $affectedrows, $insertid, $numfields, $numrows);
			if ($errno != 0)
				echo GetBlock($conn->lastErrorMsg());
			else {
				if ($numfields > 0) {
					EchoFieldsHeader3($res, $numfields);
					EchoData3($res, $numfields, $numrows);
					$res->finalize();
				} else {
					echo GetBlock("");
				}
			}
			if ($i<(count($queries)-1))
				echo "\x01";
			else
				echo "\x00";
		}
	}
	
	$conn->close();
}
	
	if (!isset($_POST["actn"]) || !isset($_POST["dbfile"])) {
		EchoHeader(202);
		echo GetBlock("invalid parameters");
		exit();
	}
	
	if ($action == "2" || $action == "3") {
		if (!isset($_POST["version"])) {
			EchoHeader(202);
			echo GetBlock("invalid parameters");
			exit();
		}
	}
	
	$action = $_POST["actn"];
	$file = $_POST["dbfile"];
	$queries = $_POST["q"];
	
	$status = FileDoesNotExist;
	if (is_file($file)) {
		$fhandle = fopen($file, "r");
		if ($fhandle) {
			$header = "** This file contains an SQLite 2.1 database **";
			$string = fread($fhandle, strlen($header));
			if (strncmp($string, $header, strlen($header)) == 0)
				$status = FileIsSQLite2;
			else
				$status = FileIsSQLite3;
			fclose($fhandle);
		} else {
			$status = FileCannotBeOpened;
		}
	}
	
	if ($action == "2" || $action == "3") {
		if ($status == FileDoesNotExist) {
			if ($action == "2")
				SQLite2($action, $file, $queries);
			else
				SQLite3($action, $file, $queries);
		} else {
			EchoHeader(202);
			echo GetBlock("Datbase file exists already");
		}
	} else {
		switch ($status) {
			case FileDoesNotExist:
				EchoHeader(202);
				echo GetBlock("Database file does not exist");
				break;
			case FileCannotBeOpened:
				EchoHeader(202);
				echo GetBlock("Database file cannot be opened");
				break;
			case FileIsSQLite2:
				SQLite2($action, $file, $queries);
				break;
			case FileIsSQLite3:
				SQLite3($action, $file, $queries);
				break;
		}
	}

?>

