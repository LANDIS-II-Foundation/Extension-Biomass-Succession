Title:			README_BiomassSuccession4.0
Project:		LANDIS-II Landscape Change Model
Project Component:	Extension-Biomass-Succession
Component Deposition:	https://github.com/LANDIS-II-Foundation/Extension-Biomass-Succession
Author:			LANDIS-II Foundation
Origin Date:		25 Mar 2017
Final Date:		15 Jun 2017


Welcome to the source code repository for Extension-Biomass-Succession, a LANDIS-II succession extension.

Biomass Succession calculates how cohorts reproduce, age, and die. In addition, changes in cohort biomass 
(g/m2) are simulated. The Biomass Succession extension tracks dead biomass over time, divided into two 
pools: woody and leaf litter. The Biomass Succession extension generally follows the methods outlined in,

Scheller, R. M. and Mladenoff, D. J. A forest growth and biomass module for a landscape simulation model, 
LANDIS: Design, validation, and application. Ecological Modelling. 2004; 180(1):211-229.


This README file provides the following info:

	1) The basic relationship between 'the science' (various biological, geological, 
geochemical,climatological, ecological, spatial, and landscape ecological mechanisms) 
and 'the model' (LANDIS-II);

	2) The basic process for modifying and subsequently compiling the source code 
(written in C#) into a new, <name-of-your-extension-of-interest>.dll library.

	3) The basic process for testing a new, <name-of-your-extension-of-interest>.dll library.


##########################
The Science and the Model
##########################

The science powering the LANDIS-II model ultimately resides in .cs files, written in 
the C# programming language. The collection of .cs files associated with the LANDIS-II 
Core Model or with any LANDIS-II extension is the so-called source code. Using the 
source code and the .NET Framework, the actual libraries (.dll files) and executables 
(.exe files) of the LANDIS-II model are constructed. The LANDIS-II model then uses 
various sets of libraries and executables to produce process-based output.

The .NET Framework provides the runtime environment needed for executing C# source code.
Executing C# source code means that the source code is compiled to produce an assembly, either 
a library (.dll file) or an executable (.exe file). The C# code in .cs files cannot be 
independently executed; the use of the .NET Framework is required because the C# programming 
language is so-called 'managed code'.

Integrated development environments (IDEs) are used to assist in compiling .cs files into 
assemblies. Visual Studio and MonoDevelop are two useful IDEs for the C# programming language.
To help with tracking the set of .cs files that are to be compiled, Visual Studio 
creates 'container' files called 'projects' and 'solutions'. A 'project' is the collection of 
source code files that the C# compiler will combine to produce a single output (an assembly). 
A Visual Studio project file is designated with a .csproj extension. A 'solution' is a set of 
one or more .csproj files.  A Visual Studio solution file is designated with a .sln extension.


The process of building 'the science' into 'the model' is done via a LANDIS-II extension.
The process looks like this:

==> a set of .cs files is created or modified that translates process-based science into 
    algorithms, and from the algorithms, into C# source code (script)

  ==> a .csproj file is created that links the various .cs files together within an IDE

    ==> the IDE takes the set of .cs files plus the .NET Framework and 'builds' the requisite 
	assemblies: libraries (.dll files) and executables (.exe files). LANDIS-II extensions
	consist ONLY of libraries.

        ==> the newly-built assemblies constitute 'the extension' and are packaged into 
	    a Windows-based installer (a Wizard)

          ==> LANDIS-II users run the Wizard which installs the extension (a set of assemblies)
	      into the following directory: "C:\Program Files\LANDIS-II\v6\bin\extensions\" 






##########################################################
Preliminary notes for building 
a new or modified extension from source code
##########################################################

