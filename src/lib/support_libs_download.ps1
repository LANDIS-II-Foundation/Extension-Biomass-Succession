# LANDIS-II support library GitHub URL
$master = "https://github.com/LANDIS-II-Foundation/Support-Library-Dlls-v7/raw/master/"


#************************************************
# LANDIS-II support library dependencies
# Modify here when any dependencies changed 

$dlls = "Landis.Library.AgeOnlyCohorts-v3.dll",
"Landis.Library.Cohorts-v2.dll",
"Landis.Library.BiomassCohorts-v3.dll",
"Landis.Library.Metadata-v2.dll",
"Landis.Library.Parameters-v2.dll",
"Landis.Library.Biomass-v2.dll",
"Landis.Library.Climate-v4.dll",
"Landis.Library.Succession-v8.dll",
"Landis.Library.InitialCommunity-v1.dll"
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

