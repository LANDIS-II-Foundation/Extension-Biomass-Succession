# LANDIS-II support library GitHub URL
$master = "https://github.com/LANDIS-II-Foundation/Support-Library-Dlls-v8/raw/main/"


#************************************************
# LANDIS-II support library dependencies
# Modify here when any dependencies changed 

$dlls = "Landis.Library.UniversalCohorts-v1.dll",
"Landis.Library.Metadata-v2.dll",
"Landis.Library.Parameters-v2.dll",
"Landis.Library.Climate-v4.4.dll",
"Landis.Library.Succession-v9.dll",
"Landis.Library.InitialCommunity.Universal.dll"
#************************************************


# LANDIS-II support libraries download
$current = Get-Location
$outpath = $current.toString() + "/"

try {
	ForEach ($item in $dlls) {
		$dll = $outpath + $item
		$url = $master + $item
		[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
		Invoke-WebRequest -uri $url -Outfile $dll
		($dll).split('/')[-1].toString() + "------------- downloaded"
	}
	"`n***** Download complete *****`n"
}
catch [System.Net.WebException],[System.IO.IOException]{
	"Unable to download file from " + $item.toString()
}
catch {
	"An error occurred."
}