NB. It is recommended that you use Git for version control and change tracking.
This means cloning the Extension-Base-Harvest repository to your local machine.
Help with Git functionality can be found in the ProGit Manual (freely available)
as a .pdf (https://git-scm.com/book/). A very straighforward Windows/Git interface 
is "git BASH" (available at https://git-for-windows.github.io/)

NB. Should you want the LANDiS-II Foundation to consider your changes for inclsuion in
the LANDIS-II Foundation's main GitHub repository (https://github.com/LANDIS-II-Foundation/)
you will need to submit a Pull request.

NB. Visual Studio (VS) may mark references to some libraries as "unavailable" until the 
solution is actually (re)built. During the build process, VS will automatically retrieve any 
requisite libraries (assmeblies) from the Support-Library-Dlls repository, located at
https://github.com/LANDIS-II-Foundation/Support-Library-Dlls. Retrieval of requisite libraries 
is done by running the script, "install-libs.cmd" as a pre-build event. 

NB. Libraries such as "System" and "System.Core" are assemblies that should be available on 
your machine as part of the .Net Framework. For example, examining "System" and "System.Core" 
in References (Solution Explorer ==> References) yields the following output in an Object 
Browser window in VS,

Assembly System
    C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.dll

Assembly System.Core
    C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.5\System.Core.dll



##########################################################
Step-by-step instructions for building 
a new or modified extension from source code
##########################################################

The following steps use both a Git command-line interface (GitBASH for Windows; see above) and  
the menu-driven options available in Visual Studio (VS). 

The example given below is a specific one and uses the extension, "Extension-Base-Harvest". The repo for this
extension is available at https://github.com/LANDIS-II-Foundation/Extension-Base-Harvest.


Substitute, as appropriate, for your extension of interest.

===== STEP1. Clone and .git track a local version of an extension repository ================

	a. Clone the extension repository (repo) of interest to your machine.
	a1. The COPY of the extension repo on your machine is the LOCAL repo;
	    the SOURCE of the cloned repo is the REMOTE repo

$ git clone https://github.com/LANDIS-II-Foundation/Extension-Base-Harvest.git
Cloning into 'Extension-Base-Harvest'...
remote: Counting objects: 429, done.
remote: Compressing objects: 100% (17/17), done.
remote: Total 429 (delta 6), reused 0 (delta 0), pack-reused 410
Receiving objects: 100% (429/429), 2.54 MiB | 1.09 MiB/s, done.
Resolving deltas: 100% (263/263), done.

$ git remote -v
origin  https://github.com/LANDIS-II-Foundation/Extension-Base-Harvest.git (fetch)
origin  https://github.com/LANDIS-II-Foundation/Extension-Base-Harvest.git (push)


==== STEP2. Setup Visual Studio (VS)  ==================================================

	a. Open VS and load the "base-harvest.csproj" file
Project ==> Open ==> Project/Solution



	b. Select the 'Solution Explorer' tab to see .cs files, References 
Solution 'base harvest' (1 project)
  C# base-harvest
    Prperties
    References
    EventsLog.cs
    InputParametersParser.cs
    MetadataHandler.cs
    packages.config
    PlugIn.cs
    SummaryLog.cs



	c. VS has added three (3) directories to your LOCAL repo:


...src\.vs\
...src\bin\		
...src\obj\


===== STEP3. (Re)build the project ==========================================================

	a. Under the "Build" tab, select "Build base-harvest"

	b. If the VS build is successful, two new files will have been created:

Landis.Extension.BaseHarvest-3.0.dll	#the functional extension to be used by the LANDIS-II
					Core-Model

Landis.Extension.BaseHarvest-3.0.pdb	#a program database(.pdb) file for storing debugging 
					information about the .dll; created from source files 
					during compilation and not needed by LANDIS-II


	b1. Note that "... \Extension-Base-Harvest\src\obj\Debug" has a text file listing the 
files created by the (re)build (see <name>.csproj.FileListAbsolute.txt)


	b2.  Note that "... \Extension-Base-Harvest\src\bin\Debug" is now populated with various 
reference libraries required for the (re)build. A copy of the newly built, 
"Landis.Extension.BaseHarvest-3.0.dll" library is also found here.




####################################################
Testing the (re)built extension
(ie, "Landis.Extension.BaseHarvest-3.0.dll")
#####################################################

==== STEP1. remove the currently installed BaseHarvest extension and re-install =============================================== 

	a. Use the Windows 'Control Panel' to uninstall the current version of LANDIS-II Base Harvest
 
	b. Open 'Base Harvest v3.0.iss' in Inno Script Studio (https://www.kymoto.org/products/inno-script-studio)

	c. Compile (CTRL-F9) 'Base Harvest v3.0.iss' to produce an installer, 'LANDIS-II Base Harvest 3.0-setup.exe'

	d. Run the new installer 


===== STEP2. Perform a test run ===========================================================

	a. With LANDIS-II already installed, run LANDIS-II with the Base Harvest example provided with 
	   Extension-Base-Harvest (found in the folder, \Extension-Base-Harvest\deploy\examples OR 
	   after installation, found in the folder, \Program Files\LANDIS-II\v6\examples\Base Harvest)

	b. Open a command prompt (as administrator) and enter the following command at the command line, 
call landis-ii scenario_s1e1.txt

	c. The following console (screen) output, somewhat truncated from the actual output,
	   is expected:

C:\Program Files\LANDIS-II\v6\examples\Base Harvest>call landis-ii scenario_s1e1.txt
LANDIS-II 6.1 (official release)

Loading scenario from file "scenario_s1e1.txt" ...
Initialized random number generator with seed = 1,620,247,537
Loading species data from file "./species_s1e1.txt" ...
Loading ecoregions from file "./ecoregions_s1e1.txt" ...
Initializing landscape from ecoregions map "./ecoregions_s1e1.gis" ...
Cell length = 100 m, cell area = 1 ha
Map dimensions: 99 rows by 99 columns = 9,801 cells
Sites: 9,801 active (100.0 %), 0 inactive (0.0 %)
  reading in ecoregion from ./ecoregions_s1e1.gis
Loading Age-only Succession extension ...
   Registering Data:  Succession.AgeCohorts.
   Loading dynamic input data from file "./age-only-succession_DynamicInputs_s1e1.txt" ...
  Dynamic Input Parser:  Add new year = 0.
   Registering Data:  TimeOfLastSuccession.
   Registering Data:  Shade.
   Creating Dispersal Neighborhood List.
   Dispersal:  NeighborRadius=5050, CellLength=100, numCellRadius=50
   Loading initial communities from file "./age-only-succession_InitialCommunities_s1e1.txt" ...
   Reading initial communities map "./age-only-succession_InitialCommunities_s1e1.gis" ...
Loading Base Wind extension ...
Loading Base Harvest extension ...
   Registering Data:  Harvest.PrescriptionName.
   Registering Data:  Harvest.TimeOfLastEvent.
   Registering Data:  Harvest.CohortsDamaged.
   Registering Data:  Wind.TimeOfLastEvent.
   Registering Data:  Wind.Severity.
   Generating event table...
   Generating summary table...
   Reading management-area map ./base-harvest_Management_s1e1.gis ...
   Reading stand map ./base-harvest_Stand_s1e1.gis ...
Loading Output Max Species Age extension ...
Loading Output Cohort Statistics extension ...
Using the following extensions ...
   Extension Name            Extension Filename
   --------------            ------------------
   Age-only Succession       age-only-succession_SetUp_s1e1.txt
   Base Wind                 base-wind_SetUp_s1e1.txt
   Base Harvest              base-harvest_SetUp_s1e1.txt
   Output Max Species Age    output_MaxSppAge.txt
   Output Cohort Statistics  output_CohortStats.txt

Running Output Max Species Age ...
   Writing maximum age map for abiebals to outputs/max-age-selected-spp/abiebals-0.gis ...
dataset created: outputs/max-age-selected-spp/abiebals-0.gis
   Writing maximum age map for all species to outputs/max-age-selected-spp/AllSppMaxAge-0.gis ...
dataset created: outputs/max-age-selected-spp/AllSppMaxAge-0.gis
Running Output Cohort Statistics ...
   Writing MIN map for tsugcana to outputs/age-per-spp/tsugcana-MIN-0.img ...
dataset created: outputs/age-per-spp/tsugcana-MIN-0.img
   Writing MIN map for betupapy to outputs/age-per-spp/betupapy-MIN-0.img ...
dataset created: outputs/age-per-spp/betupapy-MIN-0.img
   Writing MAX map for tsugcana to outputs/age-per-spp/tsugcana-MAX-0.img ...
dataset created: outputs/age-per-spp/tsugcana-MAX-0.img
   Writing MAX map for abiebals to outputs/age-per-spp/abiebals-MAX-0.img ...
dataset created: outputs/age-per-spp/abiebals-MAX-0.img
   Writing MAX map for tiliamer to outputs/age-per-spp/tiliamer-MAX-0.img ...
dataset created: outputs/age-per-spp/tiliamer-MAX-0.img
   Writing MED map for tsugcana to outputs/age-per-spp/tsugcana-MED-0.img ...
dataset created: outputs/age-per-spp/tsugcana-MED-0.img
   Writing SD map for tsugcana to outputs/age-per-spp/tsugcana-SD-0.img ...
dataset created: outputs/age-per-spp/tsugcana-SD-0.img
   Writing AVG map for tsugcana to outputs/age-per-spp/tsugcana-AVG-0.img ...
dataset created: outputs/age-per-spp/tsugcana-AVG-0.img
   Writing MIN site map to outputs/age-all-spp/AGE-MIN-0.img ...
dataset created: outputs/age-all-spp/AGE-MIN-0.img
   Writing MAX site map to outputs/age-all-spp/AGE-MAX-0.img ...
dataset created: outputs/age-all-spp/AGE-MAX-0.img
   Writing MED site map to outputs/age-all-spp/AGE-MED-0.img ...
dataset created: outputs/age-all-spp/AGE-MED-0.img
   Writing AVG site map to outputs/age-all-spp/AGE-AVG-0.img ...
dataset created: outputs/age-all-spp/AGE-AVG-0.img
   Writing RICH site map to outputs/age-all-spp/AGE-RICH-0.img ...
dataset created: outputs/age-all-spp/AGE-RICH-0.img
   Writing EVEN site map to outputs/age-all-spp/AGE-EVEN-0.img ...
dataset created: outputs/age-all-spp/AGE-EVEN-0.img
   Writing COUNT site map to outputs/age-all-spp/AGE-COUNT-0.img ...
dataset created: outputs/age-all-spp/AGE-COUNT-0.img
   Writing RICH site map to outputs/spp-counts/SPP-RICH-0.img ...
dataset created: outputs/spp-counts/SPP-RICH-0.img
Current time: 5
Running Base Wind ...
Processing landscape for wind events ...
  Wind events: 10
dataset created: wind/severity-5.img
Running Age-only Succession ...
Running Output Max Species Age ...
   Writing maximum age map for abiebals to outputs/max-age-selected-spp/abiebals-5.gis ...
dataset created: outputs/max-age-selected-spp/abiebals-5.gis
   Writing maximum age map for all species to outputs/max-age-selected-spp/AllSppMaxAge-5.gis ...
dataset created: outputs/max-age-selected-spp/AllSppMaxAge-5.gis
Current time: 10
Running Base Wind ...
Processing landscape for wind events ...
  Wind events: 9
dataset created: wind/severity-10.img
Running Base Harvest ...
dataset created: harvest/base-harvest-prescripts-10.img
Running Age-only Succession ...
Ageing cohorts ...
% done:   0%  10%  20%  30%  40%  50%  60%  70%  80%  90%  100%
          |----|----|----|----|----|----|----|----|----|----|
Progress: +++++++++++++++++++++++++++++++++++++++++++++++++++
Computing shade ...
% done:   0%  10%  20%  30%  40%  50%  60%  70%  80%  90%  100%
          |----|----|----|----|----|----|----|----|----|----|
Progress: +++++++++++++++++++++++++++++++++++++++++++++++++++
Cohort reproduction ...
% done:   0%  10%  20%  30%  40%  50%  60%  70%  80%  90%  100%
          |----|----|----|----|----|----|----|----|----|----|
Progress: +++++++++++++++++++++++++++++++++++++++++++++++++++
Running Output Max Species Age ...
   Writing maximum age map for abiebals to outputs/max-age-selected-spp/abiebals-10.gis ...
dataset created: outputs/max-age-selected-spp/abiebals-10.gis
   Writing maximum age map for all species to outputs/max-age-selected-spp/AllSppMaxAge-10.gis ...
dataset created: outputs/max-age-selected-spp/AllSppMaxAge-10.gis
Running Output Cohort Statistics ...
   Writing MIN map for tsugcana to outputs/age-per-spp/tsugcana-MIN-10.img ...
dataset created: outputs/age-per-spp/tsugcana-MIN-10.img
   Writing MIN map for betupapy to outputs/age-per-spp/betupapy-MIN-10.img ...
dataset created: outputs/age-per-spp/betupapy-MIN-10.img
   Writing MAX map for tsugcana to outputs/age-per-spp/tsugcana-MAX-10.img ...
dataset created: outputs/age-per-spp/tsugcana-MAX-10.img
   Writing MAX map for abiebals to outputs/age-per-spp/abiebals-MAX-10.img ...
dataset created: outputs/age-per-spp/abiebals-MAX-10.img
   Writing MAX map for tiliamer to outputs/age-per-spp/tiliamer-MAX-10.img ...
dataset created: outputs/age-per-spp/tiliamer-MAX-10.img
   Writing MED map for tsugcana to outputs/age-per-spp/tsugcana-MED-10.img ...
dataset created: outputs/age-per-spp/tsugcana-MED-10.img
   Writing SD map for tsugcana to outputs/age-per-spp/tsugcana-SD-10.img ...
dataset created: outputs/age-per-spp/tsugcana-SD-10.img
   Writing AVG map for tsugcana to outputs/age-per-spp/tsugcana-AVG-10.img ...
dataset created: outputs/age-per-spp/tsugcana-AVG-10.img
   Writing MIN site map to outputs/age-all-spp/AGE-MIN-10.img ...
dataset created: outputs/age-all-spp/AGE-MIN-10.img
   Writing MAX site map to outputs/age-all-spp/AGE-MAX-10.img ...
dataset created: outputs/age-all-spp/AGE-MAX-10.img
   Writing MED site map to outputs/age-all-spp/AGE-MED-10.img ...
dataset created: outputs/age-all-spp/AGE-MED-10.img
   Writing AVG site map to outputs/age-all-spp/AGE-AVG-10.img ...
dataset created: outputs/age-all-spp/AGE-AVG-10.img
   Writing RICH site map to outputs/age-all-spp/AGE-RICH-10.img ...
dataset created: outputs/age-all-spp/AGE-RICH-10.img
   Writing EVEN site map to outputs/age-all-spp/AGE-EVEN-10.img ...
dataset created: outputs/age-all-spp/AGE-EVEN-10.img
   Writing COUNT site map to outputs/age-all-spp/AGE-COUNT-10.img ...
dataset created: outputs/age-all-spp/AGE-COUNT-10.img
   Writing RICH site map to outputs/spp-counts/SPP-RICH-10.img ...
dataset created: outputs/spp-counts/SPP-RICH-10.img
Current time: 15
Running Base Wind ...
Processing landscape for wind events ...
  Wind events: 4
dataset created: wind/severity-15.img
Running Age-only Succession ...
Running Output Max Species Age ...
   Writing maximum age map for abiebals to outputs/max-age-selected-spp/abiebals-15.gis ...
dataset created: outputs/max-age-selected-spp/abiebals-15.gis
   Writing maximum age map for all species to outputs/max-age-selected-spp/AllSppMaxAge-15.gis ...
dataset created: outputs/max-age-selected-spp/AllSppMaxAge-15.gis
Current time: 20
Running Base Wind ...
Processing landscape for wind events ...
  Wind events: 2
dataset created: wind/severity-20.img
Running Base Harvest ...
dataset created: harvest/base-harvest-prescripts-20.img
Running Age-only Succession ...
Ageing cohorts ...
% done:   0%  10%  20%  30%  40%  50%  60%  70%  80%  90%  100%
          |----|----|----|----|----|----|----|----|----|----|
Progress: +++++++++++++++++++++++++++++++++++++++++++++++++++
Computing shade ...
% done:   0%  10%  20%  30%  40%  50%  60%  70%  80%  90%  100%
          |----|----|----|----|----|----|----|----|----|----|
Progress: +++++++++++++++++++++++++++++++++++++++++++++++++++
Cohort reproduction ...
% done:   0%  10%  20%  30%  40%  50%  60%  70%  80%  90%  100%
          |----|----|----|----|----|----|----|----|----|----|
Progress: +++++++++++++++++++++++++++++++++++++++++++++++++++
Running Output Max Species Age ...
   Writing maximum age map for abiebals to outputs/max-age-selected-spp/abiebals-20.gis ...
dataset created: outputs/max-age-selected-spp/abiebals-20.gis
   Writing maximum age map for all species to outputs/max-age-selected-spp/AllSppMaxAge-20.gis ...
dataset created: outputs/max-age-selected-spp/AllSppMaxAge-20.gis
Running Output Cohort Statistics ...
   Writing MIN map for tsugcana to outputs/age-per-spp/tsugcana-MIN-20.img ...
dataset created: outputs/age-per-spp/tsugcana-MIN-20.img
   Writing MIN map for betupapy to outputs/age-per-spp/betupapy-MIN-20.img ...
dataset created: outputs/age-per-spp/betupapy-MIN-20.img
   Writing MAX map for tsugcana to outputs/age-per-spp/tsugcana-MAX-20.img ...
dataset created: outputs/age-per-spp/tsugcana-MAX-20.img
   Writing MAX map for abiebals to outputs/age-per-spp/abiebals-MAX-20.img ...
dataset created: outputs/age-per-spp/abiebals-MAX-20.img
   Writing MAX map for tiliamer to outputs/age-per-spp/tiliamer-MAX-20.img ...
dataset created: outputs/age-per-spp/tiliamer-MAX-20.img
   Writing MED map for tsugcana to outputs/age-per-spp/tsugcana-MED-20.img ...
dataset created: outputs/age-per-spp/tsugcana-MED-20.img
   Writing SD map for tsugcana to outputs/age-per-spp/tsugcana-SD-20.img ...
dataset created: outputs/age-per-spp/tsugcana-SD-20.img
   Writing AVG map for tsugcana to outputs/age-per-spp/tsugcana-AVG-20.img ...
dataset created: outputs/age-per-spp/tsugcana-AVG-20.img
   Writing MIN site map to outputs/age-all-spp/AGE-MIN-20.img ...
dataset created: outputs/age-all-spp/AGE-MIN-20.img
   Writing MAX site map to outputs/age-all-spp/AGE-MAX-20.img ...
dataset created: outputs/age-all-spp/AGE-MAX-20.img
   Writing MED site map to outputs/age-all-spp/AGE-MED-20.img ...
dataset created: outputs/age-all-spp/AGE-MED-20.img
   Writing AVG site map to outputs/age-all-spp/AGE-AVG-20.img ...
dataset created: outputs/age-all-spp/AGE-AVG-20.img
   Writing RICH site map to outputs/age-all-spp/AGE-RICH-20.img ...
dataset created: outputs/age-all-spp/AGE-RICH-20.img
   Writing EVEN site map to outputs/age-all-spp/AGE-EVEN-20.img ...
dataset created: outputs/age-all-spp/AGE-EVEN-20.img
   Writing COUNT site map to outputs/age-all-spp/AGE-COUNT-20.img ...
dataset created: outputs/age-all-spp/AGE-COUNT-20.img
   Writing RICH site map to outputs/spp-counts/SPP-RICH-20.img ...
dataset created: outputs/spp-counts/SPP-RICH-20.img
Current time: 25
Running Base Wind ...
Processing landscape for wind events ...
  Wind events: 3
dataset created: wind/severity-25.img
Running Age-only Succession ...
Running Output Max Species Age ...
   Writing maximum age map for abiebals to outputs/max-age-selected-spp/abiebals-25.gis ...
dataset created: outputs/max-age-selected-spp/abiebals-25.gis
   Writing maximum age map for all species to outputs/max-age-selected-spp/AllSppMaxAge-25.gis ...
dataset created: outputs/max-age-selected-spp/AllSppMaxAge-25.gis
Current time: 30
Running Base Wind ...
Processing landscape for wind events ...
  Wind events: 5
dataset created: wind/severity-30.img
Running Base Harvest ...
dataset created: harvest/base-harvest-prescripts-30.img
Running Age-only Succession ...
Ageing cohorts ...
% done:   0%  10%  20%  30%  40%  50%  60%  70%  80%  90%  100%
          |----|----|----|----|----|----|----|----|----|----|
Progress: +++++++++++++++++++++++++++++++++++++++++++++++++++
Computing shade ...
% done:   0%  10%  20%  30%  40%  50%  60%  70%  80%  90%  100%
          |----|----|----|----|----|----|----|----|----|----|
Progress: +++++++++++++++++++++++++++++++++++++++++++++++++++
Cohort reproduction ...
% done:   0%  10%  20%  30%  40%  50%  60%  70%  80%  90%  100%
          |----|----|----|----|----|----|----|----|----|----|
Progress: +++++++++++++++++++++++++++++++++++++++++++++++++++
Running Output Max Species Age ...
 
.....

   Writing EVEN site map to outputs/age-all-spp/AGE-EVEN-100.img ...
dataset created: outputs/age-all-spp/AGE-EVEN-100.img
   Writing COUNT site map to outputs/age-all-spp/AGE-COUNT-100.img ...
dataset created: outputs/age-all-spp/AGE-COUNT-100.img
   Writing RICH site map to outputs/spp-counts/SPP-RICH-100.img ...
dataset created: outputs/spp-counts/SPP-RICH-100.img
Model run is complete.

